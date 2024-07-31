using AJCFinal.DataTransfertObjects;
using AJCFinal.Models.Person;
using AJCFinal.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Text;

namespace AJCFinal.Controllers
{
    public class PersonsController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly IPersonService personService;

        public PersonsController(IHttpClientFactory httpClientFactory, IPersonService personService)
        {
            this.httpClient = httpClientFactory.CreateClient("MyApiClient");
            this.personService = personService;
        }
        public async Task<IActionResult> Index()
        {
            IEnumerable<PersonDto> personsFromApi;
            IEnumerable<PersonDto> friendsFromApi = new List<PersonDto>();
            var userId = User.FindFirst("UserId")?.Value;
            long currentUserId = 0;

            try
            {
                var httpResponse = await this.httpClient.GetAsync("api/Person");
                if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                    return Unauthorized();
                if (!httpResponse.IsSuccessStatusCode)
                    return View(new List<PersonBaseViewModel>());
                personsFromApi = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<PersonDto>>();

                if (long.TryParse(userId, out currentUserId))
                {
                    personsFromApi = personsFromApi.Where(p => p.Id != currentUserId).ToList();

                }
            }
            catch (HttpRequestException)
            {
                personsFromApi = await this.personService.GetPersonsAsync();
             
                friendsFromApi = new List<PersonDto>();
            }

            var personsVm = personsFromApi?.Select(personDto =>
            {
                var vm = PersonBaseViewModel.FromDto(personDto);
                vm.IsFriend = personDto.FriendIds.Contains(currentUserId);
                return vm;
            }) ?? new List<PersonBaseViewModel>();

            return View(personsVm);
        }

        public async Task<IActionResult> Profile(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            var httpResponse = await this.httpClient.GetAsync($"api/Person/{id}");
            if (!httpResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var personFromApi = await httpResponse.Content.ReadFromJsonAsync<PersonDto>();
            if (personFromApi == null)
            {
                return NotFound();
            }

            var vm = PersonBaseViewModel.FromDto(personFromApi);

            // Check if the viewed profile is a friend of the current user
            var userId = User.FindFirst("UserId")?.Value;
            if (long.TryParse(userId, out long currentUserId))
            {
                vm.IsFriend = personFromApi.FriendIds.Contains(currentUserId);
            }

            foreach (var friendId in personFromApi.FriendIds)
            {
                var friendHttpResponse = await this.httpClient.GetAsync($"api/Person/{friendId}");
                var friendFromApi = await friendHttpResponse.Content.ReadFromJsonAsync<PersonDto>();
                var friend = PersonBaseViewModel.FromDto(friendFromApi);
                if (friend != null)
                    vm.Friends.Add(friend);

            }
   

            // Fetch the person's profile image
            httpResponse = await this.httpClient.GetAsync($"api/files/{vm.FirstName}-thumbs");
            if (httpResponse.IsSuccessStatusCode)
            {
                vm.Image = await httpResponse.Content.ReadAsStringAsync();
            }

            return View(vm);
        }


        [HttpGet]
        public async Task<IActionResult> Update(long id)
        {
            var response = await this.httpClient.GetAsync($"api/Person/{id}");
            if (response.IsSuccessStatusCode)
            {
                var person = await response.Content.ReadFromJsonAsync<PersonUpdateViewModel>();
                return View(person);
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Error fetching profile. Server response: {errorContent}");
                return View();
            }
        }
        [HttpPost]
        public async Task<IActionResult> Update(long id, PersonUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId) || long.Parse(userId) != id)
            {
                return Forbid();
            }

            var personInput = new
            {
                Id = id,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                Address = model.Address,
                Phone = model.Phone,
                Interests = model.Interests,
           // Has to be reomved from here 
            };

            var httpContent = new StringContent(JsonSerializer.Serialize(personInput), Encoding.UTF8, "application/json");
            var response = await httpClient.PutAsync($"api/Person/{id}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                if (model.Image != null)
                {
                    await using var memoryStream = new MemoryStream();
                    await model.Image.CopyToAsync(memoryStream);
                    await this.httpClient.PostAsJsonAsync("api/Files", new
                    {
                        Name = model.Email,
                        Content = Convert.ToBase64String(memoryStream.ToArray()),
                        ContentType = model.Image.ContentType
                    });
                }
                return RedirectToAction(nameof(Profile), new { id = id });
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Profile update failed. Server response: {errorContent}");
            }

            return View(model);
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.Email);
        }


        [HttpPost]
        public async Task<IActionResult> AddFriend(long friendId)
        {
            var currentUserId = long.Parse(User.FindFirst("UserId").Value);
            var response = await httpClient.PostAsync($"api/Person/AddFriend?personId={currentUserId}&friendId={friendId}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Friend added successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to add friend.";
            }

            return RedirectToAction("Profile", new { id = friendId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend(long friendId)
        {
            var currentUserId = long.Parse(User.FindFirst("UserId").Value);
            var response = await httpClient.PostAsync($"api/Person/RemoveFriend?personId={currentUserId}&friendId={friendId}", null);

            if (response.IsSuccessStatusCode)
            {
                TempData["Message"] = "Friend removed successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to remove friend.";
            }

            return RedirectToAction("Profile", new { id = friendId });
        }
    }
}
