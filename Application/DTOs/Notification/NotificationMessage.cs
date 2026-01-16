namespace Application.DTOs.Notification
{
    public class NotificationMessage
    {
        public string RecipientId { get; init; } = default!;
        public NotificationChannel Channel { get; init; }
        public string Subject { get; init; } = default!;
        public string Content { get; init; } = default!;
        public Dictionary<string, string>? Metadata { get; init; }
    }
}
