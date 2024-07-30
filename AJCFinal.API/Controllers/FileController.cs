using AJCFinal.API.Models;
using AJCFinal.Business.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AJCFinal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileService filesServices;
        private readonly IPersonService personService;

        public FilesController(IFileService filesServices, IPersonService personService)
        {
            this.filesServices = filesServices;
            this.personService = personService;
        }

        [HttpGet("{blobName}")]
        public async Task<string> GetAsync(string blobName)
        {
            byte[] bytes = await filesServices.GetFromAzureAsync(blobName);
            return Convert.ToBase64String(bytes);
        }

        [HttpDelete("{id}")]
        public async void DeleteAsync(string id)
        {
            if (id is not null)
            {
                var person = personService.GetPersonByIdAsync(int.Parse(id));

                if (person is not null)
                {
                    await filesServices.DeleteFromAzureAsync($"{person.Result.FirstName}_{person.Result.LastName}");
                }
            }
        }

        [HttpPost]
        public async void PostAsync([FromBody] FileInput fileInput)
        {
            byte[] data = Convert.FromBase64String(fileInput.Content);
            await filesServices.SendToAzureAsync(data, fileInput.Name, fileInput.ContentType);
        }
    }
}
