using Application.Interfaces.Commons;
using Domain.Commons;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<
        ApplicationUser,
        ApplicationRole,
        string,
        IdentityUserClaim<string>,
        UserRole,
        IdentityUserLogin<string>,
        IdentityRoleClaim<string>,
        IdentityUserToken<string>>
    {
        private readonly ICurrentUserService? _currentUserService;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService? currentUserService = null)
            : base(options) {
            _currentUserService = currentUserService;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IAuditableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var isDeletedProp = Expression.Property(parameter, nameof(IAuditableEntity.IsDeleted));
                    var notDeleted = Expression.Not(isDeletedProp);
                    var lambda = Expression.Lambda(notDeleted, parameter);

                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            SeedData.SeedPermission(modelBuilder);
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var userId = _currentUserService.UserId;
            var userName = _currentUserService.FullName;
            // Set audit fields
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IAuditableEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified || e.State == EntityState.Deleted));

            foreach (var entityEntry in entries)
            {
                var entity = (IAuditableEntity)entityEntry.Entity;

                if (entityEntry.State == EntityState.Added)
                {
                    if (entity.CreatedTime == null)
                        entity.CreatedTime = DateTime.UtcNow;
                    entity.CreatedById = userId;
                    entity.CreatedByName = userName;
                    entity.UpdatedTime = DateTime.UtcNow;
                    entity.UpdatedById = userId;
                    entity.UpdatedByName = userName;
                }
                else if (entityEntry.State == EntityState.Modified)
                {
                    entity.UpdatedTime = DateTime.UtcNow;
                    entity.UpdatedById = userId;
                    entity.UpdatedByName = userName;
                }
                else if (entityEntry.State == EntityState.Deleted)
                {
                    entityEntry.State = EntityState.Modified;
                    entity.IsDeleted = true;
                    entity.DeletedTime = DateTime.UtcNow;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public virtual DbSet<ApplicationRole> ApplicationRole { get; set; }
        public virtual DbSet<RefreshToken> RefreshToken { get; set; }
        public virtual DbSet<RolePermission> RolePermission { set; get; }
        public virtual DbSet<Permission> Permission { set; get; }
        public virtual DbSet<UserRole> UserRole { set; get; }
    }
}
