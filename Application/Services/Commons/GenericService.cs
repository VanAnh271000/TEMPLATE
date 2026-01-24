using Application.DTOs.Commons;
using Application.Interfaces.Commons;
using AutoMapper;
using Shared.Constants;
using Shared.QueryParameter;
using Shared.Results;
using System.Linq.Expressions;

namespace Application.Services.Commons
{
    public abstract class GenericService<TEntity, TDto, TCreateDto, TKey> : IGenericService<TEntity, TDto, TCreateDto, TKey>
    where TEntity : class
    where TDto : class
    where TCreateDto : class
    {
        protected readonly IGenericRepository<TEntity, TKey> _repository;
        protected readonly IMapper _mapper;
        protected readonly IUnitOfWork _unitOfWork;
        protected GenericService(IGenericRepository<TEntity, TKey> repository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        
        public virtual ServiceResult<TDto> GetById(TKey id)
        {
            try
            {
                var entity = _repository.GetSingleById(id);
                if (entity == null)
                    return ServiceResult<TDto>.NotFound($"Entity with id {id} not found");

                var dto = _mapper.Map<TDto>(entity);
                return ServiceResult<TDto>.Success(dto);
            }
            catch (Exception ex)
            {
                return ServiceResult<TDto>.InternalServerError($"{ErrorMessages.ErrorRetrivingEntity}: {ex.Message}");
            }
        }

        public virtual ServiceResult<IEnumerable<TDto>> GetAll()
        {
            try
            {
                var entities = _repository.GetAll();
                var dtos = _mapper.Map<IEnumerable<TDto>>(entities);
                return ServiceResult<IEnumerable<TDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<TDto>>.InternalServerError($"{ErrorMessages.ErrorRetrivingEntity}: {ex.Message}");
            }
        }

        public virtual async Task<ServiceResult<TDto>> CreateAsync(TCreateDto dto)
        {
            try
            {
                // Validate DTO
                var dtoValidationResult = ValidateCreatedDto(dto);
                if (!dtoValidationResult.IsSuccess)
                    return ServiceResult<TDto>.ValidationError(dtoValidationResult.Message);
                var entity = _mapper.Map<TEntity>(dto);
                // Validate business rules
                var validationResult = ValidateEntity(entity);
                if (!validationResult.IsSuccess)
                    return ServiceResult<TDto>.ValidationError(validationResult.Message);

                var createdEntity = _repository.Add(entity);
                await _unitOfWork.SaveChangesAsync();
                var resultDto = _mapper.Map<TDto>(createdEntity);
                return ServiceResult<TDto>.Created(resultDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<TDto>.InternalServerError($"{ErrorMessages.ErrorCreatingEntity}: {ex.Message}");
            }
        }

        public virtual async Task<ServiceResult<TDto>> UpdateAsync(TKey id, TCreateDto dto)
        {
            try
            {
                var dtoValidationResult = ValidateCreatedDto(dto);
                if (!dtoValidationResult.IsSuccess)
                    return ServiceResult<TDto>.ValidationError(dtoValidationResult.Message);
                var existingEntity = _repository.GetSingleById(id);
                if (existingEntity == null)
                    return ServiceResult<TDto>.NotFound($"Entity with id {id} not found");

                _mapper.Map(dto, existingEntity);

                var validationResult = ValidateEntity(existingEntity);
                if (!validationResult.IsSuccess)
                    return ServiceResult<TDto>.ValidationError(validationResult.Message);

                _repository.Update(existingEntity);
                await _unitOfWork.SaveChangesAsync();
                var resultDto = _mapper.Map<TDto>(existingEntity);
                return ServiceResult<TDto>.Success(resultDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<TDto>.InternalServerError($"{ErrorMessages.ErrorUpdatingEntity}: {ex.Message}");
            }
        }

        public virtual async Task<ServiceResult> Delete(TKey id)
        {
            try
            {
                var exists = await _repository.IsExistAsync(id);
                if (!exists)
                    return ServiceResult.NotFound($"Entity with id {id} not found");

                _repository.Delete(id);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult.NoContent();
            }
            catch (Exception ex)
            {
                return ServiceResult.InternalServerError($"{ErrorMessages.ErrorDeletingEntity}: {ex.Message}");
            }
        }

        public virtual ServiceResult<PagedResult<TDto>> GetPaged(CommonQueryParameters parameters, string[]? searchProperties, string[]? includes = null)
        {
            var pagedResult = _repository.GetPaged(parameters.ToGenericQueryParameters(), searchProperties, includes);
            if (pagedResult == null)
                return ServiceResult<PagedResult<TDto>>.Error(ErrorMessages.ErrorRetrivingEntity);
            var dtoItems = _mapper.Map<List<TDto>>(pagedResult.Items);

            var result = new PagedResult<TDto>
            {
                Items = dtoItems,
                TotalCount = pagedResult.TotalCount,
                Index = pagedResult.Index,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages
            };
            return ServiceResult<PagedResult<TDto>>.Success(result);
        }

        public virtual ServiceResult<PagedResult<TDto>> GetPaged(GenericQueryParameters parameters, string[]? searchProperties, string[]? includes = null)
        {
            var pagedResult = _repository.GetPaged(parameters, searchProperties, includes);
            if (pagedResult == null)
                return ServiceResult<PagedResult<TDto>>.Error($"{ErrorMessages.ErrorRetrivingEntity}s");
            var dtoItems = _mapper.Map<List<TDto>>(pagedResult.Items);

            var result = new PagedResult<TDto>
            {
                Items = dtoItems,
                TotalCount = pagedResult.TotalCount,
                Index = pagedResult.Index,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages
            };
            return ServiceResult<PagedResult<TDto>>.Success(result);
        }

        public virtual ServiceResult<PagedResult<TDto>> GetPaged(Expression<Func<TEntity, bool>>? predicate, GenericQueryParameters parameters, string[]? searchProperties, string[]? includes = null)
        {
            var pagedResult = _repository.GetPaged(predicate, parameters, searchProperties, includes);
            if (pagedResult == null)
                return ServiceResult<PagedResult<TDto>>.Error(ErrorMessages.ErrorRetrivingEntity);
            var dtoItems = _mapper.Map<List<TDto>>(pagedResult.Items);

            var result = new PagedResult<TDto>
            {
                Items = dtoItems,
                TotalCount = pagedResult.TotalCount,
                Index = pagedResult.Index,
                PageSize = pagedResult.PageSize,
                TotalPages = pagedResult.TotalPages
            };
            return ServiceResult<PagedResult<TDto>>.Success(result);
        }

        protected virtual ServiceResult ValidateCreatedDto(TCreateDto createDto)
        {
            return ServiceResult.Success();
        }
        protected virtual ServiceResult ValidateEntity(TEntity entity)
        {
            // Override this method in derived classes for specific validation
            return ServiceResult.Success();
        }

    }
}
