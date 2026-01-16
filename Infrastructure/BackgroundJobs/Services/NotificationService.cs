using Application.DTOs.Notification;
using Application.Interfaces.Services.Hangfire;
using Application.Interfaces.Services.Notification;
using Infrastructure.BackgroundJobs.Jobs;

namespace Infrastructure.BackgroundJobs.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IBackgroundJobService _jobService;

        public NotificationService(IBackgroundJobService jobService)
        {
            _jobService = jobService;
        }

        public Task SendAsync(NotificationMessage message)
        {
            _jobService.Enqueue<NotificationJob>(job => job.ExecuteAsync(message));
            return Task.CompletedTask;
        }
    }
}
