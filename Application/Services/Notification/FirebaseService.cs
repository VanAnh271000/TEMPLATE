using Application.Interfaces.Commons;
using Application.Interfaces.Services.Notification;
using Domain.Entities.Identity;
using Serilog;
using Shared.Constants;
using Shared.Results;

namespace Application.Services.Notification
{
    public class FirebaseService : IFirebaseService
    {
        private readonly IGenericRepository<FirebaseToken, int> _repository;
        private readonly IUnitOfWork _unitOfWork;
        public FirebaseService(IGenericRepository<FirebaseToken, int> repository,
            IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ServiceResult> RegisterAsync(Guid userId, string token)
        {
            try
            {
                var exists = _repository.GetMulti(x => x.DeviceToken == token);
                if (exists.Any())
                    return ServiceResult.Error(ErrorMessages.FirebaseTokenAlreadyExists);

                var entity = new FirebaseToken
                {
                    UserId = userId,
                    DeviceToken = token,
                };

                _repository.Add(entity);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error registering Firebase token for user {UserId}", userId);
                return ServiceResult.InternalServerError($"{ErrorMessages.ErrorRegisteringFirebaseToken}: {ex.Message}");
            }
        }

        public async Task<ServiceResult> RemoveAsync(Guid userId, string token)
        {
            try
            {
                _repository.DeleteMulti(x => x.UserId == userId && x.DeviceToken == token);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error removing Firebase token for user {UserId}", userId);
                return ServiceResult.InternalServerError($"{ErrorMessages.ErrorRemovingFirebaseToken}: {ex.Message}");
            }
        }

        public async Task<ServiceResult> DeactivateAsync(string token)
        {
            try
            {
                var entity = _repository.GetSingleByCondition(x => x.DeviceToken == token);
                if (entity == null) return ServiceResult.Error(ErrorMessages.FirebaseTokenNotFound);

                entity.IsActive = false;
                _repository.Update(entity);
                await _unitOfWork.SaveChangesAsync();
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error deactivating Firebase token {Token}", token);
                return ServiceResult.InternalServerError($"{ErrorMessages.ErrorDeactivatingFirebase}: {ex.Message}");
            }
        }

        public async Task<ServiceResult<IReadOnlyList<string>>> GetActiveTokensAsync(Guid userId)
        {
            try
            {
                var tokens = _repository.GetMulti(x => x.UserId == userId);
                return ServiceResult< IReadOnlyList<string>>.Success(tokens.Select(x => x.DeviceToken).ToList());
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error retrieving active Firebase tokens for user {UserId}", userId);
                return ServiceResult<IReadOnlyList<string>>.InternalServerError($"{ErrorMessages.ErrorRetrievingFirebaseTokens}: {ex.Message}");
            }
        }
    }
}
