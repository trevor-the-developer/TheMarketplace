using System.Diagnostics;
using Marketplace.Data;
using Marketplace.Test.Helpers;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Marketplace.Test.Infrastructure;

public class DatabaseTestFixture : IAsyncLifetime
{
    private const string ContainerName = "marketplace-db";
    private const string DatabaseName = "Marketplace";

    private static readonly SemaphoreSlim InitializationSemaphore = new(1, 1);
    private static bool _isInitialized;

    public async Task InitializeAsync()
    {
        await InitializationSemaphore.WaitAsync();
        try
        {
            if (_isInitialized)
            {
                Console.WriteLine("Database test fixture already initialized, skipping...");
                return;
            }

            Console.WriteLine("Starting database test fixture initialization...");
            await EnsureDockerContainerIsRunningAsync();
            Console.WriteLine("Database container is ready, ensuring database exists...");
            await EnsureDatabaseExistsAsync();
            Console.WriteLine("Database exists, running migrations...");
            await RunMigrationsAsync();
            Console.WriteLine("Database test fixture initialization complete.");
            
            _isInitialized = true;
        }
        finally
        {
            InitializationSemaphore.Release();
        }
    }

    public Task DisposeAsync()
    {
        // We don't stop the container here as other tests might still be running
        return Task.CompletedTask;
    }

    private static async Task<bool> TryConnectToSqlServerAsync()
    {
        try
        {
            await using var connection = DatabaseConnectionHelper.CreateConnection(DatabaseName);
            await connection.OpenAsync();
            await using var command = new SqlCommand("SELECT 1", connection);
            await command.ExecuteScalarAsync();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to connect to SQL Server: {ex.Message}");
            Console.WriteLine($"Connection string used: {DatabaseConnectionHelper.GetMarketplaceConnectionString()}");
            return false;
        }
    }

    private static async Task EnsureDockerContainerIsRunningAsync()
    {
        // First, try to connect directly to SQL Server
        // If it works, we don't need to do anything with Docker
        if (await TryConnectToSqlServerAsync())
        {
            Console.WriteLine("SQL Server is already accessible, no need to start container.");
            return;
        }

        // Check if container is running
        var checkProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = $"ps -q -f name={ContainerName}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        checkProcess.Start();
        var output = await checkProcess.StandardOutput.ReadToEndAsync();
        await checkProcess.WaitForExitAsync();

        if (string.IsNullOrWhiteSpace(output))
        {
            Console.WriteLine($"Container {ContainerName} is not running, attempting to start it...");
            await StartDockerContainerAsync();
        }
        else
        {
            Console.WriteLine($"Container {ContainerName} is already running with ID: {output.Trim()}");
        }

        // Wait for SQL Server to be ready
        await WaitForSqlServerAsync();
    }

    private static async Task StartDockerContainerAsync()
    {
        // Check if container exists but is stopped
        var existsProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "docker",
                Arguments = $"ps -aq -f name={ContainerName}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        existsProcess.Start();
        var existsOutput = await existsProcess.StandardOutput.ReadToEndAsync();
        await existsProcess.WaitForExitAsync();

        if (!string.IsNullOrWhiteSpace(existsOutput))
        {
            // Container exists, start it
            var startProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"start {ContainerName}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            startProcess.Start();
            await startProcess.WaitForExitAsync();
        }
        else
        {
            // Container doesn't exist, run docker-compose
            var composeProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "docker-compose",
                    Arguments = "up -d",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            composeProcess.Start();
            await composeProcess.WaitForExitAsync();
        }
    }

    private static async Task WaitForSqlServerAsync()
    {
        const int maxAttempts = 15;
        var attempt = 0;
        var lastError = string.Empty;

        while (attempt < maxAttempts)
        {
            try
            {
                // Use .NET connection instead of sqlcmd since that's what actually works
                await using var connection = DatabaseConnectionHelper.CreateConnection("master");
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                await connection.OpenAsync(cts.Token);

                await using var command = new SqlCommand("SELECT 1", connection);
                await command.ExecuteScalarAsync(cts.Token);
                
                Console.WriteLine("SQL Server is ready!");
                return; // SQL Server is ready
            }
            catch (Exception ex)
            {
                lastError = ex.Message;
                Console.WriteLine($"SQL Server not ready (attempt {attempt + 1}/{maxAttempts}): {lastError}");
            }

            attempt++;
            await Task.Delay(2000); // Wait 2 seconds before next attempt
        }

        throw new InvalidOperationException(
            $"SQL Server container did not start within expected time. Last error: {lastError}");
    }

    private static async Task EnsureDatabaseExistsAsync()
    {
        try
        {
            await using var connection = DatabaseConnectionHelper.CreateConnection("master");
            await connection.OpenAsync();

            const string createDbSql = $"""
                                        IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{DatabaseName}') 
                                                                        CREATE DATABASE {DatabaseName}
                                        """;

            await using var command = new SqlCommand(createDbSql, connection);
            await command.ExecuteNonQueryAsync();

            Console.WriteLine($"Database {DatabaseName} is ready.");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create database: {ex.Message}", ex);
        }
    }

    private static async Task RunMigrationsAsync()
    {
        try
        {
            // Create a temporary service provider to run migrations
            var services = new ServiceCollection();
            services.AddDbContext<MarketplaceDbContext>(options =>
                options.UseSqlServer(DatabaseConnectionHelper.GetMarketplaceConnectionString()));
            services.AddLogging(builder => builder.AddConsole());

            await using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();

            // Ensure database is created and migrated
            await context.Database.EnsureDeletedAsync();

            // Apply migrations to create schema and seed data
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            // If EF migrations fail, we'll try running them via dotnet ef command
            Console.WriteLine($"EF migrations failed: {ex.Message}. Trying CLI approach...");
            await RunMigrationsViaCliAsync();
        }
    }

    private static async Task RunMigrationsViaCliAsync()
    {
        try
        {
            var migrateProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dotnet",
                    Arguments = "ef database update --project Marketplace.Data --startup-project Marketplace.Api",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            migrateProcess.Start();
            await migrateProcess.WaitForExitAsync();

            if (migrateProcess.ExitCode != 0)
            {
                var error = await migrateProcess.StandardError.ReadToEndAsync();
                Console.WriteLine($"Migration warning (might be expected): {error}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to run migrations via CLI: {ex.Message}");
        }
    }
}