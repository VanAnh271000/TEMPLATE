using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Identity
{
    public class ChangePasswordRequest
    {
        [Required]
        public string OldPassword { get; set; } = null;

        [Required]
        public string Password { get; set; } = null;
    }
}
