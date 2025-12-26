using Shared.Results;

namespace Shared.Extensions
{
    public static class CollectionExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> enumerable) where T : class
        {
            return enumerable?.Where(item => item != null) ?? Enumerable.Empty<T>();
        }

        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            return enumerable.Where(element => knownKeys.Add(keySelector(element)));
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action(item);
            }
        }

        public static IEnumerable<T> TakePage<T>(this IEnumerable<T> enumerable, int pageNumber, int pageSize)
        {
            return enumerable.Skip((pageNumber) * pageSize).Take(pageSize);
        }

        public static PagedResult<T> ToPagedResult<T>(this IEnumerable<T> enumerable, int pageNumber, int pageSize)
        {
            var totalCount = enumerable.Count();
            var items = enumerable.TakePage(pageNumber, pageSize);

            return new PagedResult<T>(items, totalCount, pageNumber, pageSize);
        }

        public static bool HasItems<T>(this IEnumerable<T> enumerable)
        {
            return enumerable != null && enumerable.Any();
        }

        public static T SafeFirst<T>(this IEnumerable<T> enumerable) where T : class
        {
            return enumerable?.FirstOrDefault();
        }

        public static T SafeLast<T>(this IEnumerable<T> enumerable) where T : class
        {
            return enumerable?.LastOrDefault();
        }
    }
}
