using System.Net;
using System.Text.Json;
using System.Text;
using AJCFinal.DataTransfertObjects;
using AJCFinal.Models.Person;
using AJCFinal.Services.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace AJCFinal.Controllers
{
    public class AdminsController : Controller
    {
        private readonly HttpClient httpClient;
        private readonly IPersonService personService;

        public AdminsController(IHttpClientFactory httpClientFactory, IPersonService personService)
        {
            this.httpClient = httpClientFactory.CreateClient("MyApiClient");
            this.personService = personService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            IEnumerable<PersonDto> personsFromApi;
            var userId = User.FindFirst("UserId")?.Value;
            long currentUserId = 0;

            try
            {
                var httpResponse = await httpClient.GetAsync("api/Person");
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
                personsFromApi = await personService.GetPersonsAsync();
            }

            var personsVm = personsFromApi?.Select(personDto =>
            {
                var vm = PersonBaseViewModel.FromDto(personDto);
                return vm;
            }) ?? new List<PersonBaseViewModel>();

            return View(personsVm);
        }

      
        public IActionResult Create()
        {
            var vm = new PersonInputViewModel();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PersonInputViewModel model)
        {
            if (ModelState.IsValid)
            {
                var personInput = new
                {
                    Email = model.Email,
                    HashedPassword = model.Password,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth
                };

                var httpContent = new StringContent(JsonSerializer.Serialize(personInput), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("api/Auth/register", httpContent);

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
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Registration failed. Server response: {errorContent}");
                }
            }
            return View(model);
        }
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(int id)
		{
			var httpResponse = await this.httpClient.DeleteAsync($"api/Files/{id}");

			if (httpResponse.IsSuccessStatusCode)
			{
				httpResponse = await this.httpClient.DeleteAsync($"api/Person/{id}");

				return RedirectToAction("Index");
			}

			return NotFound();
		}

	}
}
