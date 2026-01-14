using Application.Interfaces.Commons;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Services.Commons
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            

        public string? FullName => _httpContextAccessor.HttpContext?.User?.FindFirstValue("fullname");
    }
}
