using System.Net;
using System.Text.RegularExpressions;
using Marketplace.Test.Helpers;
using Marketplace.Test.Infrastructure;
using Xunit;

namespace Marketplace.Test.Scenarios.Tags.IntegrationTests;

[Collection("scenarios")]
public class TagTests(WebAppFixture fixture) : ScenarioContext(fixture), IAsyncLifetime
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
    public async Task CreateTag_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new
                {
                    Name = "New Tag",
                    Description = "A new tag description.",
                    IsEnabled = true
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("New Tag", responseText);
    }

    [Fact]
    public async Task UpdateTag_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Put
                .Json(new
                {
                    Id = 1, Name = "Updated Tag", Description = "An updated tag description.", IsEnabled = false
                })
                .ToUrl("/api/tags/1");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("Updated Tag", responseText);
    }

    [Fact]
    public async Task DeleteTag_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var createResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new
                {
                    Name = "Tag to Delete",
                    Description = "A tag that will be deleted.",
                    IsEnabled = true
                })
                .ToUrl("/api/tags");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var createResponseText = await createResponse.ReadAsTextAsync();
        // Look for the tag ID in the nested structure: {"tag":{"id":1,...},...}
        var tagIdMatch = Regex.Match(createResponseText, @"""tag""\s*:\s*\{[^}]*""id""\s*:\s*(\d+)",
            RegexOptions.IgnoreCase);
        var tagId = tagIdMatch.Success ? tagIdMatch.Groups[1].Value : "2"; // fallback to 2 if not found

        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Delete.Url($"/api/tags/{tagId}");
            _.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });
    }
}