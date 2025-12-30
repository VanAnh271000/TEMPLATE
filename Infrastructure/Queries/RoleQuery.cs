using Application.DTOs.Commons;
using Application.DTOs.Identity;
using Application.Interfaces.Queries;
using Infrastructure.Context;
using Shared.Extensions;
using Shared.Results;

namespace Infrastructure.Queries
{
    public class RoleQuery : IRoleQuery
    {
        private readonly ApplicationDbContext _context;

        public RoleQuery(ApplicationDbContext context)
        {
            _context = context;
        }
        public PagedResult<RoleDto> GetListRoleAccounts(CommonQueryParameters parameters, params string[] searchProperties)
        {
            var query = (from role in _context.ApplicationRole
                         join userRole in _context.UserRole on role.Id equals userRole.RoleId
                         join user in _context.Users on userRole.UserId equals user.Id
                         where user.IsActive == true
                         join rolePermission in _context.RolePermission on role.Id equals rolePermission.RoleId
                         join permission in _context.Permission on rolePermission.PermissionId equals permission.Id
                         select new
                         {
                             user.Id,
                             user.FullName,
                             user.UserName,
                             RoleName = role.Name,
                             RoleId = role.Id,
                             RoleDescription = role.Description,
                             PermissionId = permission.Id,
                             permissionModule = permission.Module,
                             permissionDescription = permission.Description
                         })
                        .GroupBy(x => new
                        {
                            x.RoleName,
                            x.RoleId,
                            x.RoleDescription
                        })
                        .Select(ru => new RoleDto
                        {
                            Id = ru.Key.RoleId,
                            Name = ru.Key.RoleName,
                            Description = ru.Key.RoleDescription,
                            Accounts = ru
                            .Select(u => new AccountDto
                            {
                                Id = u.Id,
                                FullName = u.FullName,
                                UserName = u.UserName
                            }).Distinct(),
                            Permissions = ru.Select(p => new PermissionDto
                            {
                                Id = p.PermissionId,
                                Module = p.permissionModule,
                                Description = p.permissionDescription
                            }).Distinct()
                        });

            // Apply search
            query = query.ApplySearch(parameters.Search, searchProperties);

            // Apply sorting
            query = query.ApplySorting(parameters.SortBy, parameters.SortDirection);

            // Apply paging
            return query.ToPagedResult(parameters.Index, parameters.PageSize);
        }
    }
}
