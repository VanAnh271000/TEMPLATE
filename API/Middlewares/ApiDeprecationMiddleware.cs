using Infrastructure.Versioning;

namespace API.Middlewares
{
    public class ApiDeprecationMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiDeprecationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var apiVersion = context.GetRequestedApiVersion();

            if (apiVersion != null &&
                ApiVersionPolicy.DeprecatedMajorVersions.Contains(
                    apiVersion.MajorVersion ?? 0))
            {
                context.Response.Headers.TryAdd(
                    "Deprecation", "true");

                context.Response.Headers.TryAdd(
                    "Sunset",
                    ApiVersionPolicy.SunsetDateV1.ToString("R"));
            }

            await _next(context);
        }
    }
}
