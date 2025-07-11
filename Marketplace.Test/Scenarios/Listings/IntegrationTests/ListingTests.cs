using System.Net;
using System.Text.RegularExpressions;
using Marketplace.Test.Helpers;
using Xunit;

namespace Marketplace.Test.Scenarios.Listings.IntegrationTests;

public class ListingTests(WebAppFixture fixture) : ScenarioContext(fixture)
{
    [Fact]
    public async Task CreateListing_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new { Title = "New Listing", Description = "A new listing description." })
                .ToUrl("/api/listing/create");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("New Listing", responseText);
    }

    [Fact]
    public async Task UpdateListing_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Put
                .Json(new { Id = 1, Title = "Updated Listing", Description = "An updated listing description." })
                .ToUrl("/api/listing/update/1");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("Updated Listing", responseText);
    }

    [Fact]
    public async Task DeleteListing_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        // First create a listing to delete
        var createResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new { Title = "Listing to Delete", Description = "A listing that will be deleted." })
                .ToUrl("/api/listing/create");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var createResponseText = await createResponse.ReadAsTextAsync();
        // Extract the listing ID from the response - this is a simplified approach
        var listingIdMatch = Regex.Match(createResponseText, @"""id""\s*:\s*(\d+)", RegexOptions.IgnoreCase);
        var listingId = listingIdMatch.Success ? listingIdMatch.Groups[1].Value : "2"; // fallback to 2 if not found

        // Now delete the listing
        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Delete.Url($"/api/listing/delete/{listingId}");
            _.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });
    }
}