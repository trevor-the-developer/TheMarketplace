using Marketplace.Api.Endpoints.Authentication.Login;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Login;
using Xunit;

namespace Marketplace.Test.Scenarios.Authentication.UnitTests.Login;

public class LoginRequestTests
{
    [Fact]
    public void Creates_LoginRequest()
    {
        // Arrange

        // Act
        var loginRequest = new LoginRequest(null!, null!);

        // Assert
        Assert.NotNull(loginRequest);
        Assert.IsType<LoginRequest>(loginRequest);
        Assert.True(loginRequest.Email == null);
        Assert.True(loginRequest.Password == null);
    }

    [Fact]
    public void Creates_Populated_LoginRequest()
    {
        // Arrange

        // Act
        var loginRequest = new LoginRequest("test_email@tester.one", "p@aSsw0rd!");

        // Assert
        Assert.NotNull(loginRequest);
        Assert.IsType<LoginRequest>(loginRequest);
        Assert.True(loginRequest.Email == "test_email@tester.one");
        Assert.True(loginRequest.Password == "p@aSsw0rd!");
    }
}