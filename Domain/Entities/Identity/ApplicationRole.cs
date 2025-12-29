using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity
{
    public partial class ApplicationRole : IdentityRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; }
        public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
        public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    }
}
