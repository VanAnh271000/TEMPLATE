using Shared.Results;

namespace Application.Interfaces.Services.Notification
{
    public interface IFirebaseService
    {
        Task<ServiceResult> RegisterAsync(Guid userId, string token);
        Task<ServiceResult> RemoveAsync(Guid userId, string token);
        Task<ServiceResult> DeactivateAsync(string token);
        Task<ServiceResult<IReadOnlyList<string>>> GetActiveTokensAsync(Guid userId);
    }
}
