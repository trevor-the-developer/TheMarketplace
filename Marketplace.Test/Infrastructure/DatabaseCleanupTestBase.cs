using System;
using System.Threading.Tasks;
using Alba;
using Marketplace.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Marketplace.Test.Infrastructure;

[Collection("scenarios")]
public abstract class DatabaseCleanupTestBase : IAsyncLifetime
{
    protected readonly WebAppFixture _fixture;

    protected DatabaseCleanupTestBase(WebAppFixture fixture)
    {
        _fixture = fixture;
    }

    protected IAlbaHost Host => _fixture.AlbaHost!;

    public virtual Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public virtual async Task DisposeAsync()
    {
        // Clean up database after each test
        await CleanupDatabaseAsync();
    }

    private async Task CleanupDatabaseAsync()
    {
        try
        {
            // Get the database context from the Alba host
            using var scope = _fixture.AlbaHost!.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();

            // Delete all test data in reverse order to handle foreign key constraints
            await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUserRoles]");
            await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetUsers]");
            await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [AspNetRoles] WHERE [Name] = 'User'");

            // You can add more cleanup statements here for other tables as needed
            // Example:
            // await dbContext.Database.ExecuteSqlRawAsync("DELETE FROM [YourOtherTable]");

            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log cleanup errors but don't fail tests
            Console.WriteLine($"Database cleanup failed: {ex.Message}");
        }
    }
}