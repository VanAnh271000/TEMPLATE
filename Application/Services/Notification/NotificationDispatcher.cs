using Application.DTOs.Notification;
using Application.Interfaces.Services.Notification;
using Serilog;

namespace Application.Services.Notification
{
    public class NotificationDispatcher : INotificationDispatcher
    {
        private readonly IEnumerable<INotificationChannel> _channels;

        public NotificationDispatcher(IEnumerable<INotificationChannel> channels)
        {
            _channels = channels;
        }

        public async Task DispatchAsync(NotificationMessage message, CancellationToken ct = default)
        {
            foreach (var channel in _channels)
            {
                if (!channel.CanHandle(message)) continue;

                Log.Information(
                    "Sending notification via {Channel}",
                    channel.GetType().Name);

                await channel.SendAsync(message);
            }
        }
    }
}
