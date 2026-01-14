using Application.Interfaces.Services.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Application.Services.Identity
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionAuthorizationHandler(
            IPermissionService permissionService,
            IHttpContextAccessor httpContextAccessor)
        {
            _permissionService = permissionService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                context.Fail();
                return;
            }

            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                .FirstOrDefault()?.Split(" ").Last();

            if (token == null)
            {
                context.Fail();
                return;
            }

            var hasPermission = _permissionService.HasPermissionAsync(userId, requirement.Permission);

            if (hasPermission.Data)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }
        }
    }
}
