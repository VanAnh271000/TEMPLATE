using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Factories
{
    public sealed class ApplicationDbContextFactory
    : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(
                    "Server=localhost;Database=YourAppDb;Trusted_Connection=True;TrustServerCertificate=True")
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
