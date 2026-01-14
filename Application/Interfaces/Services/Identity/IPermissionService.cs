using Shared.Results;

namespace Application.Interfaces.Services.Identity
{
    public interface IPermissionService
    {
        ServiceResult<List<string>> GetUserPermissionsAsync(string userId);
        ServiceResult<bool> HasPermissionAsync(string userId, string permission);
    }
}
