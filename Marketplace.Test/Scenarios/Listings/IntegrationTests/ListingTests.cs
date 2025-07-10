using System.Net;
using Alba;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Marketplace.Test.Scenarios.Listings.IntegrationTests;

public class ListingTests(WebAppFixture fixture) : ScenarioContext(fixture)
{
    [Fact]
    public async Task CreateListing_Success()
    {
        var response = await Host.Scenario(_ =>
        {
            _.Post
                .Json(new { Title = "New Listing", Description = "A new listing description." })
                .ToUrl("/api/listing/create");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var result = await response.ReadAsJsonAsync<dynamic>();
        Assert.Equal("New Listing", (string)result?.Title);
    }

    [Fact]
    public async Task UpdateListing_Success()
    {
        var response = await Host.Scenario(_ =>
        {
            _.Put
                .Json(new { Id = 1, Title = "Updated Listing", Description = "An updated listing description." })
                .ToUrl($"/api/listing/update/1");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var result = await response.ReadAsJsonAsync<dynamic>();
        Assert.Equal("Updated Listing", (string)result?.Title);
    }

    [Fact]
    public async Task DeleteListing_Success()
    {
        await Host.Scenario(_ =>
        {
            _.Delete.Url($"/api/listing/delete/1");
            _.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });
    }
}
