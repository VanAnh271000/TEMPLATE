using Application.DTOs.Notification;

namespace Application.Interfaces.Services.Notification
{
    public interface INotificationService
    {
        Task SendAsync(NotificationMessage message);
    }
}
