using Application.DTOs.Commons;
using Application.DTOs.Identity;
using Application.Interfaces.Commons;
using Domain.Entities.Identity;
using Shared.Results;

namespace Application.Interfaces.Services.Identity
{
    public interface IRoleService : IGenericService<ApplicationRole, RoleDto, CreateRoleDto, string>
    {
        ServiceResult<IEnumerable<RoleDto>> GetList();
        ServiceResult<PagedResult<RoleDto>> GetListRoleAccounts(CommonQueryParameters parameters);
    }
}
