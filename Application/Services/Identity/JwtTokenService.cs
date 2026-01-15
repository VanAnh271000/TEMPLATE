using Application.Interfaces.Commons;
using Application.Interfaces.Services.Identity;
using Domain.Entities.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application.Services.Identity
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<RefreshToken, int> _refreshTokenRepository;
        public JwtTokenService(IConfiguration configuration, IGenericRepository<RefreshToken, int> refreshTokenRepository)
        {
            _configuration = configuration;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public string GenerateAccessToken(ApplicationUser user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Name, user.UserName ?? ""),
                new("fullname", user.FullName),
                new("token_type", "access_token")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:TokenValidityInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public void RevokeExpireRefreshToken(string userId)
        {
            _refreshTokenRepository.DeleteMulti(rt => rt.UserId == userId && !rt.IsActive && rt.ExpiryDate < DateTime.UtcNow);
        }

        //public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        //{
        //    var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]);
        //    var tokenValidationParameters = new TokenValidationParameters
        //    {
        //        ValidateAudience = true,
        //        ValidateIssuer = true,
        //        ValidateIssuerSigningKey = true,
        //        IssuerSigningKey = new SymmetricSecurityKey(key),
        //        ValidateLifetime = false,
        //        ValidIssuer = _configuration["JWT:ValidIssuer"],
        //        ValidAudience = _configuration["JWT:ValidAudience"],
        //        ClockSkew = TimeSpan.Zero
        //    };

        //    var tokenHandler = new JwtSecurityTokenHandler();

        //    try
        //    {
        //        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

        //        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
        //            !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            throw new SecurityTokenException(ErrorMessages.InvalidToken);
        //        }

        //        // Additional validation: check if token type is access_token
        //        var tokenTypeClaim = principal.FindFirst("token_type");
        //        if (tokenTypeClaim?.Value != "access_token")
        //        {
        //            throw new SecurityTokenException(ErrorMessages.InvalidToken);
        //        }

        //        return principal;
        //    }
        //    catch (Exception)
        //    {
        //        throw new SecurityTokenException(ErrorMessages.InvalidToken);
        //    }
        //}

        //public bool ValidateToken(string token)
        //{
        //    try
        //    {
        //        var key = Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]);
        //        var tokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateAudience = true,
        //            ValidateIssuer = true,
        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = new SymmetricSecurityKey(key),
        //            ValidateLifetime = true,
        //            ValidIssuer = _configuration["JWT:ValidIssuer"],
        //            ValidAudience = _configuration["JWT:ValidAudience"],
        //            ClockSkew = TimeSpan.Zero
        //        };

        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken validatedToken);

        //        return true;
        //    }
        //    catch
        //    {
        //        return false;
        //    }
        //}

        //public DateTime GetTokenExpirationDate(string token)
        //{
        //    var tokenHandler = new JwtSecurityTokenHandler();
        //    var jwtToken = tokenHandler.ReadJwtToken(token);
        //    return jwtToken.ValidTo;
        //}

        //public string GetUserIdFromToken(string token)
        //{
        //    try
        //    {
        //        var tokenHandler = new JwtSecurityTokenHandler();
        //        var jwtToken = tokenHandler.ReadJwtToken(token);
        //        return jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
        //    }
        //    catch
        //    {
        //        return "";
        //    }
        //}

        //public int GetAccessTokenExpirationMinutes()
        //{
        //    return Convert.ToInt32(_configuration["JWT:TokenValidityInMinutes"] ?? "60");
        //}

        //public TimeSpan GetRemainingTokenLifetime(string token)
        //{
        //    try
        //    {
        //        var expirationDate = GetTokenExpirationDate(token);
        //        var remainingTime = expirationDate - DateTime.UtcNow;
        //        return remainingTime > TimeSpan.Zero ? remainingTime : TimeSpan.Zero;
        //    }
        //    catch
        //    {
        //        return TimeSpan.Zero;
        //    }
        //}

    }
}
