using Application.DTOs.Commons;
using Serilog.Context;

namespace API.Middlewares
{
    public class CorrelationIdMiddleware
    {
        private const string Header = "X-Correlation-Id";
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var correlationId = context.Request.Headers.ContainsKey(Header)
                ? context.Request.Headers[Header].ToString()
                : Guid.NewGuid().ToString();

            context.Items[Header] = correlationId;
            context.Response.Headers[Header] = correlationId;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                NotificationMetrics.SendTotal.Add(1);
                await _next(context);
            }
        }
    }

}
