using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Factories
{
    public sealed class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var environment =
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            ?? "Development";
            var basePath = Path.GetFullPath(Path.Combine(
                Directory.GetCurrentDirectory(),
                "..",
                "API"
            ));
            Console.WriteLine($"basePath = {basePath}");
            
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true)
                .AddUserSecrets<ApplicationDbContextFactory>(optional: true)
                .AddEnvironmentVariables()
                .Build();
            Console.WriteLine("Secrets loaded:");
            foreach (var kv in configuration.AsEnumerable())
            {
                if (kv.Key.Contains("ConnectionStrings"))
                    Console.WriteLine($"{kv.Key} = {kv.Value}");
            }
            var connectionString =
                configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine($"ConnectionString = {connectionString}");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Connection string not found.");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(connectionString)
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
