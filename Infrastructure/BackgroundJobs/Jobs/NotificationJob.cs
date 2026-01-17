using Application.DTOs.Notification;
using Application.Interfaces.Services.Notification;
using Serilog.Context;

namespace Infrastructure.BackgroundJobs.Jobs
{
    public class NotificationJob
    {
        private readonly INotificationDispatcher _dispatcher;

        public NotificationJob(INotificationDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task ExecuteAsync(
        NotificationMessage message,
        string? correlationId = null)
        {
            if (!string.IsNullOrEmpty(correlationId))
            {
                using (LogContext.PushProperty("CorrelationId", correlationId))
                {
                    await _dispatcher.DispatchAsync(message);
                }
            }
            else
            {
                await _dispatcher.DispatchAsync(message);
            }
        }
    }
}
