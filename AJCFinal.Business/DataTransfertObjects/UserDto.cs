using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AJCFinal.Business.DataTransfertObjects
{
    public abstract class UserDto
    {
        public long Id { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required]
        [StringLength(255)]
        public string HashedPassword { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; } = DateTime.Now;
        public string? Address { get; set; } = null;
        public string? Phone { get; set; } = null;
        public string? Interests { get; set; } = null;
        public bool IsAdmin { get; set; } = false;

    }
}
