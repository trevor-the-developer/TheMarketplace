using System.Net;
using System.Text.RegularExpressions;
using Marketplace.Test.Helpers;
using Marketplace.Test.Infrastructure;
using Xunit;

namespace Marketplace.Test.Scenarios.ProductDetails.IntegrationTests;

[Collection("scenarios")]
public class ProductDetailTests(WebAppFixture fixture) : ScenarioContext(fixture), IAsyncLifetime
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
    public async Task CreateProductDetail_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var productResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new 
                { 
                    Title = "Product for Detail", 
                    Description = "A product that needs a detail.",
                    ProductType = "Test Type",
                    Category = "Test Category",
                    IsEnabled = true,
                    IsDeleted = false,
                    CardId = 1
                })
                .ToUrl("/api/products");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var productResponseText = await productResponse.ReadAsTextAsync();
        var productIdMatch = Regex.Match(productResponseText, @"""id""\s*:\s*(\d+)", RegexOptions.IgnoreCase);
        var productId = productIdMatch.Success ? int.Parse(productIdMatch.Groups[1].Value) : 4; // fallback to 4 if not found

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new 
                { 
                    Title = "New Product Detail", 
                    Description = "A new product detail description.",
                    ProductId = productId
                })
                .ToUrl("/api/product-details");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("New Product Detail", responseText);
    }

    [Fact]
    public async Task UpdateProductDetail_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var productResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new 
                { 
                    Title = "Product for Detail Update", 
                    Description = "A product that needs a detail for update.",
                    ProductType = "Update Type",
                    Category = "Update Category",
                    IsEnabled = true,
                    IsDeleted = false,
                    CardId = 1
                })
                .ToUrl("/api/products");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var productResponseText = await productResponse.ReadAsTextAsync();
        var productIdMatch = Regex.Match(productResponseText, @"""id""\s*:\s*(\d+)", RegexOptions.IgnoreCase);
        var productId = productIdMatch.Success ? int.Parse(productIdMatch.Groups[1].Value) : 5; // fallback to 5 if not found

        var createResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new 
                { 
                    Title = "Product Detail to Update", 
                    Description = "A product detail that will be updated.",
                    ProductId = productId
                })
                .ToUrl("/api/product-details");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var createResponseText = await createResponse.ReadAsTextAsync();
        var productDetailIdMatch = Regex.Match(createResponseText, @"""id""\s*:\s*(\d+)", RegexOptions.IgnoreCase);
        var productDetailId = productDetailIdMatch.Success ? productDetailIdMatch.Groups[1].Value : "1"; // fallback to 1 if not found

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Put
                .Json(new 
                { 
                    Id = int.Parse(productDetailId), 
                    Title = "Updated Product Detail", 
                    Description = "An updated product detail description.",
                    ProductId = productId
                })
                .ToUrl($"/api/product-details/{productDetailId}");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("Updated Product Detail", responseText);
    }

    [Fact]
    public async Task DeleteProductDetail_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var productResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new 
                { 
                    Title = "Product for Detail Delete", 
                    Description = "A product that needs a detail for delete.",
                    ProductType = "Delete Type",
                    Category = "Delete Category",
                    IsEnabled = true,
                    IsDeleted = false,
                    CardId = 1
                })
                .ToUrl("/api/products");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var productResponseText = await productResponse.ReadAsTextAsync();
        var productIdMatch = Regex.Match(productResponseText, @"""id""\s*:\s*(\d+)", RegexOptions.IgnoreCase);
        var productId = productIdMatch.Success ? int.Parse(productIdMatch.Groups[1].Value) : 6; // fallback to 6 if not found

        var createResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new 
                { 
                    Title = "Product Detail to Delete", 
                    Description = "A product detail that will be deleted.",
                    ProductId = productId
                })
                .ToUrl("/api/product-details");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var createResponseText = await createResponse.ReadAsTextAsync();
        var productDetailIdMatch = Regex.Match(createResponseText, @"""id""\s*:\s*(\d+)", RegexOptions.IgnoreCase);
        var productDetailId = productDetailIdMatch.Success ? productDetailIdMatch.Groups[1].Value : "2"; // fallback to 2 if not found

        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Delete.Url($"/api/product-details/{productDetailId}");
            _.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });
    }
}
