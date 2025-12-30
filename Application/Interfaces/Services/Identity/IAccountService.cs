using Application.DTOs.Commons;
using Application.DTOs.Identity;
using Shared.Results;

namespace Application.Interfaces.Services.Identity
{
    public interface IAccountService
    {
        ServiceResult<PagedResult<AccountDto>> GetList(CommonQueryParameters parameters);
    }
}
