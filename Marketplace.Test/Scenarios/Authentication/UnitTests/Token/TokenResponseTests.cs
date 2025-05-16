using System.Net;
using Marketplace.Api.Endpoints.Authentication.Token;
using Marketplace.Core;
using Marketplace.Test.Data;
using Xunit;

namespace Marketplace.Test.Scenarios.Authentication.UnitTests.Token;

public class TokenResponseTests
{
    [Fact]
    public void Creates_TokenResponse()
    {
        // Arrange

        // Act
        var tokenResponse = new TokenResponse();

        // Assert
        Assert.NotNull(tokenResponse);
        Assert.IsType<TokenResponse>(tokenResponse);
        Assert.Null(tokenResponse.Succeeded);
        Assert.Null(tokenResponse.JwtToken);
        Assert.Null(tokenResponse.RefreshToken);
        Assert.Null(tokenResponse.Expiration);
        Assert.Null(tokenResponse.ApiError);
        Assert.Null(tokenResponse.ApiError?.ErrorMessage);
        Assert.Null(tokenResponse.ApiError?.StackTrace);
    }

    [Fact]
    public void Creates_Populated_TokenResponse()
    {
        // Arrange
        var succeeded = true;
        var jwtToken = TestData.AccessToken;
        var refreshToken = TestData.RefreshToken;
        var expiration = DateTime.Now;
        var apiError = new ApiError(
            HttpStatusCode.Unauthorized.ToString(),
            (int)HttpStatusCode.Unauthorized,
            null,
            null
        );

        // Act
        var tokenResponse = new TokenResponse
        {
            Succeeded = succeeded,
            JwtToken = jwtToken,
            Expiration = expiration,
            RefreshToken = refreshToken,
            ApiError = apiError
        };

        // Assert
        Assert.NotNull(tokenResponse);
        Assert.IsType<TokenResponse>(tokenResponse);
        Assert.NotNull(tokenResponse.Succeeded);
        Assert.IsType<bool>(tokenResponse.Succeeded);
        Assert.NotNull(tokenResponse.JwtToken);
        Assert.IsType<DateTime>(tokenResponse.Expiration);
        Assert.NotNull(tokenResponse.RefreshToken);
        Assert.IsType<ApiError>(tokenResponse.ApiError);
        Assert.NotNull(tokenResponse.Expiration);
        Assert.Equal(succeeded, tokenResponse.Succeeded);
        Assert.Equal(jwtToken, tokenResponse.JwtToken);
        Assert.Equal(expiration, tokenResponse.Expiration);
        Assert.Equal(refreshToken, tokenResponse.RefreshToken);
        Assert.Equal(apiError, tokenResponse.ApiError);
        Assert.NotNull(tokenResponse.ApiError);
        Assert.IsType<ApiError>(tokenResponse.ApiError);
        Assert.Equal(HttpStatusCode.Unauthorized.ToString(), tokenResponse.ApiError.HttpStatusCode);
        Assert.Equal((int)HttpStatusCode.Unauthorized, tokenResponse.ApiError.StatusCode);
        Assert.Null(tokenResponse.ApiError.ErrorMessage);
        Assert.Null(tokenResponse.ApiError.StackTrace);
    }
}