using System.Net;
using System.Text.RegularExpressions;
using Marketplace.Test.Helpers;
using Marketplace.Test.Infrastructure;
using Xunit;

namespace Marketplace.Test.Scenarios.Media.IntegrationTests;

[Collection("scenarios")]
public class MediaTests(WebAppFixture fixture) : ScenarioContext(fixture), IAsyncLifetime
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
    public async Task CreateMedia_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new 
                { 
                    Title = "New Media", 
                    Description = "A new media description.",
                    FilePath = "/path/to/file.mp4",
                    DirectoryPath = "/path/to/",
                    MediaType = "Video",
                    ProductDetailId = 1
                })
                .ToUrl("/api/media");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("New Media", responseText);
    }

    [Fact]
    public async Task UpdateMedia_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var createResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new 
                { 
                    Title = "Media to Update", 
                    Description = "A media that will be updated.",
                    FilePath = "/path/to/original.mp4",
                    DirectoryPath = "/path/to/original/",
                    MediaType = "Video",
                    ProductDetailId = 1
                })
                .ToUrl("/api/media");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var createResponseText = await createResponse.ReadAsTextAsync();
        var mediaIdMatch = Regex.Match(createResponseText, @"""id""\s*:\s*(\d+)", RegexOptions.IgnoreCase);
        var mediaId = mediaIdMatch.Success ? mediaIdMatch.Groups[1].Value : "1"; // fallback to 1 if not found

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Put
                .Json(new 
                { 
                    Id = int.Parse(mediaId), 
                    Title = "Updated Media", 
                    Description = "An updated media description.",
                    FilePath = "/path/to/updated.mp4",
                    DirectoryPath = "/path/to/updated/",
                    MediaType = "Audio",
                    ProductDetailId = 1
                })
                .ToUrl($"/api/media/{mediaId}");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("Updated Media", responseText);
    }

    [Fact]
    public async Task DeleteMedia_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);
        
        var createResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new 
                { 
                    Title = "Media to Delete", 
                    Description = "A media that will be deleted.",
                    FilePath = "/path/to/delete.mp4",
                    DirectoryPath = "/path/to/delete/",
                    MediaType = "Video",
                    ProductDetailId = 1
                })
                .ToUrl("/api/media");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var createResponseText = await createResponse.ReadAsTextAsync();
        var mediaIdMatch = Regex.Match(createResponseText, @"""id""\s*:\s*(\d+)", RegexOptions.IgnoreCase);
        var mediaId = mediaIdMatch.Success ? mediaIdMatch.Groups[1].Value : "2"; // fallback to 2 if not found
        
        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Delete.Url($"/api/media/{mediaId}");
            _.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });
    }
}
