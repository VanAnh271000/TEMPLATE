using Shared.QueryParameter;
using Shared.Results;
using System.Linq.Expressions;

namespace Application.Interfaces.Commons
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : class
    {
        void Detach(TEntity entity);
        TEntity Add(TEntity entity);

        // Marks an entity as modified
        void Update(TEntity entity);

        // Marks an entity to be removed
        TEntity Delete(TEntity entity);
        TEntity Delete(TKey id);

        //Delete multi records
        void DeleteMulti(Expression<Func<TEntity, bool>> where);
        void DeleteMulti(IEnumerable<TEntity> where);

        // Get an entity by int id
        Task<bool> IsExistAsync(TKey id);
        TEntity GetSingleById(TKey id);
        TEntity GetSingleByCondition(Expression<Func<TEntity, bool>> expression, string[] includes = null);

        //Get list of entities by condition
        IEnumerable<TEntity> GetAll(string[] includes = null);
        IEnumerable<TEntity> GetMulti(Expression<Func<TEntity, bool>> predicate, string[] includes = null);
        IEnumerable<TEntity> GetMultiPaging(Expression<Func<TEntity, bool>> filter, out int total, int index = 0, int size = 50, string[] includes = null);
        IEnumerable<TEntity> GetMultiByFilterNoPaging(Expression<Func<TEntity, bool>>? predicate, GenericQueryParameters parameters, string[]? searchProperties, string[]? includes = null);
        PagedResult<TEntity> GetPaged(GenericQueryParameters parameters, string[]? searchProperties, string[]? includes = null);
        PagedResult<TEntity> GetPaged(Expression<Func<TEntity, bool>>? predicate, GenericQueryParameters parameters, string[]? searchProperties, string[]? includes = null);

        int Count(Expression<Func<TEntity, bool>> where);
        bool CheckContains(Expression<Func<TEntity, bool>> predicate);
    }
}
