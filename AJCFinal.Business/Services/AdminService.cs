using System.Security.Cryptography;
using AJCFinal.Business.Abstractions;
using AJCFinal.Business.DataTransfertObjects;
using AJCFinal.Business.Extensions;
using AJCFinal.DAL;
using AJCFinal.DAL.Entites;
using Microsoft.EntityFrameworkCore;


namespace AJCFinal.Business.Services
{
    internal sealed class AdminService : IAdminService
    {
        private readonly AjcFinalDbContext dbContext;

        public AdminService(AjcFinalDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<AdminDto?> GetAdminByIdAsync(long id)
        {
            var adminFound = await this.dbContext.Admins.FindAsync(id);
            return adminFound?.ToDto();
        }
        public async Task<IEnumerable<AdminDto>> GetAllAdminsAsync()
        {
            return await this.dbContext.Admins.Select(x => x.ToDto()).ToListAsync();

        }
        public async Task<long> UpdateAdminAsync(PersonDto personDto)
        {
            var adminFound = await this.dbContext.Admins.FindAsync(personDto.Id);
            if (adminFound is null)
                return -1;

            adminFound.Email = personDto.Email;
            adminFound.HashedPassword = personDto.HashedPassword;
            adminFound.LastName = personDto.LastName;
            adminFound.FirstName = personDto.FirstName;
            adminFound.DateOfBirth = personDto.DateOfBirth;
            adminFound.Address = personDto.Address;
            adminFound.Phone = personDto.Phone;
            adminFound.Interests = personDto.Interests;
          
            this.dbContext.Admins.Update(adminFound);
            var numberOfOperationsInDatabase = await this.dbContext.SaveChangesAsync();

            return numberOfOperationsInDatabase > 0 ? adminFound.Id : -1;
        }
        public async Task<bool> DeleteUserAsync(long id)
        {
            var userAdmin = await this.dbContext.Admins.FindAsync(id);
            if (userAdmin == null)
            {
                return false;
            }
            else
            {
                this.dbContext.Admins.Remove(userAdmin);
                return await this.dbContext.SaveChangesAsync() > 0;
            }
            
            var userPerson = await this.dbContext.Persons.FindAsync(id);
            if (userPerson == null)
            {
                return false;
            }
            else
            {
                this.dbContext.Persons.Remove(userPerson);
                return await this.dbContext.SaveChangesAsync() > 0;
            }
            return true;
        }
        public Task<bool> AdminExistsAsync(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<long> CreateUserAsync(UserDto userDto)
        {

            if (dbContext.Admins.Any(a => a.Email == userDto.Email) ||
                dbContext.Persons.Any(p => p.Email == userDto.Email))
            {
                return -1; 
            }
            if (userDto.IsAdmin)
            {
                var adminToCreate = new Admin
                {
                    Email = userDto.Email,
                    HashedPassword = userDto.HashedPassword,
                    LastName = userDto.LastName,
                    FirstName = userDto.FirstName,
                    DateOfBirth = userDto.DateOfBirth
                };

                this.dbContext.Admins.Add(adminToCreate);
                await dbContext.SaveChangesAsync();

                return adminToCreate.Id;
            }
            else 
            {
                var personToCreate = new Person
                {
                    Email = userDto.Email,
                    HashedPassword = userDto.HashedPassword,
                    LastName = userDto.LastName,
                    FirstName = userDto.FirstName,
                    DateOfBirth = userDto.DateOfBirth
                };

                dbContext.Persons.Add(personToCreate);
                await dbContext.SaveChangesAsync();

                return personToCreate.Id;
            }

            return -1; 
        }

     
    }
}
