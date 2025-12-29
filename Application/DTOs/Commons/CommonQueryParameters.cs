using Shared.QueryParameter;

namespace Application.DTOs.Commons
{
    public class CommonQueryParameters : BaseQueryParameters
    {
        public Guid? CompanyId { get; set; }
        public virtual GenericQueryParameters ToGenericQueryParameters()
        {
            var genericParams = new GenericQueryParameters
            {
                Index = Index,
                PageSize = PageSize,
                SortBy = SortBy,
                SortDirection = SortDirection,
                Search = Search
            };
            if (CompanyId.HasValue)
            {
                genericParams.AddFilter("CompanyId", "==", CompanyId.Value);
            }
            return genericParams;
        }
    }
}
