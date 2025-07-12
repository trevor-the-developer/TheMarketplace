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
                .ToUrl("/api/tag/create");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("New Tag", responseText);
    }

    [Fact]
    public async Task UpdateTag_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var createResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new 
                { 
                    Name = "Tag to Update", 
                    Description = "A tag that will be updated.",
                    IsEnabled = true
                })
                .ToUrl("/api/tag/create");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var createResponseText = await createResponse.ReadAsTextAsync();
        var tagIdMatch = Regex.Match(createResponseText, @"""id""\s*:\s*(\d+)", RegexOptions.IgnoreCase);
        var tagId = tagIdMatch.Success ? tagIdMatch.Groups[1].Value : "1"; // fallback to 1 if not found

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Put
                .Json(new 
                { 
                    Id = int.Parse(tagId), 
                    Name = "Updated Tag", 
                    Description = "An updated tag description.",
                    IsEnabled = false
                })
                .ToUrl($"/api/tag/update/{tagId}");
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
                .ToUrl("/api/tag/create");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var createResponseText = await createResponse.ReadAsTextAsync();
        var tagIdMatch = Regex.Match(createResponseText, @"""id""\s*:\s*(\d+)", RegexOptions.IgnoreCase);
        var tagId = tagIdMatch.Success ? tagIdMatch.Groups[1].Value : "2"; // fallback to 2 if not found

        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Delete.Url($"/api/tag/delete/{tagId}");
            _.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });
    }
}
