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

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = exception switch
            {
                AppNotFoundException => CreateResponse(404, "NOT_FOUND", exception, context),
                AppConflictException => CreateResponse(409, "CONFLICT", exception, context),
                AppValidationException vex => CreateValidationResponse(vex, context),
                _ => CreateResponse(500, "INTERNAL_ERROR", exception, context)
            };

            context.Response.StatusCode = response.StatusCode;
            await context.Response.WriteAsJsonAsync(response.Body);
        }

        private static (int StatusCode, object Body) CreateResponse(
            int statusCode,
            string code,
            Exception ex,
            HttpContext ctx)
        {
            return (statusCode, new ErrorResponse
            {
                Code = code,
                Message = ex.Message,
                TraceId = ctx.TraceIdentifier
            });
        }

        private static (int StatusCode, object Body) CreateValidationResponse(AppValidationException ex, HttpContext ctx)
        {
            return (422, new ValidationErrorResponse
            {
                Code = "VALIDATION_ERROR",
                Message = ex.Message,
                Errors = ex.Errors,
                TraceId = ctx.TraceIdentifier
            });
        }
    }

}
