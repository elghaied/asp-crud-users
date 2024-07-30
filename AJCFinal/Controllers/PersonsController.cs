using AJCFinal.DataTransfertObjects;
using AJCFinal.Models.Person;
using AJCFinal.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;

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

                    // Get the current user's friends using the new endpoint
                    var friendsResponse = await this.httpClient.GetAsync($"api/Person/{currentUserId}/friends");
                    if (friendsResponse.IsSuccessStatusCode)
                    {
                        friendsFromApi = await friendsResponse.Content.ReadFromJsonAsync<IEnumerable<PersonDto>>();
                    }
                }
            }
            catch (HttpRequestException)
            {
                personsFromApi = await this.personService.GetPersonsAsync();
                // If there's an exception, we'll assume the user has no friends to avoid additional exceptions
                friendsFromApi = new List<PersonDto>();
            }

            var personsVm = personsFromApi?.Select(personDto =>
            {
                var vm = PersonBaseViewModel.FromDto(personDto);
                vm.IsFriend = friendsFromApi.Any(f => f.Id == personDto.Id);
                return vm;
            }) ?? new List<PersonBaseViewModel>();

            return View(personsVm);
        }

        public async Task<IActionResult> Profile(int? id)
        {
            var httpResponse = await this.httpClient.GetAsync($"api/Person/{id}");
            if (!httpResponse.IsSuccessStatusCode)
                return NotFound();
            var personFromApi = await httpResponse.Content.ReadFromJsonAsync<PersonDto>();
            var vm = PersonBaseViewModel.FromDto(personFromApi ?? new PersonDto());

            // Check if the viewed profile is a friend of the current user
            var userId = User.FindFirst("UserId")?.Value;
            if (long.TryParse(userId, out long currentUserId))
            {
                var friendsResponse = await this.httpClient.GetAsync($"api/Person/{currentUserId}/friends");
                if (friendsResponse.IsSuccessStatusCode)
                {
                    var friendsFromApi = await friendsResponse.Content.ReadFromJsonAsync<IEnumerable<PersonDto>>();
                    vm.IsFriend = friendsFromApi.Any(f => f.Id == vm.Id);
                }
            }

            httpResponse = await this.httpClient.GetAsync($"api/files/{vm.FirstName}-thumbs");
            if (httpResponse.IsSuccessStatusCode)
            {
                vm.Image = await httpResponse.Content.ReadAsStringAsync();
            }
            return View(vm);
        }

        //[HttpPost]
        //public async Task<IActionResult> AddFriend(long friendId)
        //{
        //    var userIdClaim = User.FindFirst("UserId");
        //    if (userIdClaim == null)
        //    {
        //        return Unauthorized();
        //    }

        //    var personId = long.Parse(userIdClaim.Value);
        //    var response = await this.httpClient.PostAsync($"api/Person/AddFriend?personId={personId}&friendId={friendId}", null);

        //    if (response.IsSuccessStatusCode)
        //    {
        //        return RedirectToAction("Profile", new { id = friendId });
        //    }

        //    // Handle failure to add friend appropriately
        //    return BadRequest("Failed to add friend.");
        //}

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
            var response = await httpClient.PostAsync($"api/Persons/RemoveFriend?personId={currentUserId}&friendId={friendId}", null);

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
