using System.ComponentModel.DataAnnotations;

namespace Shared.QueryParameter
{
    public class BaseQueryParameters
    {
        private string _search;
        [Range(0, int.MaxValue, ErrorMessage = "Page must be greater than 0")]
        public int Index { get; set; } = 0;

        public int PageSize { get; set; } = 10;

        public string? SortBy { get; set; }
        public string SortDirection { get; set; } = "asc";
        public string? Search
        {
            get => _search;
            set => _search = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }
    }
}
