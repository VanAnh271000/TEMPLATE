using Application.DTOs.Identity;
using Application.DTOs.Notification;
using Application.Interfaces.Services.Hangfire;
using Infrastructure.BackgroundJobs.Jobs;

namespace Infrastructure.BackgroundJobs.Services
{
    public class BackgroundTaskService : IBackgroundTaskService
    {
        private readonly IBackgroundJobService _backgroundJobService;
        public BackgroundTaskService(IBackgroundJobService backgroundJobService)
        {
            _backgroundJobService = backgroundJobService;
        }
        public Task SendEmailAsync(EmailMessage message)
        {
            _backgroundJobService.Enqueue<EmailJob>(x => x.SendJob(message));
            return Task.CompletedTask;
        }

        public Task SendNotificationAsync(NotificationMessage message, string correlationId)
        {
            _backgroundJobService.Enqueue<NotificationJob>(x => x.ExecuteAsync(message, correlationId));
            return Task.CompletedTask;
        }
    }
}
