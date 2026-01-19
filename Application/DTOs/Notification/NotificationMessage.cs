namespace Application.DTOs.Notification
{
    public class NotificationMessage
    {
        public string Title { get; init; } = default!;
        public string Content { get; init; } = default!;

        public string? Email { get; init; }
        public string? PhoneNumber { get; init; }
        public string? FirebaseToken { get; init; }

        public IDictionary<string, string>? Metadata { get; init; }
    }
}
