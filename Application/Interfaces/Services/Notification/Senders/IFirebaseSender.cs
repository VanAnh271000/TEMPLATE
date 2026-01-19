using Application.DTOs.Notification;

namespace Application.Interfaces.Services.Notification.Senders
{
    public interface IFirebaseSender
    {
        Task SendAsync(FirebaseNotification notification);
    }
}
