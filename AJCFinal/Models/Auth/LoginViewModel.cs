using System.ComponentModel.DataAnnotations;

namespace AJCFinal.Models.Auth
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [StringLength(255)]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }
    }
}
