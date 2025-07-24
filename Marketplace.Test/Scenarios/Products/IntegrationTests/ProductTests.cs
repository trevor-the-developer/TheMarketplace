using System.Net;
using System.Threading.Tasks;
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
                .ToUrl("/api/products");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("New Product", responseText);
    }

    [Fact]
    public async Task UpdateProduct_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Put
                .Json(new
                {
                    Id = 1,
                    Title = "Updated Product",
                    Description = "An updated product description.",
                    ProductType = "Updated Type",
                    Category = "Updated Category",
                    IsEnabled = true,
                    IsDeleted = false,
                    CardId = 1
                })
                .ToUrl("/api/products/1");
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

        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Delete.Url("/api/products/2");
            _.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });
    }
}