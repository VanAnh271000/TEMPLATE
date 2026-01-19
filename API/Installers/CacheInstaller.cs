using Application.DTOs.Configuration;
using Application.Interfaces.Services.Caching;
using Infrastructure.Caching;

namespace API.Installers
{
    public class CacheInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            //In-Memory Cache
            services.AddMemoryCache();
            services.AddScoped<ICacheService, CacheService>();

            //Redis Cache
            var redisConfig = configuration.GetSection("Redis").Get<RedisConfiguration>();
            services.AddSingleton(redisConfig);
            
            services.AddSingleton(new RedisConnection(redisConfig.ConnectionString!));
            services.AddScoped<ICacheService, RedisCacheService>();
        }
    }
}
