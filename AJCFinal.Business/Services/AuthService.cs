using System.Security.Cryptography;
using AJCFinal.Business.Abstractions;
using AJCFinal.Business.DataTransfertObjects;
using AJCFinal.DAL;
using AJCFinal.DAL.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace AJCFinal.Business.Services
{
    internal sealed class AuthService : IAuthService
    {
        private readonly AjcFinalDbContext dbContext;

        public AuthService(AjcFinalDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<UserDto?> AuthenticateAsync(string email, string password)
        {
            var admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Email == email);
            if (admin != null && VerifyPassword(password, admin.HashedPassword))
            {
                return new AdminDto
                {
                    Id = admin.Id,
                    Email = admin.Email,
                    FirstName = admin.FirstName,
                    LastName = admin.LastName,
                    DateOfBirth = admin.DateOfBirth,
                    IsAdmin = true
                };
            }

            var person = await dbContext.Persons.FirstOrDefaultAsync(p => p.Email == email);
            if (person != null && VerifyPassword(password, person.HashedPassword))
            {
                return new PersonDto
                {
                    Id = person.Id,
                    Email = person.Email,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    DateOfBirth = person.DateOfBirth,
                    IsAdmin = false
                };
            }

            return null;
        }

        public async Task<bool> RegisterAsync(UserDto userDto)
        {
            if (await dbContext.Admins.AnyAsync(a => a.Email == userDto.Email) ||
                await dbContext.Persons.AnyAsync(p => p.Email == userDto.Email))
            {
                return false;
            }

            var hashedPassword = HashPassword(userDto.HashedPassword);

            if (userDto.IsAdmin)
            {
                var admin = new Admin
                {
                    Email = userDto.Email,
                    HashedPassword = hashedPassword,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    DateOfBirth = userDto.DateOfBirth
                };
                dbContext.Admins.Add(admin);
            }
            else
            {
                var person = new Person
                {
                    Email = userDto.Email,
                    HashedPassword = hashedPassword,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    DateOfBirth = userDto.DateOfBirth
                };
                dbContext.Persons.Add(person);
            }

            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> ChangePasswordAsync(long userId, string oldPassword, string newPassword)
        {
            var admin = await dbContext.Admins.FindAsync(userId);
            if (admin != null && VerifyPassword(oldPassword, admin.HashedPassword))
            {
                admin.HashedPassword = HashPassword(newPassword);
                dbContext.Admins.Update(admin);
                return await dbContext.SaveChangesAsync() > 0;
            }

            var person = await dbContext.Persons.FindAsync(userId);
            if (person != null && VerifyPassword(oldPassword, person.HashedPassword))
            {
                person.HashedPassword = HashPassword(newPassword);
                dbContext.Persons.Update(person);
                return await dbContext.SaveChangesAsync() > 0;
            }

            return false;
        }

        private string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return $"{Convert.ToBase64String(salt)}:{hashed}";
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(':');
            if (parts.Length != 2)
                return false;

            var salt = Convert.FromBase64String(parts[0]);
            var hash = parts[1];

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hash == hashed;
        }
    }
}