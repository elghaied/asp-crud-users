using AJCFinal.API.Models;
using AJCFinal.Business.Abstractions;
using AJCFinal.Business.DataTransfertObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AJCFinal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IPersonService personService;

        public PersonController(IPersonService personService)
        {
            this.personService = personService;
        }


        [HttpGet("{id:long}")]
        public async Task<ActionResult> GetPersonById(long id)
        {
            var person = await this.personService.GetPersonByIdAsync(id);
            if (person == null)
                return NotFound();

            return Ok(person);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllPersons()
        {
            var persons = await personService.GetAllPersonsAsync();
            return Ok(persons);
        }

        [HttpPut("{id:long}")]
        public async Task<ActionResult> UpdatePerson(long id, [FromBody] PersonInput personInput)
        {
            if (id != personInput.Id)
                return BadRequest("Object id does not match.");

            var updatedObjectId = await this.personService.UpdatePersonAsync(new PersonDto
            {
                Id = id,
                Email = personInput.Email,
                //A changer
                HashedPassword = personInput.HashedPassword,
                LastName = personInput.LastName,
                FirstName = personInput.FirstName,
                DateOfBirth = personInput.DateOfBirth,
                Address = personInput.Address,
                Phone = personInput.Phone,
                Interests = personInput.Interests
            });

            return updatedObjectId > 0
                ? this.NoContent()
                : this.Problem();

        }

        [HttpPost("AddFriend")]
        public async Task<IActionResult> AddFriendAsync(long personId, long friendId)
        {
            var result = await this.personService.AddFriendAsync(personId, friendId);
            if (!result)
                return BadRequest("Failed to add friend.");

            return NoContent();
        }

        [HttpPost("RemoveFriend")]
        public async Task<IActionResult> RemoveFriendAsync(long personId, long friendId)
        {
            var result = await this.personService.RemoveFriendAsync(personId, friendId);
            if (!result)
                return BadRequest("Failed to remove friend.");

            return NoContent();
        }

        [HttpGet("{id}/friends")]
        public async Task<IActionResult> GetFriends(long id)
        {
            var friends = await this.personService.GetFriendsAsync(id);
            return Ok(friends);
        }
    }
}
