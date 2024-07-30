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

        //[HttpPut]
        //public async Task<ActionResult> UpdatePerson([FromBody] UserInput userInput)
        //{
        //    //if (userInput is AdminInput adminInput)
            //{
            //    var adminToUpdate = await this.personService.Find(userInput.Id);
            //    if (adminToUpdate == null)
            //        return -1;

            //    if (dbContext.Admins.Any(a => a.Email == adminInput.Email && a.Id != adminInput.Id))
            //        return -1;

            //    adminToUpdate.Email = adminInput.Email;
            //    adminToUpdate.HashedPassword = adminInput.HashedPassword;
            //    adminToUpdate.LastName = adminInput.LastName;
            //    adminToUpdate.FirstName = adminInput.FirstName;
            //    adminToUpdate.DateOfBirth = adminInput.DateOfBirth;

            //    dbContext.Admins.Update(adminToUpdate);
            //    await dbContext.SaveChangesAsync();

            //    return adminToUpdate.Id;
            //}
            //else if (userInput is PersonInput personInput)
            //{
            //    var personToUpdate = await dbContext.Persons.Include(p => p.Friends).FirstOrDefaultAsync(p => p.Id == personInput.Id);
            //    if (personToUpdate == null)
            //        return -1;

            //    if (dbContext.Persons.Any(p => p.Email == personInput.Email && p.Id != personInput.Id))
            //        return -1;

            //    personToUpdate.Email = personInput.Email;
            //    personToUpdate.HashedPassword = personInput.HashedPassword;
            //    personToUpdate.LastName = personInput.LastName;
            //    personToUpdate.FirstName = personInput.FirstName;
            //    personToUpdate.DateOfBirth = personInput.DateOfBirth;

            //    // Update friends if needed
            //    if (personInput.FriendsIds != null)
            //    {
            //        var friendsToUpdate = await dbContext.Persons.Where(p => personInput.FriendsIds.Contains(p.Id)).ToListAsync();
            //        personToUpdate.Friends = friendsToUpdate;
            //    }

            //    dbContext.Persons.Update(personToUpdate);
            //    await dbContext.SaveChangesAsync();

            //    return personToUpdate.Id;
            //}

        //    return -1;
        //}

        [HttpPost("AddFriend")]
        public async Task<IActionResult> AddFriendAsync (long personId, long friendId)
        {
            var result = await this.personService.AddFriendAsync(personId, friendId);
            if (!result)
                return BadRequest("Failed to add friend.");

            return NoContent();
        }
    }
}
