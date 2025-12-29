namespace Domain.Entities.Identity
{
    public class RolePermission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }
        public DateTime? AssignedAt { get; set; }
        public virtual ApplicationRole Role { get; set; } = null!;
        public virtual Permission Permission { get; set; } = null!;
    }
}
