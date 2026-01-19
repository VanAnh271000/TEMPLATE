using Application.DTOs.Notification;
using Application.Interfaces.Services.Notification;
using Application.Interfaces.Services.Notification.Senders;
using Serilog;

namespace Infrastructure.Notifications
{
    public class EmailNotificationChannel : INotificationChannel
    {
        private readonly IEmailSender _emailSender;

        public EmailNotificationChannel(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public bool CanHandle(NotificationMessage message)
            => !string.IsNullOrWhiteSpace(message.Email);

        public async Task SendAsync(NotificationMessage message)
        {
            Log.Information("Sending email to {Email}", message.Email);

            await _emailSender.SendAsync(new EmailNotification(new List<string>{ message.Email! }, message.Title, message.Content));
        }
    }
}
