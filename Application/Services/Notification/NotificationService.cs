using Application.DTOs.Notification;
using Application.Interfaces.Services.Hangfire;
using Application.Interfaces.Services.Notification;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IBackgroundJobService _jobService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public NotificationService(IBackgroundJobService jobService,
            IHttpContextAccessor httpContextAccessor)
        {
            _jobService = jobService;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task SendAsync(NotificationMessage message)
        {
            var correlationId =
            _httpContextAccessor.HttpContext?
                .Items["X-Correlation-Id"]?.ToString();

            _jobService.Enqueue<IBackgroundTaskService>(job => job.SendNotificationAsync(message, correlationId));
            return Task.CompletedTask;
        }
    }
}
