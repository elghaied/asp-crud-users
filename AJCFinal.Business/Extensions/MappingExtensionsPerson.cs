using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AJCFinal.Business.DataTransfertObjects;
using AJCFinal.DAL.Entites;

namespace AJCFinal.Business.Extensions
{
    public static class MappingExtensionsPerson
    {
        public static PersonDto ToDto(this Person person)
        {
            return new PersonDto
            {
                Id = person.Id,
                Email = person.Email,
                HashedPassword = person.HashedPassword,
                LastName = person.LastName,
                FirstName = person.FirstName,
                DateOfBirth = person.DateOfBirth
            };
        }
    }
}
