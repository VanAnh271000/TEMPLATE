namespace Application.DTOs.CacheKeys
{
    public class UserCacheKeys
    {
        public const string UserPrefix = "user:entity:";
        public const string QueryPrefix = "user:query:";
        public static string UserById(string userId)
        => $"{UserPrefix}:{userId}";

        public static string UserQuery(string queryHash)
            => $"{QueryPrefix}:{queryHash}";
    }
}
