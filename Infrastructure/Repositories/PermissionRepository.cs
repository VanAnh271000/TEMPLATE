using Application.Interfaces.Repositories;
using Domain.Entities.Identity;
using Infrastructure.Context;

namespace Infrastructure.Repositories
{
    public class PermissionRepository : GenericRepository<Permission, int>, IPermissionRepository
    {
        public PermissionRepository(ApplicationDbContext context) : base(context)
        {
        }

        public List<string> GetUserPermissions(string userId)
        {
            var permissions = _context.UserRole
                .Where(ur => ur.UserId == userId)
                .Join(_context.RolePermission, ur => ur.RoleId, rp => rp.RoleId, (ur, rp) => rp)
                .Join(_context.Permission, rp => rp.PermissionId, p => p.Id, (rp, p) => p.Name)
                .Distinct()
                .ToList();
            return permissions;
        }

        public bool HasPermission(string userId, string permission)
        {
            var hasPermission = _context.UserRole
                .Where(ur => ur.UserId == userId)
                .Join(_context.RolePermission, ur => ur.RoleId, rp => rp.RoleId, (ur, rp) => rp)
                .Join(_context.Permission, rp => rp.PermissionId, p => p.Id, (rp, p) => p)
                .Any(p => p.Name == permission);

            return hasPermission;
        }
    }
}
