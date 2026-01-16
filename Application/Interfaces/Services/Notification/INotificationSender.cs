using Application.DTOs.Notification;
namespace Application.Interfaces.Services.Notification
{
    public interface INotificationSender
    {
        NotificationChannel Channel { get; }
        Task SendAsync(NotificationMessage message, CancellationToken ct = default);
    }
}
