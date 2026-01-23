namespace Application.DTOs.Commons
{
    public class ApiResponse<T>
    {
        public T? Data { get; set; }
        public string? Message { get; set; }
    }

    public class ErrorResponse
    {
        public string Code { get; set; } = string.Empty;
        public string? Message { get; set; }

        public string? TraceId { get; set; } = string.Empty;
    }

    public sealed class ValidationErrorResponse : ErrorResponse
    {
        public IDictionary<string, string[]> Errors { get; init; }
            = new Dictionary<string, string[]>();
    }

}
