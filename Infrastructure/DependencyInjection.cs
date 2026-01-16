using Application.DTOs.Commons;
using Application.Interfaces.Commons;
using Hangfire;
using Infrastructure.Context;
using Infrastructure.Context.Factories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OpenTelemetry.Metrics;
using Serilog;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            //DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            //Hangfire
            services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(connectionString);
            });

            services.AddHangfireServer();

            //Logging
            Log.Logger = new LoggerConfiguration()
           .ReadFrom.Configuration(configuration)
           .Enrich.FromLogContext()
           .CreateLogger();

            //Health Checks
            services.AddHealthChecks().AddCheck("self", () => HealthCheckResult.Healthy());

            //Metrics
            services.AddOpenTelemetry()
            .WithMetrics(m =>
            {
                m.AddAspNetCoreInstrumentation();
                m.AddRuntimeInstrumentation();
                m.AddProcessInstrumentation();
                m.AddMeter(AppMetrics.MeterName);
                m.AddPrometheusExporter();
            });


            return services;
        }
    }
}
