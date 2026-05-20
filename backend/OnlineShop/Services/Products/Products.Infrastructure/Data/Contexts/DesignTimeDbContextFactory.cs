using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Products.Infrastructure.Data.Contexts
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // 1. Get parent directory (solution-level navigation)
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Products.Api");

            // 2. Build configuration from API project (or parent level)
            var config = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // 3. Get connection string
            var connectionString = config.GetConnectionString("OrderConnectionString");

            // 4. Build DbContext options
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}