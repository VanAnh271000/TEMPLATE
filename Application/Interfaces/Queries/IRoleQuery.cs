using Application.DTOs.Commons;
using Application.DTOs.Identity;
using Shared.Results;

namespace Application.Interfaces.Queries
{
    public interface IRoleQuery
    {
        PagedResult<RoleDto> GetListRoleAccounts(CommonQueryParameters parameters, params string[] searchProperties);
    }
}
