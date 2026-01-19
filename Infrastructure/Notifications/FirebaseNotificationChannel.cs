using Application.DTOs.Notification;
using Application.Interfaces.Services.Notification;
using Application.Interfaces.Services.Notification.Senders;

namespace Infrastructure.Notifications
{
    public class FirebaseNotificationChannel : INotificationChannel
    {
        private readonly IFirebaseSender _firebaseSender;

        public FirebaseNotificationChannel(IFirebaseSender firebaseSender)
        {
            _firebaseSender = firebaseSender;
        }

        public bool CanHandle(NotificationMessage message)
            => message.FirebaseToken != null;

        public async Task SendAsync(NotificationMessage message)
        {
            await _firebaseSender.SendAsync(new FirebaseNotification()
            {
                Body = message.Content,
                Title = message.Title,
                DeviceToken = message.FirebaseToken!,
                Data = message.Metadata != null
                    ? new Dictionary<string, string>(message.Metadata)
                    : null
            });
        }
    }
}
