using Application.DTOs.Identity;
using Shared.Results;

namespace Application.Interfaces.Services.Identity
{
    public interface IAuthenticationService
    {
        Task<ServiceResult<AuthenticationResult>> LoginAsync(LoginRequest loginRequest);
        Task<ServiceResult<AuthenticationResult>> Login2FA(LoginOtpRequest loginRequest);
        Task<ServiceResult<AuthenticationResult>> RefreshTokenAsync(RefreshTokenRequest request);
        Task<ServiceResult> ChangePassword(ChangePasswordRequest changePassword, string userName);
        Task<ServiceResult> ResendLoginOTP(string userName);
    }
}
