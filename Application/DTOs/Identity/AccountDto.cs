namespace Application.DTOs.Identity
{
    public class AccountDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string? Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public IEnumerable<RoleDto>? Roles { get; set; }
    }
}
