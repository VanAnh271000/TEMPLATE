using Application.Interfaces.Commons;
using Application.Interfaces.Services.Hangfire;
using Application.Services.Commons;
using Application.Services.Identity;
using Infrastructure.BackgroundJobs.Jobs;
using Infrastructure.BackgroundJobs.Services;
using Infrastructure.Context.Queries;
using Infrastructure.Context.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace API.Installers
{
    public class ServiceInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            string? connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddMemoryCache();

            //Http services
            services.AddHttpClient();

            services.AddScoped<ICurrentUserService, CurrentUserService>();
            
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));

            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            services.AddScoped<IBackgroundJobService, BackgroundJobService>();

            services.AddScoped<IBackgroundTaskService, BackgroundTaskService>();

            services.AddTransient<NotificationJob>();

            services.AddHealthChecks();

            //Repository & service
            var appRepositories = typeof(PermissionRepository).Assembly.GetTypes()
                .Where(type => !type.IsAbstract && !type.IsInterface)
                .Where(r => r.Name.EndsWith("Repository")).ToList();
            appRepositories.ForEach(repository => services.AddScoped(repository.GetInterfaces()
                            .Where(r => r.Name.EndsWith("Repository")).FirstOrDefault(), repository));

            var appQueries = typeof(UserQuery).Assembly.GetTypes()
                .Where(type => !type.IsAbstract && !type.IsInterface)
                .Where(r => r.Name.EndsWith("Query")).ToList();
            appQueries.ForEach(query => services.AddScoped(query.GetInterfaces()
                            .Where(r => r.Name.EndsWith("Query")).FirstOrDefault(), query));

            var serviceTypes = typeof(AccountService).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface)
            .Where(type => type.Name.EndsWith("Service") &&
                          type.Name != "GenericService" &&
                          type.Name != "BaseService")
            .ToList();

            foreach (var serviceType in serviceTypes)
            {
                var interfaceType = serviceType.GetInterfaces()
                    .FirstOrDefault(i => i.Name == $"I{serviceType.Name}");

                if (interfaceType != null)
                {
                    services.AddScoped(interfaceType, serviceType);
                    Console.WriteLine($"Registered: {interfaceType.Name} -> {serviceType.Name}");
                }
            }
        }
    }
}
