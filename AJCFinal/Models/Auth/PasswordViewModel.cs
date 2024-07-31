using System.ComponentModel.DataAnnotations;

namespace AJCFinal.Models.Auth
{
    public class PasswordViewModel
    {
        [Required]
        [StringLength(255)]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(255)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
    }
}
