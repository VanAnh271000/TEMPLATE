using Application.DTOs.Commons;
using Application.DTOs.Identity;
using Application.Interfaces.Commons;
using Application.Interfaces.Queries;
using Application.Interfaces.Services.Identity;
using Application.Services.Commons;
using AutoMapper;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Shared.Constants;
using Shared.Results;
using System.Data;

namespace Application.Services.Identity
{
    public class AccountService : GenericService<ApplicationUser, AccountDto, CreateAccountDto, string>, IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenericRepository<ApplicationRole, string> _roleRepository;
        private readonly IUserQuery _userQuery;

        public AccountService(IGenericRepository<ApplicationUser, string> repository, 
            IGenericRepository<ApplicationRole, string> roleRepository,
            IMapper mapper, IUnitOfWork unitOfWork, 
            UserManager<ApplicationUser> userManager,
            IUserQuery userQuery)
            : base(repository, mapper, unitOfWork)
        {
            _roleRepository = roleRepository;
            _userManager = userManager;
            _userQuery = userQuery;
        }

        public async override Task<ServiceResult<AccountDto>> CreateAsync(CreateAccountDto createAccountDto)
        {
            try
            {
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
                accountDto.Roles = _mapper.Map<List<RoleDto>>(roles);

                return ServiceResult<AccountDto>.Success(accountDto);
            }
            catch(Exception ex)
            {
                return ServiceResult<AccountDto>.InternalServerError($"An error occurred while creating the user: {ex.Message}");
            }
        }

        public ServiceResult<PagedResult<AccountDto>> GetList(CommonQueryParameters parameters)
        {
            try
            {
                var result = _userQuery.GetList(parameters, new[] { "UserName", "FullName", "Department.Name" });
                if (result == null)
                    return ServiceResult<PagedResult<AccountDto>>.NotFound(ErrorMessages.UserNotFound);
                return ServiceResult<PagedResult<AccountDto>>.Success(result);
            }
            catch (Exception ex)
            {
                return ServiceResult<PagedResult<AccountDto>>.InternalServerError($"{ErrorMessages.GetAccountFailed}: {ex.Message}");
            }
        }

    }
}
