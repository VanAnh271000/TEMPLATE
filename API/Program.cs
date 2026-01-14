using API.Installers;
using API.Middlewares;
using Infrastructure;
using Serilog;

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
            builder.Host.UseSerilog();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v2/swagger.json", "Template");
                });
                app.UseDeveloperExceptionPage();
            }
            app.UseMiddleware<CorrelationIdMiddleware>();
            
            app.MapHealthChecks("/health");

            app.MapPrometheusScrapingEndpoint().AllowAnonymous();

            app.UseSecuredHangfireDashboard();

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseSecuredHangfireDashboard();

            app.MapControllers();

            app.Run();
        }
    }
}
