using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identity
{
    public class ChangePasswordDto
    {
        [Required]
        public string OldPassword { get; set; } = null;

        [Required]
        public string Password { get; set; } = null;
    }
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Password cannot be empty")]
        public string Password { get; set; }
        [Required(ErrorMessage = "User name cannot be empty")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Token cannot be empty")]
        public string Token { get; set; }
    }
}
