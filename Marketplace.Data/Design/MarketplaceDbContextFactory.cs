using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Marketplace.Data.Design;

public class MarketplaceDbContextFactory : IDesignTimeDbContextFactory<MarketplaceDbContext>
{
    public MarketplaceDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Marketplace.Api"))
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .AddEnvironmentVariables()
            .Build();

        var builder = new DbContextOptionsBuilder<MarketplaceDbContext>();
        var connectionString = configuration.GetConnectionString("MarketplaceDbConnection");

        builder.UseSqlServer(connectionString);

        return new MarketplaceDbContext(builder.Options);
    }
}