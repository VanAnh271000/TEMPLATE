using Application.DTOs.Identity;
using Application.DTOs.Notification;

namespace Application.Interfaces.Services.Hangfire
{
    public interface IBackgroundTaskService
    {
        Task SendEmailAsync(EmailMessage message);
        Task SendNotificationAsync(NotificationMessage message, string correlationId);
    }
}
