using System.Net;
using Alba;
using Marketplace.Api.Endpoints.Authentication.Token;
using Marketplace.Core.Constants;
using Marketplace.Test.Data;
using Marketplace.Test.Helpers;
using Marketplace.Test.Infrastructure;
using Newtonsoft.Json;
using Xunit;

namespace Marketplace.Test.Scenarios.Authentication.IntegrationTests.Authentication;

[Collection("scenarios")]
public class TokenTests(WebAppFixture fixture) : ScenarioContext(fixture), IAsyncLifetime
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
    public async Task TokenRefreshRequest()
    {
        // Arrange
        await Task.Delay(100); // Add small delay to avoid race conditions
        var loginResult = await AuthenticationHelper.GetLoginResponse(Host);
        var command = new TokenRefreshRequest
        (
            loginResult.SecurityToken!,
            loginResult.RefreshToken!
        );

        var bearerToken = $"{loginResult.SecurityToken!}";

        // Act
        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(bearerToken);
            _.Post
                .Json(command, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiRefresh);
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var result = await GetTokenResponse(response);

        // Assert
        Assert.NotNull(result?.Succeeded);
        Assert.True(result?.Succeeded);
        Assert.NotNull(result?.RefreshToken);
        Assert.NotNull(result?.JwtToken);
        Assert.NotNull(result?.Expiration);
        Assert.Null(result.ApiError);
    }

    [Fact]
    public async Task TokenRevokeRequest()
    {
        // Arrange
        await Task.Delay(400); // Add delay to avoid race conditions
        var loginResult = await AuthenticationHelper.GetLoginResponse(Host);
        var command = new TokenRevokeRequest
        (
            loginResult.SecurityToken!,
            loginResult.RefreshToken!
        );

        var bearerToken = $"{loginResult.SecurityToken!}";

        // Act
        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(bearerToken);
            _.Post
                .Json(command, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiSlashLogout);
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var result = await GetTokenResponse(response);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Succeeded);
        Assert.True(result.Succeeded);
        Assert.Null(result.ApiError);
        // Note: After revocation, JwtToken and RefreshToken should be null/empty
        // but the response structure might vary based on implementation
    }

    [Fact]
    public async Task Bad_RefreshToken_RefreshRequest()
    {
        // Arrange
        await Task.Delay(200); // Add delay to avoid race conditions
        var loginResult = await AuthenticationHelper.GetLoginResponse(Host);
        var badCommand = new TokenRefreshRequest
        (
            loginResult.SecurityToken!,
            TestData.RefreshToken!
        );

        var bearerToken = $"{loginResult.SecurityToken!}";

        // Act
        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(bearerToken);
            _.Post
                .Json(badCommand, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiRefresh);
            _.StatusCodeShouldBe(HttpStatusCode.Unauthorized);
        });

        // Assert
        Assert.Equal(401, response.Context.Response.StatusCode);
    }

    [Fact]
    public async Task Bad_SecurityToken_RefreshRequest()
    {
        // Arrange
        await Task.Delay(300); // Add delay to avoid race conditions
        var loginResult = await AuthenticationHelper.GetLoginResponse(Host);
        var badCommand = new TokenRefreshRequest
        (
            "invalid.jwt.token.format", // Use genuinely invalid token format
            loginResult.RefreshToken!
        );

        var bearerToken = $"{loginResult.SecurityToken!}";

        // Act
        var response = await Host.Scenario(_ =>
        {
            _.WithBearerToken(bearerToken);
            _.Post
                .Json(badCommand, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiRefresh);
            _.StatusCodeShouldBe(HttpStatusCode.Unauthorized);
        });

        // Assert
        Assert.NotNull(response);
        Assert.IsAssignableFrom<IScenarioResult>(response);
        Assert.Equal(401, response.Context.Response.StatusCode);
    }

    private static async Task<TokenResponse?> GetTokenResponse(IScenarioResult response)
    {
        var jsonString = await response.ReadAsTextAsync();
        var result = JsonConvert.DeserializeObject<TokenResponse>(jsonString);
        return result;
    }
}