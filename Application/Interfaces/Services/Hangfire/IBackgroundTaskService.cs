using Application.DTOs.Identity;

namespace Application.Interfaces.Services.Hangfire
{
    public interface IBackgroundTaskService
    {
        Task SendEmailAsync(EmailMessage message);
    }
}
