namespace Application.Interfaces.Commons
{
    public interface IBulkService
    {
        Task BulkInsertAsync<T>(IList<T> entities) where T : class;

        Task BulkUpdateAsync<T>(IList<T> entities) where T : class;
    }
}
