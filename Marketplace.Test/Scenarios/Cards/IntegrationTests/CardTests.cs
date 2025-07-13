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

        // First create a card to update
        var createResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new { Title = "Card to Update", Description = "A card that will be updated.", ListingId = 1 })
                .ToUrl("/api/cards");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var createResponseText = await createResponse.ReadAsTextAsync();
        // Extract the card ID from the response - this is a simplified approach
        var cardIdMatch = Regex.Match(createResponseText, @"""id""\s*:\s*(\d+)", RegexOptions.IgnoreCase);
        var cardId = cardIdMatch.Success ? cardIdMatch.Groups[1].Value : "1"; // fallback to 1 if not found

        // Now update the card
        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Put
                .Json(new { Id = int.Parse(cardId), Title = "Updated Card", Description = "An updated card description." })
                .ToUrl($"/api/cards/{cardId}");
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

        // First create a card to delete
        var createResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new { Title = "Card to Delete", Description = "A card that will be deleted.", ListingId = 1 })
                .ToUrl("/api/cards");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var createResponseText = await createResponse.ReadAsTextAsync();
        // Extract the card ID from the response - this is a simplified approach
        var cardIdMatch = Regex.Match(createResponseText, @"""id""\s*:\s*(\d+)", RegexOptions.IgnoreCase);
        var cardId = cardIdMatch.Success ? cardIdMatch.Groups[1].Value : "2"; // fallback to 2 if not found

        // Now delete the card
        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Delete.Url($"/api/cards/{cardId}");
            _.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });
    }
}