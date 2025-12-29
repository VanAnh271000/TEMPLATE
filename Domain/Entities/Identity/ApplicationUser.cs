using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.Identity
{
    public partial class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
