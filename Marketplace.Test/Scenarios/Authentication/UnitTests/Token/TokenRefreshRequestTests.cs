using Marketplace.Api.Endpoints.Authentication.Token;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Token;
using Marketplace.Test.Data;
using Xunit;

namespace Marketplace.Test.Scenarios.Authentication.UnitTests.Token;

public class TokenRefreshRequestTests
{
    [Fact]
    public void Creates_TokenRefreshRequest()
    {
        // Arrange
        var accessToken = TestData.AccessToken;
        var refreshToken = TestData.RefreshToken;

        // Act
        var tokenRefreshRequest = new TokenRefreshRequest(
            accessToken!,
            refreshToken!);

        // Assert
        Assert.NotNull(tokenRefreshRequest);
        Assert.IsType<TokenRefreshRequest>(tokenRefreshRequest);
        Assert.True(tokenRefreshRequest.AccessToken == accessToken);
        Assert.True(tokenRefreshRequest.RefreshToken == refreshToken);
        Assert.Equal(accessToken, tokenRefreshRequest.AccessToken);
        Assert.Equal(refreshToken, tokenRefreshRequest.RefreshToken);
    }

    [Fact]
    public void Creates_Null_TokenRefreshRequest()
    {
        // Arrange

        // Act
        var tokenRefreshRequest = new TokenRefreshRequest(null!, null!);

        // Assert
        Assert.NotNull(tokenRefreshRequest);
        Assert.IsType<TokenRefreshRequest>(tokenRefreshRequest);
        Assert.True(tokenRefreshRequest.AccessToken == null);
        Assert.True(tokenRefreshRequest.RefreshToken == null);
        Assert.Null(tokenRefreshRequest.AccessToken);
        Assert.Null(tokenRefreshRequest.RefreshToken);
    }
}