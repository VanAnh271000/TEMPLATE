using Application.DTOs.Notification;

namespace Application.Interfaces.Services.Hangfire
{
    public interface IBackgroundTaskService
    {
        Task SendNotificationAsync(NotificationMessage message, string correlationId);
    }
}
