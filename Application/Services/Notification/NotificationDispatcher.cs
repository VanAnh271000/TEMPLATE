using Application.DTOs.Notification;
using Application.Interfaces.Services.Notification;

namespace Application.Services.Notification
{
    public class NotificationDispatcher : INotificationDispatcher
    {
        private readonly IEnumerable<INotificationSender> _senders;

        public NotificationDispatcher(IEnumerable<INotificationSender> senders)
        {
            _senders = senders;
        }

        public async Task DispatchAsync(NotificationMessage message, CancellationToken ct = default)
        {
            var sender = _senders.FirstOrDefault(x => x.Channel == message.Channel);

            if (sender is null)
                throw new InvalidOperationException($"No sender registered for channel {message.Channel}");

            await sender.SendAsync(message, ct);
        }
    }
}
