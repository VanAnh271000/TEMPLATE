namespace Shared.QueryParameter
{
    public class GenericQueryParameters : BaseQueryParameters
    {
        public List<FilterParameter> Filters { get; set; } = new();

        public GenericQueryParameters AddFilter(string propertyName, string operatorType, object? value, string logicalOperator = "and")
        {
            Filters.Add(new FilterParameter
            {
                Field = propertyName,
                Operator = operatorType,
                Value = value,
                LogicalOperator = logicalOperator
            });
            return this;
        }
    }
}
