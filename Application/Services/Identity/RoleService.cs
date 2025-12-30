using Application.DTOs.Commons;
using Application.DTOs.Identity;
using Application.Interfaces.Commons;
using Application.Interfaces.Queries;
using Application.Interfaces.Services.Identity;
using Application.Services.Commons;
using AutoMapper;
using Domain.Entities.Identity;
using Microsoft.Extensions.Caching.Memory;
using Shared.Constants;
using Shared.Results;
using System.Data;

namespace Application.Services.Identity
{
    public class RoleService : GenericService<ApplicationRole, RoleDto, CreateRoleDto, string>, IRoleService
    {
        private readonly IMemoryCache _cache;
        private readonly IGenericRepository<Permission, int> _permissionRepository;
        private readonly IGenericRepository<UserRole, string> _userRoleRepository;
        private readonly IGenericRepository<RolePermission, int> _rolePermissionRepository;
        private readonly IGenericRepository<ApplicationUser, string> _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IRoleQuery _roleQuery;
        public RoleService(
            IGenericRepository<ApplicationRole,string> roleRepository, 
            IGenericRepository<ApplicationUser, string> userRepository,
            IGenericRepository<RolePermission, int> rolePermissionRepository,
            IGenericRepository<Permission, int> permissionRepository,
            IGenericRepository<UserRole, string> userRoleRepository,
            ICurrentUserService currentUserService,
            IMapper mapper, IUnitOfWork unitOfWork,
            IRoleQuery roleQuery, IUserQuery userQuery,
            IMemoryCache cache)
            : base(roleRepository, mapper, unitOfWork)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _permissionRepository = permissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
            _userRoleRepository = userRoleRepository;
            _roleQuery = roleQuery;
            _cache = cache;
        }
       
        public ServiceResult<IEnumerable<RoleDto>> GetList()
        {
            try
            {
                var roles = _repository.GetAll();
                if (roles == null) return ServiceResult<IEnumerable<RoleDto>>.NotFound(ErrorMessages.RoleNotFound);
                var roleDtos = _mapper.Map<IEnumerable<RoleDto>>(roles);
                return ServiceResult<IEnumerable<RoleDto>>.Success(roleDtos);
            }
            catch (Exception ex)
            {
                return ServiceResult<IEnumerable<RoleDto>>.InternalServerError($"{ErrorMessages.ErrorRetrivingRoles}: {ex.Message}");
            }
        }

        public ServiceResult<PagedResult<RoleDto>> GetListRoleAccounts(CommonQueryParameters parameters)
        {
            try
            {
                var result = _roleQuery.GetListRoleAccounts(parameters, ["Name"]);
                if (result == null)
                    return ServiceResult<PagedResult<RoleDto>>.NotFound(ErrorMessages.RoleNotFound);
                return ServiceResult<PagedResult<RoleDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<RoleDto>>.InternalServerError($"{ErrorMessages.ErrorRetrivingRoles}: {ex.Message}");
            }
        }

        public override async Task<ServiceResult<RoleDto>> CreateAsync(CreateRoleDto dto)
        {
            try
            {
                var role = _mapper.Map<ApplicationRole>(dto);
                var validationResult = ValidateEntity(role);
                if (!validationResult.IsSuccess) return ServiceResult<RoleDto>.Error(validationResult.Message);
                role.RolePermissions = dto.PermissionIds.Select(pid => new RolePermission
                {
                    PermissionId = pid,
                    Role = role

                }).ToList();
                role.UserRoles = dto.AccountIds.Select(uid => new UserRole
                {
                    UserId = uid,
                    Role = role
                }).ToList();
                var createdRole = _repository.Add(role);
                await _unitOfWork.SaveChangesAsync();

                var cacheKey = $"permissions_{_currentUserService.UserId}";
                _cache.Remove(cacheKey);

                var roleDto = _mapper.Map<RoleDto>(createdRole);
                var permissions = _permissionRepository.GetMulti(p => dto.PermissionIds.Contains(p.Id)).ToList();
                var accounts = _userRepository.GetMulti(u => dto.AccountIds.Contains(u.Id)).ToList();
                roleDto.Permissions = _mapper.Map<IEnumerable<PermissionDto>>(permissions);
                roleDto.Accounts = _mapper.Map<IEnumerable<AccountDto>>(accounts);
                return ServiceResult<RoleDto>.Success(roleDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<RoleDto>.InternalServerError($"{ErrorMessages.ErrorCreateRole}: {ex.Message}");
            }
        }

        public override async Task<ServiceResult<RoleDto>> UpdateAsync(string id, CreateRoleDto dto)
        {
            try
            {
                var existingRole = _repository.GetSingleById(id);
                if (existingRole == null) return ServiceResult<RoleDto>.NotFound(ErrorMessages.RoleNotFound);
                _mapper.Map(dto, existingRole);
                var validationResult = ValidateEntity(existingRole);
                if (!validationResult.IsSuccess) return ServiceResult<RoleDto>.Error(validationResult.Message);

                var newPermissionIds = dto.PermissionIds.ToHashSet();
                var newAccountIds = dto.AccountIds.ToHashSet();

                foreach (var accId in newAccountIds)
                {
                    var cacheKey = $"permissions_{accId}";
                    _cache.Remove(cacheKey);
                }

                var existingRolePermission = _rolePermissionRepository.GetMulti(x => x.RoleId == existingRole.Id);
                var existingAccountRoles = _userRoleRepository.GetMulti(x => x.RoleId == existingRole.Id);

                var existingPermissionIds = existingRolePermission.Select(rp => rp.PermissionId).ToHashSet();
                var existingAccountIds = existingAccountRoles.Select(ur => ur.UserId).ToHashSet();

                var permissionsToRemove = existingPermissionIds.Except(newPermissionIds).ToList();
                var accountsToRemove = existingAccountIds.Except(newAccountIds).ToList();
                if (permissionsToRemove.Any())
                {
                    _rolePermissionRepository.DeleteMulti(rp => rp.RoleId == existingRole.Id && permissionsToRemove.Contains(rp.PermissionId));
                }
                if (accountsToRemove.Any())
                {
                    _userRoleRepository.DeleteMulti(ur => ur.RoleId == existingRole.Id && accountsToRemove.Contains(ur.UserId));
                    foreach (var accId in accountsToRemove)
                    {
                        var cacheKey = $"permissions_{accId}";
                        _cache.Remove(cacheKey);
                    }
                }

                var permisstionsToAdd = newPermissionIds.Except(existingPermissionIds).ToList();
                var accountsToAdd = newAccountIds.Except(existingAccountIds).ToList();
                foreach (var permission in permisstionsToAdd)
                {
                    _rolePermissionRepository.Add(new RolePermission
                    {
                        PermissionId = permission,
                        RoleId = existingRole.Id
                    });
                }
                foreach (var account in accountsToAdd)
                {
                    _userRoleRepository.Add(new UserRole
                    {
                        UserId = account,
                        RoleId = existingRole.Id
                    });

                }
                _repository.Update(existingRole);
                await _unitOfWork.SaveChangesAsync();

                var resultDto = _mapper.Map<RoleDto>(existingRole);
                var permissions = _permissionRepository.GetMulti(p => dto.PermissionIds.Contains(p.Id)).ToList();
                resultDto.Permissions = _mapper.Map<IEnumerable<PermissionDto>>(permissions);
                var accounts = _userRepository.GetMulti(u => dto.AccountIds.Contains(u.Id)).ToList();
                resultDto.Accounts = _mapper.Map<IEnumerable<AccountDto>>(accounts);
                return ServiceResult<RoleDto>.Success(resultDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<RoleDto>.InternalServerError($"{ErrorMessages.ErrorUpdateRole}: {ex.Message}");
            }
        }

        protected override ServiceResult ValidateEntity(ApplicationRole entity)
        {
            ApplicationRole existRole = _repository.GetSingleByCondition(x => x.Name == entity.Name &&  x.Id != entity.Id);
            if (existRole != null)
            {
                return ServiceResult.ValidationError(ErrorMessages.RoleNameAlreadyExists);
            }
            return ServiceResult.Success();
        }
    }
}
