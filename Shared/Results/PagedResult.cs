namespace Shared.Results
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int TotalCount { get; set; }
        public int Index { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => Index > 1;
        public bool HasNextPage => Index < TotalPages;
        public PagedResult()
        {
            Data = new List<T>();
        }

        public PagedResult(IEnumerable<T> items, int totalCount, int index, int pageSize)
        {
            Data = items;
            TotalCount = totalCount;
            Index = index;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        }

        public static PagedResult<T> Create(IEnumerable<T> items, int totalCount, int index, int pageSize)
        {
            return new PagedResult<T>(items, totalCount, index, pageSize);
        }
    }
}
