namespace Application.DTOs.Identity
{
    public class CreateAccountDto
    {
        public string UserName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public List<string>? RoleIds { get; set; }
    }

    public class UpdateProfileDto
    {
        public string FullName { get; set; }
        public string? Email { get; set; }
        public bool TwoFactorEnabled { get; set; }
    }
}
