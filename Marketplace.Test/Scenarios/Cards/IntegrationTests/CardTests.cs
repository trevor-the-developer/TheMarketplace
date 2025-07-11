using System.Net;
using Alba;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Marketplace.Test.Helpers;
using Xunit;

namespace Marketplace.Test.Scenarios.Cards.IntegrationTests;

public class CardTests(WebAppFixture fixture) : ScenarioContext(fixture)
{
    [Fact]
    public async Task CreateCard_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);
        
        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new { Title = "New Card", Description = "A new card description.", ListingId = 1 })
                .ToUrl("/api/card/create");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
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
                .Json(new { Id = 1, Title = "Updated Card", Description = "An updated card description." })
                .ToUrl($"/api/card/update/1");
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
                .ToUrl("/api/card/create");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });
        
        var createResponseText = await createResponse.ReadAsTextAsync();
        // Extract the card ID from the response - this is a simplified approach
        var cardIdMatch = System.Text.RegularExpressions.Regex.Match(createResponseText, @"""id""\s*:\s*(\d+)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
        var cardId = cardIdMatch.Success ? cardIdMatch.Groups[1].Value : "2"; // fallback to 2 if not found
        
        // Now delete the card
        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Delete.Url($"/api/card/delete/{cardId}");
            _.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });
    }
}
