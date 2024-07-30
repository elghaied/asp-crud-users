using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJCFinal.Business.DataTransfertObjects;
using AJCFinal.DAL.Entites;

namespace AJCFinal.Business.Extensions
{
    public static class MappingExtensionsAdmin
    {
        public static AdminDto ToDto(this Admin admin)
        {
            return new AdminDto
            {
                Id = admin.Id,
                Email = admin.Email,
                HashedPassword = admin.HashedPassword,
                LastName = admin.LastName,
                FirstName = admin.FirstName,
                DateOfBirth = admin.DateOfBirth,
                Address = admin.Address,
                Phone = admin.Phone,
                Interests = admin.Interests
            };
        }
    }
}
