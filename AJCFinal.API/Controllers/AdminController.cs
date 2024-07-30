using AJCFinal.API.Models;
using AJCFinal.Business.Abstractions;
using AJCFinal.Business.DataTransfertObjects;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AJCFinal.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService  adminService;
        public AdminController(IAdminService adminService)
        {
            this.adminService = adminService;
        }
        [HttpPost("users")]
        public async Task<ActionResult<long>> CreateUser([FromBody] UserInput userInput)
        {
            long createdId;

            if (userInput.IsAdmin)
            {
                var adminDto = new AdminDto
                {
                    Email = userInput.Email,
                    HashedPassword = userInput.HashedPassword,
                    LastName = userInput.LastName,
                    FirstName = userInput.FirstName,
                    DateOfBirth = userInput.DateOfBirth
                };
                createdId = await this.adminService.CreateUserAsync(adminDto);
            }
            else
            {
                var personDto = new PersonDto
                {
                    Email = userInput.Email,
                    HashedPassword = userInput.HashedPassword,
                    LastName = userInput.LastName,
                    FirstName = userInput.FirstName,
                    DateOfBirth = userInput.DateOfBirth
                };
                createdId = await this.adminService.CreateUserAsync(personDto);
            }

            return createdId > 0
                ? CreatedAtAction(nameof(GetAdminById), new { id = createdId }, createdId)
                : Problem("Failed to create user", statusCode: 400);
        }
        //TO DO => Il faut checher dans admin et person
        [HttpGet("{id:long}")]
        public async Task<ActionResult<AdminDto>> GetAdminById(long id)
        {
            var adminFound = await adminService.GetAdminByIdAsync(id);
            if (adminFound == null)
                return NotFound();

            return Ok(adminFound);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllAdmins()
        {
            var admins = await adminService.GetAllAdminsAsync();
            return Ok(admins);
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult> UpdateAsync(int id, [FromBody] PersonInput personInput)
        {
            if (id != personInput.Id)
                return BadRequest("Object id does not match.");

            var updatedObjectId = await this.adminService.UpdateAdminAsync(new PersonDto
            {
                Email = personInput.Email,
                HashedPassword = personInput.HashedPassword,
                LastName = personInput.LastName,
                FirstName = personInput.FirstName,
                DateOfBirth = personInput.DateOfBirth
            });

            return updatedObjectId > 0
                ? this.NoContent()
                : this.Problem();
        }

        [HttpDelete("{id:long}")]
        public async Task<ActionResult> DeleteAdmin(long id)
        {
            var isSuccess = await this.adminService.DeleteUserAsync(id);

            return isSuccess
                ? NoContent()
                : Problem();
        }


    }
}
