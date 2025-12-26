using Application.DTOs.Commons;
using Shared.QueryParameter;
using Shared.Results;
using System.Linq.Expressions;

namespace Application.Interfaces.Commons
{
    public interface IGenericService<TEntity, TDto, TCreateDto, TKey>
    where TEntity : class
    where TDto : class
    where TCreateDto : class
    {
        ServiceResult<TDto> GetById(TKey id);
        ServiceResult<IEnumerable<TDto>> GetAll();
        Task<ServiceResult<TDto>> CreateAsync(TCreateDto dto);
        Task<ServiceResult<TDto>> UpdateAsync(TKey id, TCreateDto dto);
        Task<ServiceResult> Delete(TKey id);
        ServiceResult<PagedResult<TDto>> GetPaged(CommonQueryParameters parameters, string[]? searchProperties, string[]? includes = null);
        ServiceResult<PagedResult<TDto>> GetPaged(GenericQueryParameters parameters, string[]? searchProperties, string[]? includes = null);
        ServiceResult<PagedResult<TDto>> GetPaged(Expression<Func<TEntity, bool>>? predicate, GenericQueryParameters parameters, string[]? searchProperties, string[]? includes = null);
    }
}
