using Application.DTOs.Identity;
using Application.Interfaces.Commons;
using Application.Interfaces.Services.Hangfire;
using Application.Interfaces.Services.Identity;
using AutoMapper;
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
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBackgroundTaskService _backgroundTaskService;

        public AuthenticationService(SignInManager<ApplicationUser> signInManager,
            IGenericRepository<RefreshToken, int> refreshTokenRepository,
            UserManager<ApplicationUser> userManager,
            IJwtTokenService jwtTokenService,
            IConfiguration configuration,
            IEmailService emailService,
            IUnitOfWork unitOfWork, IMapper mapper,
            IBackgroundTaskService backgroundTaskService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _jwtTokenService = jwtTokenService;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _backgroundTaskService = backgroundTaskService;
        }

        public async Task<ServiceResult<AccountDto>> GetCurrentUserAsync(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null) return ServiceResult<AccountDto>.Error(ErrorMessages.UserNotFound);
                var accountDto = _mapper.Map<AccountDto>(user);
                return ServiceResult<AccountDto>.Success(accountDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<AccountDto>.InternalServerError("An error occurred while retrieving the user.");
            }
        }

        public async Task<ServiceResult<AccountDto>> RegisterAsync(CreateAccountDto createAccountDto)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = createAccountDto.UserName,
                    Email = createAccountDto.Email,
                    FullName = createAccountDto.FullName,
                    CreatedTime = DateTime.UtcNow,
                    IsActive = true
                };
                var result = await _userManager.CreateAsync(user, createAccountDto.Password);
                if (!result.Succeeded) return ServiceResult<AccountDto>.Error(string.Join(", ", result.Errors.Select(e => e.Description)));
                var accountDto = _mapper.Map<AccountDto>(result);

                return ServiceResult<AccountDto>.Success(accountDto);
            }
            catch (Exception ex)
            {
                return ServiceResult<AccountDto>.InternalServerError($"An error occurred while creating the user: {ex.Message}");
            }
        }

        public async Task<ServiceResult<AuthenticationResult>> LoginAsync(LoginDto loginRequest)
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

                var tokens = await GenerateTokenAsync(user, loginRequest.IpAddress);
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

        public async Task<ServiceResult<AuthenticationResult>> Login2FaAsync(LoginOtpDto loginRequest)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginRequest.UserName);
                if (user == null || !user.IsActive) return ServiceResult<AuthenticationResult>.Error(ErrorMessages.UserNotFound);
                else if (!user.TwoFactorEnabled) return ServiceResult<AuthenticationResult>.Error(ErrorMessages.TwoFactorNotEnabled);
                var validVertification = await _userManager.VerifyTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider, loginRequest.Code);
                if (!validVertification) return ServiceResult<AuthenticationResult>.Error(ErrorMessages.InvalidOTPCode);
                
                var tokens = await GenerateTokenAsync(user, loginRequest.IpAddress);
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

        public async Task<ServiceResult<AuthenticationResult>> RefreshTokenAsync(RefreshTokenDto request)
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
                var tokens = await GenerateTokenAsync(user, request.IpAddress);

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

        public async Task<ServiceResult> ChangePasswordAsync(ChangePasswordDto changePassword, string userName)
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

        public async Task<ServiceResult> ResendLoginOTPAsync(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null) return ServiceResult.Error(ErrorMessages.UserNotFound);
                var code = await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                var sendEmailResult = SendLoginOtpByEmail(user, code);
                if (!sendEmailResult.IsSuccess) return ServiceResult.Error(ErrorMessages.SendEmailFailed);
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

        public async Task<ServiceResult> SendEmailConfirmationLinkAsync(string userName, string email)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null) return ServiceResult.Error(ErrorMessages.UserNotFound);
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = $"{_configuration["JWT:ValidAudience"]}/{_configuration["EmailConfiguration:ConfirmEmailPath"]}" +
                        $"?token={Uri.EscapeDataString(token)}" +
                        $"&email={Uri.EscapeDataString(email)}";
                var body = $"<p>Hi {user.FullName}</p>" +
                        $"<p>Please confirm your email by clicking in the link below.</p>" +
                        $"<p><a href=\"{confirmationLink}\">Confirm</a></p>" +
                        $"<p>Thank you.</p>";
                var message = new EmailMessage(new string[] { email }, "[EMAIL CONFIRMATION LINK]", body);
                _backgroundTaskService.SendEmailAsync(message);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.InternalServerError("An error occurred while sending the email confirmation link.");
            }
        }

        public async Task<ServiceResult> ConfirmEmailAsync(string userName, EmailToken emailToken)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null) return ServiceResult.Error(ErrorMessages.UserNotFound);
                var result = await _userManager.ConfirmEmailAsync(user, emailToken.Token);
                if (result.Succeeded)
                {
                    user.Email = emailToken.Email;
                    await _userManager.UpdateAsync(user);
                    return ServiceResult.Success();
                }
                else
                    return ServiceResult.Error(ErrorMessages.InvalidToken);
            }
            catch (Exception ex)
            {
                return ServiceResult.InternalServerError("An error occurred while confirming the email.");
            }
        }

        public async Task<ServiceResult> UpdateProfileAsync(string userName, UpdateProfileDto profile)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userName);
                if(user == null) return ServiceResult.Error(ErrorMessages.UserNotFound);
                if(profile.Email != user.Email && user.EmailConfirmed) user.EmailConfirmed = false;
                _mapper.Map(profile, user);
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                    return ServiceResult<string>.Success("Profile updated successfully");
                return ServiceResult<string>.Error(ErrorMessages.UpdateProfileFailed);
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.InternalServerError($"{ErrorMessages.UpdateProfileFailed}: {ex.Message}");
            }
        }

        public async Task<ServiceResult> ForgotPasswordByEmailAsync(string email) 
        {             
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null) return ServiceResult.Error(ErrorMessages.UserNotFound);
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = $"{_configuration["JWT:ValidAudience"]}/{_configuration["EmailConfiguration:ResetPasswordPath"]}" +
                        $"?token={Uri.EscapeDataString(token)}" +
                        $"&email={Uri.EscapeDataString(email)}";
                var body = $"<p>Hi {user.FullName}</p>" +
                        $"<p>Please reset your password by clicking in the link below.</p>" +
                        $"<p><a href=\"{resetLink}\">Reset Password</a></p>" +
                        $"<p>Thank you.</p>";
                var message = new EmailMessage(new string[] { email }, "[PASSWORD RESET LINK]", body);
                _emailService.SendEmail(message);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.InternalServerError("An error occurred while sending the password reset link.");
            }
        }

        public async Task<ServiceResult> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(resetPasswordDto.UserName);
                if (user == null) return ServiceResult.Error(ErrorMessages.UserNotFound);
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);
                if (!result.Succeeded) 
                {
                    foreach (var error in result.Errors)
                    {
                        return ServiceResult.Error(error.Description);
                    }
                }
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.InternalServerError("An error occurred while resetting the password.");
            }
        }
        
        
        #region Private Methods
        private ServiceResult SendLoginOtpByEmail(ApplicationUser user, string code)
        {
            if (!user.EmailConfirmed)
            {
                return ServiceResult.Error(ErrorMessages.EmailNotConfirmed);
            }
            var body = $"<p>Hi {user.FullName}</p>" +
                $"<p>Please login with the OTP below.</p>" +
                $"<p><b>{code}</b></p>";
            var message = new EmailMessage(new string[] { user.Email }, "[LOGIN OTP]", body);
            _emailService.SendEmail(message);
            return ServiceResult.Success();
        }

        private async Task<Tuple<string,string>> GenerateTokenAsync(ApplicationUser user, string createdByIp)
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
        #endregion
    }
}
