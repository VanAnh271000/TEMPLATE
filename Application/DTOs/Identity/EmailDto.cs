using MimeKit;

namespace Application.DTOs.Identity
{
    public class EmailMessage
    {
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public EmailMessage(IEnumerable<string> to, string subject, string content)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress("email", x)));
            Subject = subject;
            Content = content;
        }
    }
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
