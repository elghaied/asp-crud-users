namespace AJCFinal.API.Models
{
    public class ChangePasswordInput
    {
        public long UserId { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
