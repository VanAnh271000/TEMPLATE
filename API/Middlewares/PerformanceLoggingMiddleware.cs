using Serilog;
using System.Diagnostics;

namespace API.Middlewares
{
    public class PerformanceLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        public async Task Invoke(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            await _next(context);
            sw.Stop();

            if (sw.ElapsedMilliseconds > 500)
            {
                Log.Warning(
                    "Slow HTTP request {Path} took {Elapsed}ms",
                    context.Request.Path,
                    sw.ElapsedMilliseconds);
            }
        }
    }

}
