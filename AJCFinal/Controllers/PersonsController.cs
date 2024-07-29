using AJCFinal.DataTransfertObjects;
using AJCFinal.Models.Person;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;

namespace AJCFinal.Controllers
{
    public class PersonsController : Controller
    {
        private readonly HttpClient httpClient;

        public PersonsController(IHttpClientFactory httpClientFactory)
        {
            this.httpClient = httpClientFactory.CreateClient("MyApiClient");
        }
        public async Task<IActionResult> Index()
        {
            var httpResponse = await this.httpClient.GetAsync("api/persons");

            if (httpResponse.StatusCode == HttpStatusCode.Unauthorized)
                return Unauthorized();

            if (!httpResponse.IsSuccessStatusCode)
                return View(new List<PersonBaseViewModel>());

            var personsFromApi = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<PersonDto>>();

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
    }
}
