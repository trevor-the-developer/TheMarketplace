using System.Net;
using Alba;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Marketplace.Test.Scenarios.Cards.IntegrationTests;

public class CardTests(WebAppFixture fixture) : ScenarioContext(fixture)
{
    [Fact]
    public async Task CreateCard_Success()
    {
        var response = await Host.Scenario(_ =>
        {
            _.Post
                .Json(new { Title = "New Card", Description = "A new card description.", ListingId = 1 })
                .ToUrl("/api/card/create");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var result = await response.ReadAsJsonAsync<dynamic>();
        Assert.NotNull(result);
        Assert.Equal("New Card", (string)result.Title);
    }

    [Fact]
    public async Task UpdateCard_Success()
    {
        var response = await Host.Scenario(_ =>
        {
            _.Put
                .Json(new { Id = 1, Title = "Updated Card", Description = "An updated card description." })
                .ToUrl($"/api/card/update/1");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var result = await response.ReadAsJsonAsync<dynamic>();
        Assert.NotNull(result);
        Assert.Equal("Updated Card", (string)result.Title);
    }

    [Fact]
    public async Task DeleteCard_Success()
    {
        await Host.Scenario(_ =>
        {
            _.Delete.Url($"/api/card/delete/1");
            _.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });
    }
}
