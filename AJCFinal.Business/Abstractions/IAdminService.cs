using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJCFinal.Business.DataTransfertObjects;

namespace AJCFinal.Business.Abstractions
{
    public interface IAdminService
    {
        Task<AdminDto?> GetAdminByIdAsync(long id);
        Task<IEnumerable<AdminDto>> GetAllAdminsAsync();
        Task<long> UpdateAdminAsync(PersonDto personDto);
        //Task<bool> DeleteAdminAsync(long id);
        //Task<bool> AdminExistsAsync(string email);
        //Task<long> CreateAsync(AdminDto adminDto);
        Task<long> CreateUserAsync(UserDto userDto);
        Task<bool> DeleteUserAsync(long userDto);


    }
}
