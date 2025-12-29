namespace Domain.Entities.Identity
{
    public class UserRole
    {
        public string UserId { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual ApplicationRole Role { get; set; } = null!;
    }
}
