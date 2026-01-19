namespace Application.DTOs.Configuration
{
    public class EmailConfiguration
    {
        public string From { get; set; } = null;
        public string SmtpServer { get; set; } = null;
        public string Password { get; set; } = null;
        public string UserName { get; set; } = null;
        public int Port { get; set; }
        public string ApplicationName { get; set; } = null;
        public string ConfirmEmailPath { get; set; } = null;
        public string ResetPasswordPath { get; set; } = null;
    }
}
