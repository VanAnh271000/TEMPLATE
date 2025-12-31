using API.Controllers.Commons;
using Application.DTOs.Identity;
using Application.Interfaces.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {
        private readonly IAuthenticationService _authService;
        public AuthController(IAuthenticationService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var result = await _authService.LoginAsync(request);
            return HandleServiceResult(result);
        }

        [HttpPost("Login2FA")]
        public async Task<IActionResult> LoginWithOTP([FromBody] LoginOtpDto loginOtp)
        {
            var result = await _authService.Login2FaAsync(loginOtp);
            return HandleServiceResult(result);
        }

        [HttpPost("ResendLoginOtp/{userName}")]
        public async Task<IActionResult> ResendLoginOtp(string userName)
        {
            var result = await _authService.ResendLoginOTPAsync(userName);
            return HandleServiceResult(result);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] CreateAccountDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(request);

            return HandleServiceResult(result);
        }

        [HttpPut("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
        {
            var result = await _authService.ChangePasswordAsync(request, User.Identity.Name);
            return HandleServiceResult(result);
        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            return HandleServiceResult(result);
        }

        [HttpPost]
        [Authorize]
        [Route("SendEmailConfirmationLink/{email}")]
        public async Task<IActionResult> SendEmailConfirmationLink(string email)
        {
            var result = await _authService.SendEmailConfirmationLinkAsync(User.Identity.Name, email);
            return HandleServiceResult(result);
        }

        [HttpPut("ConfirmEmail")]
        [Authorize]
        public async Task<IActionResult> ConfirmEmail([FromBody] EmailToken emailToken)
        {
            var result = await _authService.ConfirmEmailAsync(User.Identity.Name, emailToken);
            return HandleServiceResult(result);
        }

        [HttpGet("Me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var result = await _authService.GetCurrentUserAsync(User.Identity.Name);
            return HandleServiceResult(result);
        }

        [HttpPut("UpdateProfile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto request)
        {
            var result = await _authService.UpdateProfileAsync(User.Identity.Name, request);
            return HandleServiceResult(result);
        }

        [HttpPost("ForgotPasswordByEmail/{email}")]
        public async Task<IActionResult> ForgotPasswordByEmail(string email)
        {
            var result = await _authService.ForgotPasswordByEmailAsync(email);
            return HandleServiceResult(result);
        }

        [HttpPost]
        [Route("ResetPassword")]
        [Authorize(Policy = "CreateAccount")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPassword)
        {
            var result = await _authService.ResetPasswordAsync(resetPassword);
            return HandleServiceResult(result);
        }

    }
}
