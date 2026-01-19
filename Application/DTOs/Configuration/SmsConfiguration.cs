namespace Application.DTOs.Configuration
{
    public class SmsConfiguration
    {
        public string ApiUrl { get; init; } = default!;
        public string ApiKey { get; init; } = default!;
        public string SenderId { get; init; } = default!;
    }
}
