using System.ComponentModel.DataAnnotations;
namespace Application.DTOs.Identity
{
    public class LoginDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
    }
    public class LoginOtpDto
    {
        [Required(ErrorMessage = "Code cannot be empty")]
        public string Code { get; set; }
        [Required(ErrorMessage = "User name cannot be empty")]
        public string UserName { get; set; }
        public string IpAddress { get; set; } = string.Empty;
    }
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
    }
}
