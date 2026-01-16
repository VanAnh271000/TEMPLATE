using Application.DTOs.Notification;
using Application.Interfaces.Services.Notification;
using Serilog;

namespace Application.Services.Notification
{
    public class NotificationSender : INotificationSender
    {
        public NotificationChannel Channel => NotificationChannel.Email;

        public Task SendAsync(NotificationMessage message, CancellationToken ct = default)
        {
            Log.Information(
                "Sending email to {RecipientId}. Subject={Subject}",
                message.RecipientId,
                message.Subject);

            // TODO: integrate SMTP / provider

            return Task.CompletedTask;
        }
    }
}
