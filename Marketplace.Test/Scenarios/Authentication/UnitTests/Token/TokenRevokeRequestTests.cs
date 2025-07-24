using Marketplace.Api.Endpoints.Authentication.Token;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Token;
using Marketplace.Test.Data;
using Xunit;

namespace Marketplace.Test.Scenarios.Authentication.UnitTests.Token;

public class TokenRevokeRequestTests
{
    [Fact]
    public void Create_TokenRevokeRequest()
    {
        // Arrange
        var accessToken = TestData.AccessToken;
        var refreshToken = TestData.RefreshToken;

        // Act
        var tokenRevokeRequest = new TokenRevokeRequest(accessToken!, refreshToken!);

        // Assert
        Assert.NotNull(tokenRevokeRequest);
        Assert.IsType<TokenRevokeRequest>(tokenRevokeRequest);
        Assert.NotNull(tokenRevokeRequest.AccessToken);
        Assert.IsType<string>(tokenRevokeRequest.AccessToken);
        Assert.NotNull(tokenRevokeRequest.RefreshToken);
        Assert.IsType<string>(tokenRevokeRequest.RefreshToken);
        Assert.Equal(accessToken, tokenRevokeRequest.AccessToken);
        Assert.Equal(refreshToken, tokenRevokeRequest.RefreshToken);
    }

    [Fact]
    public void Create_Null_TokenRevokeRequest()
    {
        // Arrange

        // Act
        var tokenRevokeRequest = new TokenRevokeRequest(null!, null!);

        // Assert
        Assert.NotNull(tokenRevokeRequest);
        Assert.IsType<TokenRevokeRequest>(tokenRevokeRequest);
        Assert.Null(tokenRevokeRequest.AccessToken);
        Assert.Null(tokenRevokeRequest.RefreshToken);
    }
}