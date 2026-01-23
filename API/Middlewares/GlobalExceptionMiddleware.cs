using Application.DTOs.Commons;
using Serilog;

namespace API.Middlewares
{
    public sealed class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception. TraceId: {TraceId}", context.TraceIdentifier);

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = context.TraceIdentifier;

            Log.Error(
                exception,
                "Unhandled exception occurred. TraceId: {TraceId}",
                traceId);

            var (statusCode, errorCode, message) = MapException(exception);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var response = new ErrorResponse
            {
                Code = errorCode,
                Message = message,
                TraceId = traceId
            };

            await context.Response.WriteAsJsonAsync(response);
        }

        private static (int StatusCode, string Code, string Message) MapException(Exception exception)
        {
            return exception switch
            {
                TaskCanceledException =>
                    (StatusCodes.Status408RequestTimeout,
                     "REQUEST_TIMEOUT",
                     "The request timed out"),

                UnauthorizedAccessException =>
                    (StatusCodes.Status401Unauthorized,
                     "UNAUTHORIZED",
                     "Unauthorized access"),

                _ =>
                    (StatusCodes.Status500InternalServerError,
                     "INTERNAL_SERVER_ERROR",
                     "An unexpected error occurred")
            };
        }
    }

}
