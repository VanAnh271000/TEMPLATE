using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Infrastructure.Factories
{
    public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(
                    "Data Source=112.213.91.17;Initial Catalog=Template;Persist Security Info=True;User ID=sa;Password=Vas123456;Encrypt=False;MultipleActiveResultSets=True")
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
