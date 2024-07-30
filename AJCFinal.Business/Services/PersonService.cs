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

            return person.Friends.Select(f => f.ToDto()).ToList();
        }

        public async Task<long> UpdatePersonAsync(PersonDto personDto)
        {
            var foundPerson = await dbContext.Persons.FindAsync(personDto.Id);
            if (foundPerson is null) return -1;

            foundPerson.Email = personDto.Email;
            foundPerson.HashedPassword = personDto.HashedPassword;
            foundPerson.LastName = personDto.LastName;
            foundPerson.FirstName = personDto.FirstName;
            foundPerson.DateOfBirth = personDto.DateOfBirth;

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
            var foundPerson= await dbContext.Persons
                                         .Include(p => p.Friends)
                                         .FirstOrDefaultAsync(p => p.Id == personId);
            var friend = await this.dbContext.Persons.FindAsync(friendId);

            if (foundPerson == null || friend == null || foundPerson.Friends.Any(f => f.Id == friendId))
                return false;

            foundPerson.Friends.Add(friend);
            var numberOfOperationsInDatabase = await this.dbContext.SaveChangesAsync();

            return numberOfOperationsInDatabase > 0;
        }

       
    }

}
