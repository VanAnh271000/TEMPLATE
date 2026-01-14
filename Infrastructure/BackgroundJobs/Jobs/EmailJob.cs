using Application.DTOs.Identity;
using Application.Interfaces.Services.Identity;

namespace Infrastructure.BackgroundJobs.Jobs
{
    public class EmailJob
    {
        private readonly IEmailService _emailService;
        public EmailJob(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public void SendJob(EmailMessage message)
        {
            _emailService.SendEmail(message);
        }
    }
}
