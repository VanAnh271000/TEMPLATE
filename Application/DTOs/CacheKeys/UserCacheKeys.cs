namespace Application.DTOs.CacheKeys
{
    public class UserCacheKeys
    {
        public static string UserById(string userId)
        => $"user:entity:{userId}";

        public static string UserQuery(string queryHash)
            => $"user:query:{queryHash}";
    }
}
