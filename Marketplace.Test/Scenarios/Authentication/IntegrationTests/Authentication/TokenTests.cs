using System.Net;
using Alba;
using Marketplace.Api.Endpoints.Authentication.Login;
using Marketplace.Api.Endpoints.Authentication.Token;
using Marketplace.Core.Constants;
using Marketplace.Test.Data;
using Marketplace.Test.Helpers;
using Newtonsoft.Json;
using Xunit;

namespace Marketplace.Test.Scenarios.Authentication.IntegrationTests.Authentication;

public class TokenTests(WebAppFixture fixture) : ScenarioContext(fixture)
{
    [Fact]
    public async Task TokenRefreshRequest()
    {
        // Arrange
        var loginResult = await GetLoginResponse();
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
    }

    [Fact]
    public async Task Bad_RefreshToken_RefreshRequest()
    {
        // Arrange
        var loginResult = await GetLoginResponse();
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
        var loginResult = await GetLoginResponse();
        var badCommand = new TokenRefreshRequest
        (
            TestData.AccessToken!,
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
        var jsonResult = await JsonHelper.ReadAsJsonAsync(response);
        var unescapedJson = JsonConvert.DeserializeObject<string>(jsonResult);
        var result = JsonConvert.DeserializeObject<TokenResponse>(unescapedJson!);
        return result;
    }

    private async Task<LoginResponse> GetLoginResponse()
    {
        var loginResponse = await Host.Scenario(_ =>
        {
            _.Post
                .Json(new { Email = "admin@localhost", Password = "P@ssw0rd!" }, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiSlashLogin);
        });

        Assert.NotNull(loginResponse);
        Assert.IsType<ScenarioResult>(loginResponse);
        var loginResult = await loginResponse.ReadAsJsonAsync<LoginResponse>();
        Assert.True(loginResult?.Succeeded);
        Assert.NotNull(loginResult?.SecurityToken);
        Assert.NotNull(loginResult.Expiration);
        Assert.NotNull(loginResult.RefreshToken);
        Assert.Null(loginResult.ApiError);
        return loginResult;
    }
}