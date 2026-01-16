using Application.DTOs.Notification;
using Application.Interfaces.Services.Notification;

namespace Application.Services.Notification
{
    public class NotificationSender : INotificationSender
    {
        public NotificationChannel Channel => throw new NotImplementedException();

        public Task SendAsync(NotificationMessage message, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
