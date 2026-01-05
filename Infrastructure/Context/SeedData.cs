using Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Infrastructure.Context
{
    public static class SeedData
    {
        public static readonly string AdminRoleId = "11111111-1111-1111-1111-111111111111";
        public static void SeedPermission(ModelBuilder builder)
        {
            
            builder.Entity<Permission>().HasData(
                //User
                new Permission { Id = 1, Name = "user.read", Resource = "user", Module = "User", Action = "read", Description = "Get user" },
                new Permission { Id = 2, Name = "user.write", Resource = "user", Module = "User", Action = "write", Description = "Create/ update user" },
                new Permission { Id = 3, Name = "user.delete", Resource = "user", Module = "User", Action = "delete", Description = "Delete user" }

            );

            // Seed Roles
            builder.Entity<ApplicationRole>().HasData(
                new ApplicationRole { Id = AdminRoleId, Name = "Admin", Description = "Full system access" }
            );

            // Seed Role Permissions
            builder.Entity<RolePermission>().HasData(
                new RolePermission
                {
                    RoleId = AdminRoleId,
                    PermissionId = 1
                }
            );
            builder.Entity<RolePermission>().HasData(
                new RolePermission
                {
                    RoleId = AdminRoleId,
                    PermissionId = 2
                }
            );
            builder.Entity<RolePermission>().HasData(
                new RolePermission
                {
                    RoleId = AdminRoleId,
                    PermissionId = 3
                }
            );
        }
    }
}
