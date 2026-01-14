using Application.DTOs.Commons;
using Application.DTOs.Identity;
using Application.Interfaces.Commons;
using Domain.Entities.Identity;
using Shared.Results;

namespace Application.Interfaces.Services.Identity
{
    public interface IAccountService : IGenericService<ApplicationUser, AccountDto, CreateAccountDto, string>
    {
        ServiceResult<PagedResult<AccountDto>> GetList(CommonQueryParameters parameters);
    }
}
