using Application.DTOs.CacheKeys;
using Application.DTOs.Commons;
using Application.DTOs.Identity;
using Application.Interfaces.Commons;
using Application.Interfaces.Queries;
using Application.Interfaces.Services.Caching;
using Application.Interfaces.Services.Identity;
using Application.Services.Commons;
using AutoMapper;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Shared.Constants;
using Shared.Extensions;
using Shared.Results;
using System.Data;

namespace Application.Services.Identity
{
    public class AccountService : GenericService<ApplicationUser, AccountDto, CreateAccountDto, string>, IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenericRepository<ApplicationRole, string> _roleRepository;
        private readonly IGenericRepository<UserRole, string> _userRoleRepository;
        private readonly IUserQuery _userQuery;
        private readonly ICacheService _cache;
        public AccountService(IGenericRepository<ApplicationUser, string> repository, 
            IGenericRepository<ApplicationRole, string> roleRepository,
            IGenericRepository<UserRole, string> userRoleRepository,
            IMapper mapper, IUnitOfWork unitOfWork, 
            UserManager<ApplicationUser> userManager,
            ICacheService cache,
            IUserQuery userQuery)
            : base(repository, mapper, unitOfWork)
        {
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _userManager = userManager;
            _userQuery = userQuery;
            _cache = cache;
        }

        public async override Task<ServiceResult<AccountDto>> CreateAsync(CreateAccountDto createAccountDto)
        {
            var validationResult = ValidateDto(createAccountDto);
            if (!validationResult.IsSuccess) 
                return ServiceResult<AccountDto>.ValidationError(validationResult.Message);
            var user = new ApplicationUser
            {
                UserName = createAccountDto.UserName,
                Email = createAccountDto.Email,
                FullName = createAccountDto.FullName,
                CreatedTime = DateTime.UtcNow,
                IsActive = true
            };
            var result = await _userManager.CreateAsync(user, createAccountDto.Password);
            if (!result.Succeeded) return ServiceResult<AccountDto>.Error(string.Join(", ", result.Errors.Select(e => e.Description)));
            var accountDto = _mapper.Map<AccountDto>(user);

            var roles = _roleRepository.GetMulti(r => createAccountDto.RoleIds.Contains(r.Id), ["RolePermissions"]).ToList();
            if (roles != null && roles.Any())
            {
                foreach (var role in roles)
                {
                    _userRoleRepository.Add(new UserRole
                    {
                        UserId = user.Id,
                        RoleId = role.Id
                    });
                }
                await _unitOfWork.SaveChangesAsync();
            }
            accountDto.Roles = _mapper.Map<List<RoleDto>>(roles);
            await _cache.RemoveByPrefixesAsync(UserCacheKeys.UserPrefix, $"user:query:");
            return ServiceResult<AccountDto>.Created(accountDto);
        }

        public async Task<ServiceResult<PagedResult<AccountDto>>> GetListAsync(CommonQueryParameters parameters)
        {
            var queryHash = parameters.ToSha256Hash();
            var data = await _cache.GetOrSetAsync(
                UserCacheKeys.UserQuery(queryHash),
                async () =>
                {
                    var result = _userQuery.GetList(
                        parameters,
                        new[] { "UserName", "FullName", "Department.Name" });

                    return result;
                },
                TimeSpan.FromMinutes(5)
            );

            if (data == null) return ServiceResult<PagedResult<AccountDto>>.NotFound(ErrorMessages.UserNotFound);
            return ServiceResult<PagedResult<AccountDto>>.Success(data);
        }

        private ServiceResult ValidateDto(CreateAccountDto dto)
        {
            if (dto == null || dto.UserName.IsNullOrEmpty() || dto.Password.IsNullOrEmpty())
                return ServiceResult.ValidationError("Dữ liệu không hợp lệ");

            return ServiceResult.Success();
        }

    }
}
