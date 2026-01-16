using Application.DTOs.Notification;
using Domain.Enums;

namespace Application.Interfaces.Services.Notification
{
    public interface INotificationSender
    {
        NotificationChannel Channel { get; }
        Task SendAsync(NotificationMessage message);
    }
}
