using API.Installers;
using API.Middlewares;
using Asp.Versioning.ApiExplorer;
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

                app.UseSwaggerUI(options =>
                {
                    var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                    foreach (var description in provider.ApiVersionDescriptions)
                    {
                        options.SwaggerEndpoint(
                            $"/swagger/{description.GroupName}/swagger.json",
                            description.GroupName.ToUpperInvariant());
                    }

                    options.DisplayRequestDuration();
                });

                app.UseDeveloperExceptionPage();
            }


            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<ApiDeprecationMiddleware>();


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
