using Application.DTOs.Identity;
using Shared.Results;

namespace Application.Interfaces.Services.Identity
{
    public interface IAuthenticationService
    {
        Task<ServiceResult<AccountDto>> RegisterAsync(CreateAccountDto createAccountDto);
        Task<ServiceResult<AuthenticationResult>> LoginAsync(LoginDto loginRequest);
        Task<ServiceResult<AuthenticationResult>> Login2FaAsync(LoginOtpDto loginRequest);
        Task<ServiceResult<AuthenticationResult>> RefreshTokenAsync(RefreshTokenDto request);
        Task<ServiceResult> ChangePasswordAsync(ChangePasswordDto changePassword, string userName);
        Task<ServiceResult> ResendLoginOTPAsync(string userName);
        Task<ServiceResult> SendEmailConfirmationLinkAsync(string userName, string email);
        Task<ServiceResult> ConfirmEmailAsync(string userName, EmailToken emailToken);
        Task<ServiceResult<AccountDto>> GetCurrentUserAsync(string userName);
        Task<ServiceResult> UpdateProfileAsync(string userName, UpdateProfileDto profile);
        Task<ServiceResult> ForgotPasswordByEmailAsync(string email);
        Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    }
}
