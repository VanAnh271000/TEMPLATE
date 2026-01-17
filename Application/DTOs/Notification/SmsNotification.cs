namespace Application.DTOs.Notification
{
    public class SmsNotification
    {
        public string PhoneNumber { get; init; } = default!;
        public string Message { get; init; } = default!;
    }
}
