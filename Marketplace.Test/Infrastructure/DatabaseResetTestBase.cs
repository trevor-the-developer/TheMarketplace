using System.Threading.Tasks;
using Xunit;

namespace Marketplace.Test.Infrastructure;

/// <summary>
///     Base class for tests that need database reset functionality.
///     This provides methods to reset the database state before/after tests.
/// </summary>
public abstract class DatabaseResetTestBase : IAsyncLifetime
{
    /// <summary>
    ///     Called before each test method. Override to add custom initialization.
    /// </summary>
    public virtual async Task InitializeAsync()
    {
        await ResetDatabaseAsync();
    }

    /// <summary>
    ///     Called after each test method. Override to add custom cleanup.
    /// </summary>
    public virtual async Task DisposeAsync()
    {
        // Default implementation does nothing
        // Override if you need cleanup after each test
        await Task.CompletedTask;
    }

    /// <summary>
    ///     Reset the database to a clean state with fresh seed data.
    /// </summary>
    protected static async Task ResetDatabaseAsync()
    {
        await DatabaseResetService.ResetDatabaseAsync();
    }
}