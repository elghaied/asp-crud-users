using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJCFinal.Business.DataTransfertObjects;

namespace AJCFinal.Business.Abstractions
{
    public interface IAuthService
    {
        Task<UserDto?> AuthenticateAsync(string email, string password);
        Task<bool> RegisterAsync(UserDto userDto);
        Task<bool> ChangePasswordAsync(long userId, string oldPassword, string newPassword);
    }
}
