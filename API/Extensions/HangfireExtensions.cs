using API.Authorization;
using Hangfire;

namespace API.Extensions
{
    public static class HangfireExtensions
    {
        public static IApplicationBuilder UseSecuredHangfireDashboard(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[]
                {
                new HangfireAuthorizationFilter()
                }
            });

            return app;
        }
    }

}
