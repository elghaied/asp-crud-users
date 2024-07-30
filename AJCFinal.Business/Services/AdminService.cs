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
        public async Task<long> UpdateAdminAsync(AdminDto adminDto)
        {
            var adminFound = await this.dbContext.Admins.FindAsync(adminDto.Id);
            if (adminFound is null)
                return -1;

            adminFound.Email = adminDto.Email;
            adminFound.HashedPassword = adminDto.HashedPassword;
            adminFound.LastName = adminDto.LastName;
            adminFound.FirstName = adminDto.FirstName;
            adminFound.DateOfBirth = adminDto.DateOfBirth;

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

        //private void HashedPassWord(string passWord)
        //{
        //    byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); // divide by 8 to convert bits to bytes
        //    Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

        //    // derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
        //    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        //        password: passWord!,
        //        salt: salt,
        //        prf: KeyDerivationPrf.HMACSHA256,
        //        iterationCount: 100000,
        //        numBytesRequested: 256 / 8));
        //}

      
    }
}
