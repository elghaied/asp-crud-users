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
                var httpResponse = await this.httpClient.GetAsync("api/persons");

                if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                    return Unauthorized();

                if (!httpResponse.IsSuccessStatusCode)
                    return View(new List<PersonBaseViewModel>());

                personsFromApi = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<PersonDto>>();
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
   
            var httpResponse = await this.httpClient.GetAsync($"api/persons/{id}");

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
            var currentUserId = GetCurrentUserId();
            if (currentUserId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var httpResponse = await this.httpClient.PostAsync($"api/persons/{currentUserId}/friends/{friendId}", null);

            if (httpResponse.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Friend added successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to add friend. Please try again later.";
            }

            return RedirectToAction(nameof(Index));
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.Email);
        }

    }
}
