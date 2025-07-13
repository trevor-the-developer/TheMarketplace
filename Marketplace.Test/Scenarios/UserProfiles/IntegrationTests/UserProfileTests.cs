using System.Net;
using System.Text.RegularExpressions;
using Marketplace.Test.Helpers;
using Marketplace.Test.Infrastructure;
using Xunit;

namespace Marketplace.Test.Scenarios.UserProfiles.IntegrationTests;

[Collection("scenarios")]
public class UserProfileTests(WebAppFixture fixture) : ScenarioContext(fixture), IAsyncLifetime
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
    public async Task CreateUserProfile_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new 
                { 
                    DisplayName = "New User Profile", 
                    Bio = "A new user profile bio.",
                    SocialMedia = "newuser@social.com",
                    ApplicationUserId = "69a38a69-e24d-4c7f-bdf2-c7bc2222cbe7"
                })
                .ToUrl("/api/user-profiles");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("New User Profile", responseText);
    }

    [Fact]
    public async Task UpdateUserProfile_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);
        var applicationUserId = "a5ac5ebb-5f11-4363-a58d-4362d8ff6863";

        var createResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new 
                { 
                    DisplayName = "User Profile to Update", 
                    Bio = "A user profile that will be updated.",
                    SocialMedia = "update@social.com",
                    ApplicationUserId = applicationUserId
                })
                .ToUrl("/api/user-profiles");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var createResponseText = await createResponse.ReadAsTextAsync();
        Assert.NotNull(createResponseText);
        Assert.Contains("User Profile to Update", createResponseText);

        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Put
                .Json(new 
                { 
                    DisplayName = "Updated User Profile", 
                    Bio = "An updated user profile bio.",
                    SocialMedia = "updated@social.com",
                    ApplicationUserId = applicationUserId
                })
                .ToUrl($"/api/user-profiles/{applicationUserId}");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var responseText = await response.ReadAsTextAsync();
        Assert.NotNull(responseText);
        Assert.Contains("Updated User Profile", responseText);
    }

    [Fact]
    public async Task DeleteUserProfile_Success()
    {
        var token = await AuthenticationHelper.GetAdminTokenAsync(Host);
        var applicationUserId = "69a38a69-e24d-4c7f-bdf2-c7bc2222cbe7";

        var createResponse = await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Post
                .Json(new 
                { 
                    DisplayName = "User Profile to Delete", 
                    Bio = "A user profile that will be deleted.",
                    SocialMedia = "delete@social.com",
                    ApplicationUserId = applicationUserId
                })
                .ToUrl("/api/user-profiles");
            _.StatusCodeShouldBe(HttpStatusCode.Created);
        });

        var createResponseText = await createResponse.ReadAsTextAsync();
        Assert.NotNull(createResponseText);
        Assert.Contains("User Profile to Delete", createResponseText);

        await Host.Scenario(_ =>
        {
            _.WithBearerToken(token);
            _.Delete.Url($"/api/user-profiles/{applicationUserId}");
            _.StatusCodeShouldBe(HttpStatusCode.NoContent);
        });
    }
}
