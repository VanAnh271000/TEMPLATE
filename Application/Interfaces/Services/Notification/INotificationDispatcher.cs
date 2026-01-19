using Application.DTOs.Notification;

namespace Application.Interfaces.Services.Notification
{
    public interface INotificationDispatcher
    {
        Task DispatchAsync(NotificationMessage message, CancellationToken ct = default);
    }
}
