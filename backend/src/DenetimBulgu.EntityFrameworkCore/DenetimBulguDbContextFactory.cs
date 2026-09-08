using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DenetimBulgu.EntityFrameworkCore
{
    public class DenetimBulguDbContextFactory : IDesignTimeDbContextFactory<DenetimBulguDbContext>
    {
        public DenetimBulguDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../DenetimBulgu.Web.Host"))
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var connStr = configuration.GetConnectionString("Default");
            var builder = new DbContextOptionsBuilder();
            builder.UseNpgsql(connStr);
            return new DenetimBulguDbContext(builder.Options);
        }
    }
}
