namespace Application.DTOs.Notification
{
    public class FirebaseNotification
    {
        public string DeviceToken { get; init; } = default!;
        public string Title { get; init; } = default!;
        public string Body { get; init; } = default!;
        public Dictionary<string, string>? Data { get; init; }
    }
}
