using Application.DTOs.Notification;

namespace Application.Interfaces.Services.Notification
{
    public interface INotificationChannel
    {
        bool CanHandle(NotificationMessage message);
        Task SendAsync(NotificationMessage message);
    }
}
