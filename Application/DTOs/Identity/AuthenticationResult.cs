namespace Application.DTOs.Identity
{
    public class AuthenticationResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public bool TwoFactorEnabled { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
}
