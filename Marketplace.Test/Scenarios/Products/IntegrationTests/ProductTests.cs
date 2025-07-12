using System.Net;
using System.Text.RegularExpressions;
using Marketplace.Test.Helpers;
using Marketplace.Test.Infrastructure;
using Xunit;

namespace Marketplace.Test.Scenarios.Products.IntegrationTests;

[Collection("scenarios")]
public class ProductTests(WebAppFixture fixture) : ScenarioContext(fixture), IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await DatabaseResetService.ResetDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    [Fact]
    public async Task CreateProduct_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new 
                { 
                    Title = "New Product", 
                    Description = "A new product description.",
                    ProductType = "Test Type",
                    Category = "Test Category",
                    IsEnabled = true,
                    IsDeleted = false,
                    CardId = 1
                })
                .ToUrl("/api/product/create");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("New Product", responseText);
    }

    [Fact]
    public async Task UpdateProduct_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var createResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new 
                { 
                    Title = "Product to Update", 
                    Description = "A product that will be updated.",
                    ProductType = "Original Type",
                    Category = "Original Category",
                    IsEnabled = true,
                    IsDeleted = false,
                    CardId = 1
                })
                .ToUrl("/api/product/create");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var createResponseText = await createResponse.ReadAsTextAsync();
        var productIdMatch = Regex.Match(createResponseText, @"""id""\s*:\s*(\d+)", RegexOptions.IgnoreCase);
        var productId = productIdMatch.Success ? productIdMatch.Groups[1].Value : "1"; // fallback to 1 if not found

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Put
                .Json(new 
                { 
                    Id = int.Parse(productId), 
                    Title = "Updated Product", 
                    Description = "An updated product description.",
                    ProductType = "Updated Type",
                    Category = "Updated Category",
                    IsEnabled = true,
                    IsDeleted = false,
                    CardId = 1
                })
                .ToUrl($"/api/product/update/{productId}");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("Updated Product", responseText);
    }

    [Fact]
    public async Task DeleteProduct_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var createResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new 
                { 
                    Title = "Product to Delete", 
                    Description = "A product that will be deleted.",
                    ProductType = "Delete Type",
                    Category = "Delete Category",
                    IsEnabled = true,
                    IsDeleted = false,
                    CardId = 1
                })
                .ToUrl("/api/product/create");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var createResponseText = await createResponse.ReadAsTextAsync();
        var productIdMatch = Regex.Match(createResponseText, @"""id""\s*:\s*(\d+)", RegexOptions.IgnoreCase);
        var productId = productIdMatch.Success ? productIdMatch.Groups[1].Value : "2"; // fallback to 2 if not found

        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Delete.Url($"/api/product/delete/{productId}");
            _.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });
    }
}
