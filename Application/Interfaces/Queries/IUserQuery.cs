using Application.DTOs.Commons;
using Application.DTOs.Identity;
using Shared.Results;

namespace Application.Interfaces.Queries
{
    public interface IUserQuery
    {
        PagedResult<AccountDto> GetList(CommonQueryParameters parameters, params string[] searchProperties);
    }
}
