using AJCFinal.Business.Abstractions;
using AJCFinal.Business.DataTransfertObjects;
using AJCFinal.Business.Extensions;
using AJCFinal.DAL;
using AJCFinal.DAL.Entites;
using Microsoft.EntityFrameworkCore;

namespace AJCFinal.Business.Services
{
    internal sealed class PersonService : IPersonService
    {
        private readonly AjcFinalDbContext dbContext;
        //private readonly BlobStorageService blobStorageService;


        public PersonService(AjcFinalDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<PersonDto?> GetPersonByIdAsync(long id)
        {
            var person = await dbContext.Persons
                                         .Include(p => p.Friends)
                                         .FirstOrDefaultAsync(p => p.Id == id);
            return person?.ToDto();
        }

        public async Task<IEnumerable<PersonDto>> GetAllPersonsAsync()
        {
            return await dbContext.Persons
                                   .Include(p => p.Friends)
                                   .Select(p => p.ToDto())
                                   .ToListAsync();
        }
        public async Task<IEnumerable<PersonDto>> GetFriendsAsync(long personId)
        {
            var person = await dbContext.Persons
                                         .Include(p => p.Friends)
                                         .FirstOrDefaultAsync(p => p.Id == personId);
            if (person == null)
                return Enumerable.Empty<PersonDto>();
            var friends = person.Friends ?? Enumerable.Empty<Person>();
            var friendsToDo = friends.Select(f => f.ToDto()).ToList();
            return friendsToDo;
        }

        public async Task<long> UpdatePersonAsync(PersonDto personDto)
        {
            var foundPerson = await dbContext.Persons.FindAsync(personDto.Id);
            if (foundPerson is null) return -1;

            foundPerson.Email = personDto.Email;
            foundPerson.LastName = personDto.LastName;
            foundPerson.FirstName = personDto.FirstName;
            foundPerson.DateOfBirth = personDto.DateOfBirth;
            foundPerson.Address = personDto.Address;
            foundPerson.Phone = personDto.Phone;
            foundPerson.Interests = personDto.Interests;
            
            this.dbContext.Persons.Update(foundPerson);
            var numberOfOperationsInDatabase = await this.dbContext.SaveChangesAsync();

            return numberOfOperationsInDatabase > 0 ? foundPerson.Id : -1;
        }

        public async Task<bool> DeletePersonAsync(long id)
        {
            var foundPerson = await this.dbContext.Persons.FindAsync(id);
            if (foundPerson is null) return false;

            dbContext.Persons.Remove(foundPerson);
            var numberOfOperationsInDatabase = await this.dbContext.SaveChangesAsync();

            return numberOfOperationsInDatabase > 0;
        }

        public async Task<bool> AddFriendAsync(long personId, long friendId)
        {
            var person = await dbContext.Persons
                .Include(p => p.Friends)
                .FirstOrDefaultAsync(p => p.Id == personId);

            var friend = await dbContext.Persons
                .Include(p => p.Friends)
                .FirstOrDefaultAsync(p => p.Id == friendId);

            if (person == null || friend == null)
                return false;

            if (person.Friends.Any(f => f.Id == friendId) || friend.Friends.Any(f => f.Id == personId))
                return false;

            person.Friends.Add(friend);

            friend.Friends.Add(person);

            var numberOfOperationsInDatabase = await dbContext.SaveChangesAsync();
            return numberOfOperationsInDatabase > 0;
        }
        public async Task<bool> RemoveFriendAsync(long personId, long friendId)
        {
            var person = await dbContext.Persons
                .Include(p => p.Friends)
                .FirstOrDefaultAsync(p => p.Id == personId);

            var friend = await dbContext.Persons
                .Include(p => p.Friends)
                .FirstOrDefaultAsync(p => p.Id == friendId);

            if (person == null || friend == null)
                return false;

            if (!person.Friends.Any(f => f.Id == friendId) && !friend.Friends.Any(f => f.Id == personId))
                return false;

            person.Friends = person.Friends.Where(f => f.Id != friendId).ToList();
            friend.Friends = friend.Friends.Where(f => f.Id != personId).ToList();

            var numberOfOperationsInDatabase = await dbContext.SaveChangesAsync();
            return numberOfOperationsInDatabase > 0;
        }

     

    }

}
