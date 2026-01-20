using Application.DTOs.Configuration;
using Application.Interfaces.Services.Caching;
using System.Text.Json;

namespace Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly RedisConnection _redis;
        private readonly RedisConfiguration _config;
        private readonly string _instance;
        public RedisCacheService(
        RedisConnection redis,
        RedisConfiguration config)
        {
            _redis = redis;
            _config = config;
            _instance = _config.InstanceName ?? string.Empty;
        }

        private string BuildKey(string key) => $"{_instance}{key}";

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _redis.Database.StringGetAsync(BuildKey(key));
            if (value.IsNullOrEmpty) return default;

            return JsonSerializer.Deserialize<T>(value!);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var json = JsonSerializer.Serialize(value);

            await _redis.Database.StringSetAsync(BuildKey(key), json,(TimeSpan) expiration);
        }

        public async Task RemoveAsync(string key)
        {
            await _redis.Database.KeyDeleteAsync(BuildKey(key));
        }

        public async Task RemoveByPrefixAsync(string prefix)
        {
            var server = _redis.Server;

            var keys = server.Keys(
                pattern: $"{_instance}{prefix}*");

            foreach (var key in keys)
            {
                await _redis.Database.KeyDeleteAsync(key);
            }
        }
    }
}
