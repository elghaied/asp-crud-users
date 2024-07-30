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

            try
            {
                var httpResponse = await this.httpClient.GetAsync("api/Person");

                if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                    return Unauthorized();

                if (!httpResponse.IsSuccessStatusCode)
                    return View(new List<PersonBaseViewModel>());

                personsFromApi = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<PersonDto>>();

                var userId = User.FindFirst("UserId")?.Value;

                if (long.TryParse(userId, out long currentUserId))
                {

                    personsFromApi = personsFromApi.Where(p => p.Id != currentUserId).ToList();
                }

            }
            catch (HttpRequestException)
            {
                personsFromApi = await this.personService.GetPersonsAsync();
            }


            

            var personsVm = personsFromApi?.Select(personDto => PersonBaseViewModel.FromDto(personDto)) ?? new List<PersonBaseViewModel>();

            return View(personsVm);
            
        }

        public async Task<IActionResult> Profile(int? id)
        {
   
            var httpResponse = await this.httpClient.GetAsync($"api/Person/{id}");

            if (!httpResponse.IsSuccessStatusCode)
                return NotFound();

            var personFromApi = await httpResponse.Content.ReadFromJsonAsync<PersonDto>();
            var vm = PersonBaseViewModel.FromDto(personFromApi ?? new PersonDto());

            httpResponse = await this.httpClient.GetAsync($"api/files/{vm.FirstName}-thumbs");

            if (httpResponse.IsSuccessStatusCode)
            {
                vm.Image = await httpResponse.Content.ReadAsStringAsync();
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddFriend(long friendId)
        {
            var userIdClaim = User.FindFirst("UserId");
            if (userIdClaim == null)
            {
                return Unauthorized();
            }

            var personId = long.Parse(userIdClaim.Value);
            var response = await this.httpClient.PostAsync($"api/Person/AddFriend?personId={personId}&friendId={friendId}", null);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Profile", new { id = friendId });
            }

            // Handle failure to add friend appropriately
            return BadRequest("Failed to add friend.");
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.Email);
        }

    }
}
