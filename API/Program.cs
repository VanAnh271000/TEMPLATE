using API.Authorization;
using API.Extensions;
using API.Installers;
using Hangfire;
using Infrastructure;
namespace API {     
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.InstallServicesInAssembly(builder.Configuration);

            builder.Services.AddControllers();
            builder.Services.AddOpenApi();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "WholeSale");
                });
                app.UseDeveloperExceptionPage();
            }
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[]
                {
                    new HangfireAuthorizationFilter()
                }
            });
            app.UseSecuredHangfireDashboard();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
