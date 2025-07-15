using System.Net;
using Marketplace.Test.Helpers;
using Marketplace.Test.Infrastructure;
using Xunit;

namespace Marketplace.Test.Scenarios.Documents.IntegrationTests;

[Collection("scenarios")]
public class DocumentTests(WebAppFixture fixture) : ScenarioContext(fixture), IAsyncLifetime
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
    public async Task CreateDocument_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new
                {
                    Title = "New Document",
                    Description = "A new document description.",
                    Text = "Content of the document.",
                    DocumentType = "Type",
                    ProductDetailId = 1
                })
                .ToUrl("/api/documents");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("New Document", responseText);
    }

    [Fact]
    public async Task UpdateDocument_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Put
                .Json(new
                {
                    Id = 1,
                    Title = "Updated Document",
                    Description = "An updated document description.",
                    Text = "Updated content.",
                    DocumentType = "Updated",
                    ProductDetailId = 1
                })
                .ToUrl("/api/documents/1");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("Updated Document", responseText);
    }

    [Fact]
    public async Task DeleteDocument_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Delete.Url("/api/documents/2");
            _.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });
    }
}