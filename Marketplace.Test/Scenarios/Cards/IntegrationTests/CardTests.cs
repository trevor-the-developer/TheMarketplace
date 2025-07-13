using System.Net;
using System.Text.RegularExpressions;
using Marketplace.Test.Helpers;
using Marketplace.Test.Infrastructure;
using Xunit;

namespace Marketplace.Test.Scenarios.Cards.IntegrationTests;

[Collection("scenarios")]
public class CardTests(WebAppFixture fixture) : ScenarioContext(fixture), IAsyncLifetime
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
    public async Task CreateCard_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new { Title = "New Card", Description = "A new card description.", ListingId = 1 })
                .ToUrl("/api/cards");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("New Card", responseText);
    }

    [Fact]
    public async Task UpdateCard_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Put
                .Json(new { Id = 1, Title = "Updated Card", Description = "An updated card description.", ListingId = 1 })
                .ToUrl("/api/cards/1");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("Updated Card", responseText);
    }

    [Fact]
    public async Task DeleteCard_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Delete.Url("/api/cards/2");
            _.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });
    }
}