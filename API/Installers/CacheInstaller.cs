using Application.DTOs.Configuration;
using Application.Interfaces.Services.Caching;
using Infrastructure.Caching;

namespace API.Installers
{
    public class CacheInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddMemoryCache();

            var redisConfig = configuration.GetSection("Redis").Get<RedisConfiguration>();
            services.AddSingleton(redisConfig);

            services.AddSingleton(new RedisConnection(redisConfig));

            if (!string.IsNullOrEmpty(redisConfig.ConnectionString))
            {
                services.AddScoped<ICacheService, RedisCacheService>();
            }
            else
            {
                services.AddScoped<ICacheService, CacheService>();
            }
        }
    }
}
