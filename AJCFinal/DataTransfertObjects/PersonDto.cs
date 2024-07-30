using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AJCFinal.Models.Person;

namespace AJCFinal.DataTransfertObjects
{
    public class PersonDto
    {
        public long Id { get; set; }

   
        public string Email { get; set; }

   
        public string HashedPassword { get; set; }

 
        public string LastName { get; set; }

  
        public string FirstName { get; set; }

 
        public DateTime DateOfBirth { get; set; }

        public List<PersonBaseViewModel> Friends { get; set; } = new List<PersonBaseViewModel>();

        public IEnumerable<long>? FriendIds { get; set; } = new List<long>();
    }
}
