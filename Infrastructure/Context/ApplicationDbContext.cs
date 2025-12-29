using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            SeedData.SeedPermission(modelBuilder);
        }
        
        public virtual DbSet<RefreshToken> RefreshToken { get; set; }
        public virtual DbSet<RolePermission> RolePermission { set; get; }
        public virtual DbSet<Permission> Permission { set; get; }
        public virtual DbSet<UserRole> UserRole { set; get; }
    }
}
