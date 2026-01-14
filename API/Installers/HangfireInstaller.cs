using Hangfire;
using Hangfire.Dashboard.BasicAuthorization;

namespace API.Installers
{
    public static class HangfireInstaller
    {
        public static IApplicationBuilder UseSecuredHangfireDashboard(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[]
                {
                    new BasicAuthAuthorizationFilter(
                        new BasicAuthAuthorizationFilterOptions
                        {
                            RequireSsl = false,
                            LoginCaseSensitive = true,
                            Users = new[]
                            {
                                new BasicAuthAuthorizationUser
                                {
                                    Login = "admin",
                                    PasswordClear = "admin123"
                                }
                            }
                        })
                }
            });

            return app;
        }
    }
}
