using Marketplace.Api.Endpoints.Authentication.Login;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Login;
using Xunit;

namespace Marketplace.Test.Scenarios.Authentication.UnitTests.Login;

public class LoginResponseTests
{
    [Fact]
    public void Creates_LoginResponse()
    {
        // Arrange

        // Act
        var loginResponse = new LoginResponse();

        // Assert
        Assert.NotNull(loginResponse);
        Assert.IsType<LoginResponse>(loginResponse);
        Assert.True(loginResponse.Email == null);
        Assert.True(loginResponse.SecurityToken == null);
    }

    [Fact]
    public void Creates_Populated_LoginResponse()
    {
        // Arrange

        // Act
        var loginResponse = new LoginResponse();
        loginResponse.Email = "test_email@tester.one";
        loginResponse.SecurityToken = "test_security_token";

        // Assert
        Assert.NotNull(loginResponse);
        Assert.IsType<LoginResponse>(loginResponse);
        Assert.True(loginResponse.Email == "test_email@tester.one");
        Assert.True(loginResponse.SecurityToken == "test_security_token");
    }
}