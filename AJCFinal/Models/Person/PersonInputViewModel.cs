using System.ComponentModel.DataAnnotations;

namespace AJCFinal.Models.Person
{
    public class PersonInputViewModel
    {
        [Required(ErrorMessage = "Email field is missing!!")]
        [EmailAddress(ErrorMessage = "Please provide an email ex: name@domain.ext")]
        [StringLength(255)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is missing !!")]
        [StringLength(255)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Last Name is missing !!!")]
        [Display(Name ="Last Name")]
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

        [Display(Name ="Profile Image")]
        public IFormFile? Image { get; set; }
    }
}
