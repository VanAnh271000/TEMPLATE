using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity
{
    public class UserRole : IdentityUserRole<string>
    {
        //public string UserId { get; set; } = string.Empty;
        //public string RoleId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ApplicationRole Role { get; set; } = null!;
    }
}
