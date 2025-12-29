using Application.DTOs.Identity;

namespace Application.Interfaces.Services.Identity
{
    public interface IEmailService
    {
        void SendEmail(EmailMessage message);
    }
}
