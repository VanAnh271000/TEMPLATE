using Application.DTOs.Identity;
using Application.Interfaces.Commons;
using Application.Interfaces.Services.Identity;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Shared.Constants;
using Shared.Results;

namespace Application.Services.Identity
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IGenericRepository<RefreshToken, int> _refreshTokenRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;
        public AuthenticationService(SignInManager<ApplicationUser> signInManager,
            IGenericRepository<RefreshToken, int> refreshTokenRepository,
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            IJwtTokenService jwtTokenService,
            IUnitOfWork unitOfWork)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            _unitOfWork = unitOfWork;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<ServiceResult<AuthenticationResult>> LoginAsync(LoginRequest loginRequest)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginRequest.UserName);
                if (user == null || !user.IsActive)
                {
                    return ServiceResult<AuthenticationResult>.Error(ErrorMessages.InvalidCredentials);
                }
                var signInResult = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
                if (!signInResult.Succeeded)
                {
                    return ServiceResult<AuthenticationResult>.Error(ErrorMessages.InvalidCredentials);
                }
                if (user.TwoFactorEnabled)
                {
                    var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                    var sendEmailResult = SendLoginOtpByEmail(user, code);
                    if (!sendEmailResult.IsSuccess)
                    {
                        return ServiceResult<AuthenticationResult>.Error("Failed to resend OTP email.");
                    }
                    return ServiceResult<AuthenticationResult>.Success(new AuthenticationResult()
                    {
                        UserName = user.UserName,
                        TwoFactorEnabled = true
                    });
                }

                var tokens = await GenerateToken(user, loginRequest.IpAddress);
                return ServiceResult<AuthenticationResult>.Success(new AuthenticationResult()
                {
                    AccessToken = tokens.Item1,
                    RefreshToken = tokens.Item2,
                });
            }
            catch (Exception ex)
            {
                return ServiceResult<AuthenticationResult>.InternalServerError("An error occurred during login.");
            }
        }

        public async Task<ServiceResult<AuthenticationResult>> Login2FA(LoginOtpRequest loginRequest)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginRequest.UserName);
                if (user == null || !user.IsActive)
                {
                    return ServiceResult<AuthenticationResult>.Error(ErrorMessages.UserNotFound);
                }
                else if (!user.TwoFactorEnabled)
                {
                    return ServiceResult<AuthenticationResult>.Error(ErrorMessages.TwoFactorNotEnabled);
                }
                var validVertification = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, loginRequest.Code);
                if (!validVertification)
                    return ServiceResult<AuthenticationResult>.Error(ErrorMessages.InvalidOTPCode);
                
                var tokens = await GenerateToken(user, loginRequest.IpAddress);
                return ServiceResult<AuthenticationResult>.Success(new AuthenticationResult
                {
                    AccessToken = tokens.Item1,
                    RefreshToken = tokens.Item2,
                });
            }
            catch (Exception ex)
            {
                return ServiceResult<AuthenticationResult>.InternalServerError("An error occurred during 2FA login.");
            }
        }

        public async Task<ServiceResult<AuthenticationResult>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            try
            {
                var storedRefreshToken = _refreshTokenRepository.GetSingleByCondition(rt => rt.Token == request.RefreshToken
                                             && rt.IsActive
                                             && rt.ExpiryDate > DateTime.UtcNow, ["User"]);

                if (storedRefreshToken == null)
                {
                    return ServiceResult<AuthenticationResult>.Error(ErrorMessages.InvalidRefreshToken);
                }

                var user = storedRefreshToken.User;
                if (user == null || !user.IsActive)
                {
                    return ServiceResult<AuthenticationResult>.Error(ErrorMessages.UserNotFound);
                }

                _refreshTokenRepository.Delete(storedRefreshToken);
                var tokens = await GenerateToken(user, request.IpAddress);

                return ServiceResult<AuthenticationResult>.Success(new AuthenticationResult
                {
                    AccessToken = tokens.Item1,
                    RefreshToken = tokens.Item2,
                });
            }
            catch (Exception ex)
            {
                return ServiceResult<AuthenticationResult>.Error(ErrorMessages.InvalidRefreshToken);
            }
        }

        public async Task<ServiceResult> ChangePassword(ChangePasswordRequest changePassword, string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null) return ServiceResult.Error(ErrorMessages.UserNotFound);

                var isValid = await _userManager.CheckPasswordAsync(user, changePassword.OldPassword);
                if(!isValid) return ServiceResult.Error(ErrorMessages.InvalidPassword);

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var changePasswordResult = await _userManager.ResetPasswordAsync(user, token, changePassword.Password);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors)
                    {
                        return ServiceResult.Error(error.Description);
                    }
                }
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.InternalServerError("An error occurred while changing the password.");
            }
        }

        public async Task<ServiceResult> ResendLoginOTP(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    return ServiceResult.Error(ErrorMessages.UserNotFound);
                }
                var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                var sendEmailResult = SendLoginOtpByEmail(user, code);
                if (!sendEmailResult.IsSuccess)
                {
                    return ServiceResult.Error("Failed to resend OTP email.");
                }
                return ServiceResult<AuthenticationResult>.Success(new AuthenticationResult()
                {
                    UserName = user.UserName,
                    TwoFactorEnabled = true
                });
            }
            catch (Exception ex)
            {
                return ServiceResult.InternalServerError("An error occurred while resending the OTP.");
            }
        }
        
        public ServiceResult SendLoginOtpByEmail(ApplicationUser user, string code)
        {
            //if (!user.EmailConfirmed)
            //{
            //    return ServiceResult.BadRequest("You haven't confirmed your email yet. Please contact admin for support!");
            //}
            //var body = $"<p>Hi {user.FullName}</p>" +
            //    $"<p>Please login with the OTP below.</p>" +
            //    $"<p><b>{code}</b></p>";
            //var message = new EmailMessage(new string[] { user.Email }, "[LOGIN OTP]", body);
            //_emailService.SendEmail(message);
            return ServiceResult.Success();
        }

        private async Task<Tuple<string,string>> GenerateToken(ApplicationUser user, string createdByIp)
        {
            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();
            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["JWT:RefreshTokenValidity"])),
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedByIp = createdByIp
            };
            _jwtTokenService.RevokeExpireRefreshToken(user.Id);
            _refreshTokenRepository.Add(refreshTokenEntity);
            await _unitOfWork.SaveChangesAsync();
            return new Tuple<string, string>(accessToken, refreshToken);
        }
    }
}
