using Application.DTOs.Notification;

namespace Application.Interfaces.Services.Notification.Senders
{
    public interface ISmsSender
    {
        Task SendAsync(SmsNotification message);
    }
}
