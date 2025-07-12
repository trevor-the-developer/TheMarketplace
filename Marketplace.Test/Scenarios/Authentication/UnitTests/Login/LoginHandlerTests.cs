using System.Net;
using Marketplace.Api.Endpoints.Authentication.Login;
using Marketplace.Core.Constants;
using Marketplace.Test.Data;
using Marketplace.Test.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Marketplace.Test.Scenarios.Authentication.UnitTests.Login;

public class LoginHandlerTests
{
    [Fact]
    public void Creates_LoginHandler()
    {
        // Arrange

        // Act
        var loginHandler = new LoginHandler();

        // Assert
        Assert.NotNull(loginHandler);
        Assert.IsType<LoginHandler>(loginHandler);
    }

    [Fact]
    public async Task With_Null_LoginRequest_Parameter_Throws_ArgumentNullException()
    {
        // Arrange
        var userManager = new MockUserManager(TestData.TestApplicationUser);
        var configuration = new Mock<IConfiguration>();
        var tokenService = new MockTokenService();
        var validationService = new MockValidationService();
        var logger = new Mock<ILogger<LoginHandler>>();
        var loginHandler = new LoginHandler();

        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            var result = await loginHandler.Handle
            (
                null!,
                userManager.Object,
                configuration.Object,
                tokenService.Object,
                validationService,
                logger.Object
            );
        });
    }

    [Fact]
    public async Task With_Null_UserManager_Parameter_Throws_ArgumentNullException()
    {
        // Arrange
        var configuration = new Mock<IConfiguration>();
        var tokenService = new MockTokenService();
        var validationService = new MockValidationService();
        var logger = new Mock<ILogger<LoginHandler>>();
        var loginHandler = new LoginHandler();

        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            var command = new LoginRequest("test_user@tester.one", "password");
            var result = await loginHandler.Handle
            (
                command,
                null!,
                configuration.Object,
                tokenService.Object,
                validationService,
                logger.Object
            );
        });
    }

    [Fact]
    public async Task With_Null_Configuration_Parameter_Throws_ArgumentNullException()
    {
        // Arrange
        var userManager = new MockUserManager(TestData.TestApplicationUser);
        var tokenService = new MockTokenService();
        var validationService = new MockValidationService();
        var logger = new Mock<ILogger<LoginHandler>>();
        var loginHandler = new LoginHandler();

        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            var command = new LoginRequest("test_user@tester.one", "password");
            var result = await loginHandler.Handle
            (
                command,
                userManager.Object,
                null!,
                tokenService.Object,
                validationService,
                logger.Object
            );
        });
    }

    [Fact]
    public async Task With_Null_TokenService_Parameter_Throws_ArgumentNullException()
    {
        // Arrange
        var userManager = new MockUserManager(TestData.TestApplicationUser);
        var configuration = new Mock<IConfiguration>();
        var validationService = new MockValidationService();
        var logger = new Mock<ILogger<LoginHandler>>();
        var loginHandler = new LoginHandler();

        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            var command = new LoginRequest("test_user@tester.one", "password");
            var result = await loginHandler.Handle
            (
                command,
                userManager.Object,
                configuration.Object,
                null!,
                validationService,
                logger.Object
            );
        });
    }

    [Fact]
    public async Task With_Null_Logger_Parameter_Throws_ArgumentNullException()
    {
        // Arrange
        var userManager = new MockUserManager(TestData.TestApplicationUser);
        var tokenService = new MockTokenService();
        var validationService = new MockValidationService();
        var configuration = new Mock<IConfiguration>();
        var loginHandler = new LoginHandler();

        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            var command = new LoginRequest("test_user@tester.one", "password");
            var result = await loginHandler.Handle
            (
                command,
                userManager.Object,
                configuration.Object,
                tokenService.Object,
                validationService,
                null!
            );
        });
    }

    [Fact]
    public async Task Returns_LoginResponse_200()
    {
        // Arrange
        var command = new LoginRequest("test_user@tester.one", "password");
        var user = TestData.TestApplicationUser;
        user.EmailConfirmed = true;
        var userManager = new MockUserManager(user);
        var configuration = new Mock<IConfiguration>();
        var tokenService = new MockTokenService();
        var validationService = new MockValidationService();
        var logger = new Mock<ILogger<LoginHandler>>();
        var loginHandler = new LoginHandler();

        // Act
        var result = await loginHandler.Handle
        (
            command,
            userManager.Object,
            configuration.Object,
            tokenService.Object,
            validationService,
            logger.Object
        );

        // Assert
        Assert.NotNull(result);
        Assert.IsType<LoginResponse>(result);
        Assert.True(result.Succeeded);
        Assert.Null(result.ApiError);
        Assert.NotNull(result.RefreshToken);
        Assert.IsType<string>(result.RefreshToken);
        Assert.NotNull(result.SecurityToken);
        Assert.IsType<string>(result.SecurityToken);
    }

    [Fact]
    public async Task UnconfirmedEmail_Returns_LoginResponse_401()
    {
        // Arrange
        var command = new LoginRequest("test_user@tester.one", "password");

        var userManager = new MockUserManager(TestData.TestApplicationUser);
        var configuration = new Mock<IConfiguration>();
        var tokenService = new MockTokenService();
        var validationService = new MockValidationService();
        var logger = new Mock<ILogger<LoginHandler>>();
        var loginHandler = new LoginHandler();

        // Act
        var result = await loginHandler.Handle
        (
            command,
            userManager.Object,
            configuration.Object,
            tokenService.Object,
            validationService,
            logger.Object
        );

        // Assert
        Assert.NotNull(result);
        Assert.IsType<LoginResponse>(result);
        Assert.NotNull(result.ApiError);
        Assert.True(result.ApiError?.StatusCode is (int)HttpStatusCode.Unauthorized);
        Assert.True(result.ApiError?.HttpStatusCode == ((int)HttpStatusCode.Unauthorized).ToString());
        Assert.Equal(AuthConstants.UserEmailNotConfirmed, result.ApiError?.ErrorMessage);
    }

    [Fact]
    public async Task InvalidEmail_Returns_LoginResponse_401()
    {
        // Arrange
        var command = new LoginRequest("test_user@tester.one", "password");

        var userManager = new MockUserManager(TestData.TestApplicationUser, true);
        var configuration = new Mock<IConfiguration>();
        var tokenService = new MockTokenService();
        var validationService = new MockValidationService();
        var logger = new Mock<ILogger<LoginHandler>>();
        var loginHandler = new LoginHandler();

        // Act
        var result = await loginHandler.Handle
        (
            command,
            userManager.Object,
            configuration.Object,
            tokenService.Object,
            validationService,
            logger.Object
        );

        // Assert
        Assert.NotNull(result);
        Assert.IsType<LoginResponse>(result);
        Assert.NotNull(result.ApiError);
        Assert.True(result.ApiError?.StatusCode is (int)HttpStatusCode.Unauthorized);
        Assert.True(result.ApiError?.HttpStatusCode == ((int)HttpStatusCode.Unauthorized).ToString());
        Assert.Equal(AuthConstants.UserDoesntExist, result.ApiError?.ErrorMessage);
    }

    [Fact]
    public async Task InvalidPassword_Returns_LoginResponse_401()
    {
        // Arrange
        var command = new LoginRequest("test_user@tester.one", null!);
        var user = TestData.TestApplicationUser;
        user.EmailConfirmed = true;
        var userManager = new MockUserManager(user, invalidPassword: true);
        var configuration = new Mock<IConfiguration>();
        var tokenService = new MockTokenService();
        var validationService = new MockValidationService();
        var logger = new Mock<ILogger<LoginHandler>>();
        var loginHandler = new LoginHandler();

        // Act
        var result = await loginHandler.Handle
        (
            command,
            userManager.Object,
            configuration.Object,
            tokenService.Object,
            validationService,
            logger.Object
        );

        // Assert
        Assert.NotNull(result);
        Assert.IsType<LoginResponse>(result);
        Assert.NotNull(result.ApiError);
        Assert.True(result.ApiError?.StatusCode is (int)HttpStatusCode.Unauthorized);
        Assert.True(result.ApiError?.HttpStatusCode == ((int)HttpStatusCode.Unauthorized).ToString());
        Assert.Equal(AuthConstants.InvalidEmailPassword, result.ApiError?.ErrorMessage);
    }

    [Fact]
    public async Task InvalidToken_Throws_ArgumentNullException()
    {
        // Arrange
        var command = new LoginRequest("test_user@tester.one", null!);
        var user = TestData.TestApplicationUser;
        user.EmailConfirmed = true;
        var userManager = new MockUserManager(user);
        var configuration = new Mock<IConfiguration>();
        var tokenService = new MockTokenService(new MockTokenServiceOptions { ReturnNullToken = true });
        var validationService = new MockValidationService();
        var logger = new Mock<ILogger<LoginHandler>>();
        var loginHandler = new LoginHandler();

        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            var result = await loginHandler.Handle
            (
                command,
                userManager.Object,
                configuration.Object,
                tokenService.Object,
                validationService,
                logger.Object
            );
        });
    }

    [Fact]
    public async Task InvalidRefreshToken_Throws_ArgumentNullException()
    {
        // Arrange
        var command = new LoginRequest("test_user@tester.one", null!);
        var user = TestData.TestApplicationUser;
        user.EmailConfirmed = true;
        var userManager = new MockUserManager(user);
        var configuration = new Mock<IConfiguration>();
        var tokenService = new MockTokenService(new MockTokenServiceOptions { ReturnNullRefreshToken = true });
        var validationService = new MockValidationService();
        var logger = new Mock<ILogger<LoginHandler>>();
        var loginHandler = new LoginHandler();

        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            var result = await loginHandler.Handle
            (
                command,
                userManager.Object,
                configuration.Object,
                tokenService.Object,
                validationService,
                logger.Object
            );
        });
    }

    [Fact]
    public async Task UpdateFailed_Returns_LoginResponse_500()
    {
        // Arrange
        var command = new LoginRequest("test_user@tester.one", "password");
        var user = TestData.TestApplicationUser;
        user.EmailConfirmed = true;
        var userManager = new MockUserManager(user, updateAsyncFailed: true);
        var configuration = new Mock<IConfiguration>();
        var tokenService = new MockTokenService();
        var validationService = new MockValidationService();
        var logger = new Mock<ILogger<LoginHandler>>();
        var loginHandler = new LoginHandler();

        // Act
        var result = await loginHandler.Handle
        (
            command,
            userManager.Object,
            configuration.Object,
            tokenService.Object,
            validationService,
            logger.Object
        );

        // Assert
        Assert.NotNull(result);
        Assert.IsType<LoginResponse>(result);
        Assert.NotNull(result.ApiError);
        Assert.True(result.ApiError?.StatusCode is (int)HttpStatusCode.InternalServerError);
        Assert.True(result.ApiError?.HttpStatusCode == ((int)HttpStatusCode.InternalServerError).ToString());
        Assert.Equal(AuthConstants.LoginFailed, result.ApiError?.ErrorMessage);
    }

    [Fact]
    public async Task ValidationFailed_Returns_LoginResponse_400()
    {
        // Arrange
        var command = new LoginRequest("", "123"); // Invalid email and password
        var user = TestData.TestApplicationUser;
        user.EmailConfirmed = true;
        var userManager = new MockUserManager(user);
        var configuration = new Mock<IConfiguration>();
        var tokenService = new MockTokenService();
        
        // Setup validation service to return validation errors
        var validationErrors = new List<string> { "Email is required", "Password must be at least 6 characters long" };
        var validationService = new MockValidationService(new MockValidationServiceOptions { ValidationErrors = validationErrors });
        
        var logger = new Mock<ILogger<LoginHandler>>();
        var loginHandler = new LoginHandler();

        // Act
        var result = await loginHandler.Handle
        (
            command,
            userManager.Object,
            configuration.Object,
            tokenService.Object,
            validationService,
            logger.Object
        );

        // Assert
        Assert.NotNull(result);
        Assert.IsType<LoginResponse>(result);
        Assert.NotNull(result.ApiError);
        Assert.True(result.ApiError?.StatusCode is (int)HttpStatusCode.BadRequest);
        Assert.True(result.ApiError?.HttpStatusCode == ((int)HttpStatusCode.BadRequest).ToString());
        Assert.Equal("Validation failed", result.ApiError?.ErrorMessage);
        Assert.Contains("Email is required", result.ApiError?.StackTrace ?? "");
        Assert.Contains("Password must be at least 6 characters long", result.ApiError?.StackTrace ?? "");
    }
}
