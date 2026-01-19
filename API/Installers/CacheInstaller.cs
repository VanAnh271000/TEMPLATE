using Application.Interfaces.Services.Caching;
using Infrastructure.Caching;

namespace API.Installers
{
    public class CacheInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();
            services.AddScoped<ICacheService, CacheService>();
        }
    }
}
