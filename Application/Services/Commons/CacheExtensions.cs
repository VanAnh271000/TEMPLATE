using Application.Interfaces.Services.Caching;

namespace Application.Services.Commons
{
    public static class CacheExtensions
    {
        public static async Task<T> GetOrSetAsync<T>(
            this ICacheService cache,
            string key,
            Func<Task<T>> factory,
            TimeSpan? expiration = null)
        {
            var cached = await cache.GetAsync<T>(key);
            if (cached != null) return cached;

            var value = await factory();
            if (value != null) await cache.SetAsync(key, value, expiration);

            return value;
        }

        public static async Task<T> GetOrSetWithLockAsync<T>(
            this ICacheService cache,
            string key,
            Func<Task<T>> factory,
            TimeSpan expiration,
            SemaphoreSlim semaphore)
        {
            var cached = await cache.GetAsync<T>(key);
            if (cached != null) return cached;

            await semaphore.WaitAsync();
            try
            {
                cached = await cache.GetAsync<T>(key);
                if (cached != null)
                    return cached;

                var value = await factory();
                await cache.SetAsync(key, value, expiration);

                return value;
            }
            finally
            {
                semaphore.Release();
            }
        }
        public static async Task RemoveByPrefixesAsync(
            this ICacheService cache,
            params string[] prefixes)
        {
            foreach (var prefix in prefixes)
            {
                await cache.RemoveByPrefixAsync(prefix);
            }
        }
    }
}
