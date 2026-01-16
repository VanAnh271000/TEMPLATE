using Application.DTOs.Notification;
using Application.Interfaces.Services.Notification;

namespace Infrastructure.BackgroundJobs.Jobs
{
    public class NotificationJob
    {
        private readonly INotificationDispatcher _dispatcher;

        public NotificationJob(INotificationDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task ExecuteAsync(NotificationMessage message)
        {
            await _dispatcher.DispatchAsync(message);
        }
    }
}
