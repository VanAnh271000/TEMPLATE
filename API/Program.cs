using API.Installers;
using API.Middlewares;
using Asp.Versioning;
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
            
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            builder.Services.AddApiVersioning().AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });



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
