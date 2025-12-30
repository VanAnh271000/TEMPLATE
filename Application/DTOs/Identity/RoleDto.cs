namespace Application.DTOs.Identity
{
    public class CreateRoleDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<int> PermissionIds { get; set; }
        public List<string> AccountIds { get; set; }
    }
    public class RoleDto
    {
        public string Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IEnumerable<PermissionDto>? Permissions { get; set; }
        public IEnumerable<AccountDto>? Accounts { get; set; }
    }
}
