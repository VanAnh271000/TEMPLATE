using Application.DTOs.Notification;
using Application.Interfaces.Services.Notification;
using Application.Interfaces.Services.Notification.Senders;

namespace Infrastructure.Notifications
{
    public class SmsNotificationChannel : INotificationChannel
    {
        private readonly ISmsSender _smsSender;

        public SmsNotificationChannel(ISmsSender smsSender)
        {
            _smsSender = smsSender;
        }

        public bool CanHandle(NotificationMessage message)
            => !string.IsNullOrWhiteSpace(message.PhoneNumber);

        public Task SendAsync(NotificationMessage message)
        {
            return _smsSender.SendAsync(new SmsNotification()
            {
                Message = message.Content,
                PhoneNumber = message.PhoneNumber!
            });
        }
    }
}
