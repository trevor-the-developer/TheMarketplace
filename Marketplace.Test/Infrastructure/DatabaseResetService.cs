using Marketplace.Data;
using Marketplace.Data.Entities;
using Marketplace.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Marketplace.Test.Infrastructure;

public static class DatabaseResetService
{
    private const string ConnectionString =
        "Server=127.0.0.1,1433;Database=Marketplace;User Id=sa;Password=P@ssw0rd!;Trust Server Certificate=True";

    public static async Task ResetDatabaseAsync()
    {
        try
        {
            // Create a temporary service provider to reset the database
            var services = new ServiceCollection();
            services.AddDbContext<MarketplaceDbContext>(options =>
                options.UseSqlServer(ConnectionString));
            services.AddLogging(builder => builder.AddConsole());

            await using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<MarketplaceDbContext>();

        // Delete all data but keep the schema
        await ClearAllDataAsync(context);
        
        // Re-seed the database with fresh data
        await SeedDatabaseAsync(context);
            
            Console.WriteLine("Database reset completed successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Database reset failed: {ex.Message}");
            throw;
        }
    }

    private static async Task ClearAllDataAsync(MarketplaceDbContext context)
    {
        // Disable foreign key constraints temporarily
        await context.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'");

        // Clear all data from tables in the correct order to avoid foreign key violations
        var tablesToClear = new[]
        {
            "AspNetUserClaims",
            "AspNetUserLogins", 
            "AspNetUserRoles",
            "AspNetUserTokens",
            "AspNetRoleClaims",
            "Documents",
            "Files",
            "ProductDetails",
            "Products",
            "Cards",
            "Listings",
            "Profiles",
            "Tags",
            "AspNetUsers",
            "AspNetRoles"
        };

        foreach (var table in tablesToClear)
        {
            try
            {
                // These are hardcoded table names from a predefined list, so they're safe
#pragma warning disable EF1002 // Table names cannot be parameterized and are from a safe predefined list
                await context.Database.ExecuteSqlRawAsync($"DELETE FROM [{table}]");
                
                // Reset identity columns if they exist
                await context.Database.ExecuteSqlRawAsync($@"
                    IF EXISTS (SELECT 1 FROM sys.identity_columns WHERE object_id = OBJECT_ID('[{table}]'))
                    BEGIN
                        DBCC CHECKIDENT('[{table}]', RESEED, 0)
                    END");
#pragma warning restore EF1002
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not clear table {table}: {ex.Message}");
                // Continue with other tables
            }
        }

        // Clear Wolverine tables if they exist
        var wolverineTables = new[]
        {
            "wolverine.wolverine_control_queue",
            "wolverine.wolverine_dead_letters",
            "wolverine.wolverine_incoming_envelopes",
            "wolverine.wolverine_outgoing_envelopes",
            "wolverine.wolverine_node_assignments",
            "wolverine.wolverine_node_records",
            "wolverine.wolverine_nodes"
        };

        foreach (var table in wolverineTables)
        {
            try
            {
                // These are hardcoded table names from a predefined list, so they're safe
#pragma warning disable EF1002 // Table names cannot be parameterized and are from a safe predefined list
                await context.Database.ExecuteSqlRawAsync($"IF OBJECT_ID('{table}', 'U') IS NOT NULL DELETE FROM [{table}]");
#pragma warning restore EF1002
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Warning: Could not clear Wolverine table {table}: {ex.Message}");
                // Continue with other tables
            }
        }

        // Re-enable foreign key constraints
        await context.Database.ExecuteSqlRawAsync("EXEC sp_MSforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT ALL'");
    }

    private static async Task SeedDatabaseAsync(MarketplaceDbContext context)
    {
        // Add roles
        context.Roles.AddRange(new List<IdentityRole>
        {
            new() {Id = "00917cdb-f5b0-4c84-9172-ff5b72ff8500", Name = "Administrator", NormalizedName = "ADMINISTRATOR"},
            new() {Id = "e23ba8c8-b3ae-4e81-b468-c269c6e35cf2", Name = "User", NormalizedName = "USER"}
        });

        // Add users
        context.Users.AddRange(new List<ApplicationUser>
        {
            new()
            {
                Id = "69a38a69-e24d-4c7f-bdf2-c7bc2222cbe7",
                Email = "demouser@localhost",
                EmailConfirmed = true,
                NormalizedEmail = "DEMOUSER@LOCALHOST",
                NormalizedUserName = "DEMOUSER@LOCALHOST",
                SecurityStamp = "db7f09be-d39a-4b27-b9dd-6b7093cda243",
                ConcurrencyStamp = "ab592486-cce9-4489-acf1-414c93f8363a",
                UserName = "demouser@localhost",
                FirstName = "Demo",
                LastName = "User",
                Role = Role.User,
                PasswordHash = "AQAAAAIAAYagAAAAELfj5utVmWzewI/tzmjx5Y2Db/sKtTvYMg9CXc/4cEo0C/kwjBG0u/jPD6P8KnefOA=="
            },
            new()
            {
                Id = "a5ac5ebb-5f11-4363-a58d-4362d8ff6863",
                Email = "admin@localhost",
                EmailConfirmed = true,
                NormalizedEmail = "ADMIN@LOCALHOST",
                NormalizedUserName = "ADMIN@LOCALHOST",
                SecurityStamp = "16147ec8-bcfa-4379-b4f5-3bb4f249a70c",
                ConcurrencyStamp = "12bb4704-e6d8-4a58-9a0f-10cf65e7421e",
                UserName = "admin@localhost",
                FirstName = "System",
                LastName = "Administrator",
                Role = Role.Adminstrator,
                PasswordHash = "AQAAAAIAAYagAAAAEOEoVFr1wD4qNm8MKtyLOnadMhQW5m+pufGtf8eWP9zwGK6eIIMwSkpi/jKC3ZjhsA=="
            }
        });

        await context.SaveChangesAsync();

        // Add user roles
        context.UserRoles.AddRange(new List<IdentityUserRole<string>>
        {
            new() { UserId = "69a38a69-e24d-4c7f-bdf2-c7bc2222cbe7", RoleId = "e23ba8c8-b3ae-4e81-b468-c269c6e35cf2" },
            new() { UserId = "a5ac5ebb-5f11-4363-a58d-4362d8ff6863", RoleId = "00917cdb-f5b0-4c84-9172-ff5b72ff8500" }
        });

        await context.SaveChangesAsync();

        // Add sample data for testing
        context.Listings.AddRange(new List<Listing>
        {
            new()
            {
                Title = "Sample Listing", 
                Description = "Sample listing for testing",
                CreatedBy = "admin@localhost",
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = "admin@localhost",
                ModifiedDate = DateTime.UtcNow
            }
        });

        await context.SaveChangesAsync();
        
        // Add sample cards for testing
        context.Cards.AddRange(new List<Card>
        {
            new()
            {
                Title = "Sample Card",
                Description = "Sample card for testing",
                IsEnabled = true,
                Colour = "#007bff",
                ListingId = 1,
                CreatedBy = "admin@localhost",
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = "admin@localhost",
                ModifiedDate = DateTime.UtcNow
            },
            new()
            {
                Title = "Sample Card 2",
                Description = "Second sample card for testing",
                IsEnabled = true,
                Colour = "#28a745",
                ListingId = 1,
                CreatedBy = "admin@localhost",
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = "admin@localhost",
                ModifiedDate = DateTime.UtcNow
            }
        });

        await context.SaveChangesAsync();
        
        // Add sample products for testing
        context.Products.AddRange(new List<Product>
        {
            new()
            {
                Title = "Sample Product",
                Description = "Sample product for testing",
                ProductType = "Sample Type",
                Category = "Sample Category",
                IsEnabled = true,
                IsDeleted = false,
                CardId = 1,
                CreatedBy = "admin@localhost",
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = "admin@localhost",
                ModifiedDate = DateTime.UtcNow
            },
            new()
            {
                Title = "Sample Product 2",
                Description = "Second sample product for testing",
                ProductType = "Sample Type 2",
                Category = "Sample Category 2",
                IsEnabled = true,
                IsDeleted = false,
                CardId = 2,
                CreatedBy = "admin@localhost",
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = "admin@localhost",
                ModifiedDate = DateTime.UtcNow
            }
        });

        await context.SaveChangesAsync();
        
        // Add sample tags for testing
        context.Tags.AddRange(new List<Tag>
        {
            new()
            {
                Name = "Sample Tag",
                Description = "Sample tag for testing",
                IsEnabled = true,
                CreatedBy = "admin@localhost",
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = "admin@localhost",
                ModifiedDate = DateTime.UtcNow
            },
            new()
            {
                Name = "Sample Tag 2",
                Description = "Second sample tag for testing",
                IsEnabled = true,
                CreatedBy = "admin@localhost",
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = "admin@localhost",
                ModifiedDate = DateTime.UtcNow
            }
        });

        await context.SaveChangesAsync();
        
        // Add sample product details for testing
        context.ProductDetails.AddRange(new List<ProductDetail>
        {
            new()
            {
                Title = "Sample Product Detail",
                Description = "Sample product detail for testing",
                ProductId = 1,
                CreatedBy = "admin@localhost",
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = "admin@localhost",
                ModifiedDate = DateTime.UtcNow
            },
            new()
            {
                Title = "Sample Product Detail 2",
                Description = "Second sample product detail for testing",
                ProductId = 2,
                CreatedBy = "admin@localhost",
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = "admin@localhost",
                ModifiedDate = DateTime.UtcNow
            }
        });

        await context.SaveChangesAsync();
        
        // Add sample documents for testing
        context.Documents.AddRange(new List<Document>
        {
            new()
            {
                Title = "Sample Document",
                Description = "Sample document for testing",
                Text = "Sample document content",
                DocumentType = "Test",
                ProductDetailId = 1,
                CreatedBy = "admin@localhost",
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = "admin@localhost",
                ModifiedDate = DateTime.UtcNow
            },
            new()
            {
                Title = "Sample Document 2",
                Description = "Second sample document for testing",
                Text = "Second sample document content",
                DocumentType = "Test",
                ProductDetailId = 2,
                CreatedBy = "admin@localhost",
                CreatedDate = DateTime.UtcNow,
                ModifiedBy = "admin@localhost",
                ModifiedDate = DateTime.UtcNow
            }
        });

        await context.SaveChangesAsync();
        
        // Add sample files for testing
        context.Files.AddRange(new List<Media>
        {
            new Media()
            {
                Title = "Sample File",
                Description = "Sample file for testing",
                FilePath = "/sample/path/file1.mp4",
                DirectoryPath = "/sample/path/",
                MediaType = "video",
                ProductDetailId = 1
            },
            new Media()
            {
                Title = "Sample File 2",
                Description = "Second sample file for testing",
                FilePath = "/sample/path/file2.mp4",
                DirectoryPath = "/sample/path/",
                MediaType = "video",
                ProductDetailId = 2
            }
        });

        await context.SaveChangesAsync();
    }
}
