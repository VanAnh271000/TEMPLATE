using Application.DTOs.Metrics;
using Application.Interfaces.Services.Caching;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;

namespace Infrastructure.Caching
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ConcurrentDictionary<string, byte> _keys = new();
        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<T?> GetAsync<T>(string key)
        {
            var value = _cache.Get<T>(key);

            if (value == null)
                CacheMetrics.Miss.Add(1);
            else
                CacheMetrics.Hit.Add(1);

            return Task.FromResult(value);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions();

            if (expiration.HasValue)
                options.SetAbsoluteExpiration(expiration.Value);

            _cache.Set(key, value, options);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string key)
        {
            _cache.Remove(key);
            return Task.CompletedTask;
        }

        public Task RemoveByPrefixAsync(string prefix)
        {
            var keysToRemove = _keys.Keys
                .Where(k => k.StartsWith(prefix))
                .ToList();

            foreach (var key in keysToRemove)
            {
                _cache.Remove(key);
                _keys.TryRemove(key, out _);
            }

            return Task.CompletedTask;
        }
    }
}
