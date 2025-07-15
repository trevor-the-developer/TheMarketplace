using Microsoft.Data.SqlClient;

namespace Marketplace.Test.Helpers;

public static class DatabaseConnectionHelper
{
    private const string Server = "127.0.0.1,1433";
    private const string UserId = "sa";
    private const string Password = "P@ssw0rd!";

    public static string GetConnectionString(string databaseName)
    {
        var builder = new SqlConnectionStringBuilder
        {
            DataSource = Server,
            InitialCatalog = databaseName,
            UserID = UserId,
            Password = Password,
            TrustServerCertificate = true,
            ConnectTimeout = 30,
            CommandTimeout = 30
        };

        return builder.ConnectionString;
    }

    public static string GetMasterConnectionString()
    {
        return GetConnectionString("master");
    }

    public static string GetMarketplaceConnectionString()
    {
        return GetConnectionString("Marketplace");
    }

    // Optional: Method to create a SqlConnection directly
    public static SqlConnection CreateConnection(string databaseName)
    {
        return new SqlConnection(GetConnectionString(databaseName));
    }

    // Optional: Method for testing connection
    public static async Task<bool> TestConnectionAsync(string databaseName)
    {
        try
        {
            await using var connection = CreateConnection(databaseName);
            await connection.OpenAsync();
            await using var command = new SqlCommand("SELECT 1", connection);
            await command.ExecuteScalarAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}