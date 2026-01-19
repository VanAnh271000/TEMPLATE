using Application.DTOs.Notification;

namespace Application.Interfaces.Services.Notification.Senders
{
    public interface IEmailSender
    {
        Task SendAsync(EmailNotification email);
    }
}
