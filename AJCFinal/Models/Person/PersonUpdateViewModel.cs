using System.ComponentModel.DataAnnotations;

namespace AJCFinal.Models.Person
{
    public class PersonUpdateViewModel
    {

        [Required(ErrorMessage = "Email field is missing!!")]
        [EmailAddress(ErrorMessage = "Please provide an email ex: name@domain.ext")]
        [StringLength(255)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Last Name is missing !!!")]
        [Display(Name = "Last Name")]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "First Name is missing !!!")]
        [Display(Name = "First Name")]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Date of birth")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Profile Image")]
        public IFormFile? Image { get; set; } = null;

        public IEnumerable<long> FriendIds { get; set; } = new List<long>();
        public bool? IsAdmin { get; set; } = false;

        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Interests { get; set; }
    }
}
