using Domain.Entities.Identity;
using System.Security.Claims;

namespace Application.Interfaces.Services.Identity
{
    public interface IJwtTokenService
    {
        string GenerateAccessToken(ApplicationUser user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        bool ValidateToken(string token);
        DateTime GetTokenExpirationDate(string token);
        string GetUserIdFromToken(string token);
        int GetAccessTokenExpirationMinutes();
        TimeSpan GetRemainingTokenLifetime(string token);
        void RevokeExpireRefreshToken(string userId);
    }
}
