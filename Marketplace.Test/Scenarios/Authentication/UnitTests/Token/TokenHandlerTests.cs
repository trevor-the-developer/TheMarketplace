using System;
using System.Net;
using System.Threading.Tasks;
using Marketplace.Api.Endpoints.Authentication.Token;
using Marketplace.Core.Constants;
using Marketplace.Core.Interfaces;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Token;
using Marketplace.Core.Security;
using Marketplace.Test.Data;
using Marketplace.Test.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Xunit;
using TokenHandler = Marketplace.Api.Endpoints.Authentication.Token.TokenHandler;

namespace Marketplace.Test.Scenarios.Authentication.UnitTests.Token;

public class TokenHandlerTests
{
    [Fact]
    public void Create_TokenHandler()
    {
        // Arrange

        // Act
        var tokenHandler = new TokenHandler();

        // Assert
        Assert.NotNull(tokenHandler);
        Assert.IsType<TokenHandler>(tokenHandler);
    }

    [Fact]
    public async Task With_Null_TokenRefreshRequest_Parameter_Throws_ArgumentNullException()
    {
        // Arrange
        var authRepository = new MockAuthenticationRepository(TestData.TestApplicationUser);
        var tokenService = new MockTokenService();
        var tokenValidationParameters = new Mock<TokenValidationParameters>();
        var configuration = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<ITokenService>>();
        var tokenHandler = new TokenHandler();

        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            var result = await tokenHandler.Handle
            (
                null!,
                authRepository.Object,
                tokenService.Object,
                tokenValidationParameters.Object,
                configuration.Object,
                logger.Object
            );
        });
    }

    [Fact]
    public async Task With_Null_UserManager_Parameter_Throws_ArgumentNullException()
    {
        // Arrange
        var command = new TokenRefreshRequest(TestData.AccessToken!, TestData.RefreshToken!);
        var tokenService = new MockTokenService();
        var tokenValidationParameters = new Mock<TokenValidationParameters>();
        var configuration = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<ITokenService>>();
        var tokenHandler = new TokenHandler();

        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            var result = await tokenHandler.Handle
            (
                command,
                null!,
                tokenService.Object,
                tokenValidationParameters.Object,
                configuration.Object,
                logger.Object
            );
        });
    }

    [Fact]
    public async Task With_Null_TokenService_Parameter_Throws_ArgumentNullException()
    {
        // Arrange
        var command = new TokenRefreshRequest(TestData.AccessToken!, TestData.RefreshToken!);
        var authRepository = new MockAuthenticationRepository(TestData.TestApplicationUser);
        var configuration = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<ITokenService>>();
        var tokenService = new Mock<TokenService>();
        var tokenValidationParameters = new Mock<TokenValidationParameters>();
        var tokenHandler = new TokenHandler();

        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            var result = await tokenHandler.Handle
            (
                command,
                authRepository.Object,
                null!,
                tokenValidationParameters.Object,
                configuration.Object,
                logger.Object
            );
        });
    }

    [Fact]
    public async Task With_Null_Configuration_Parameter_Throws_ArgumentNullException()
    {
        // Arrange
        var command = new TokenRefreshRequest(TestData.AccessToken!, TestData.RefreshToken!);
        var authRepository = new MockAuthenticationRepository(TestData.TestApplicationUser);
        var configuration = new Mock<IConfiguration>();
        var tokenService = new MockTokenService();
        var tokenValidationParameters = new Mock<TokenValidationParameters>();
        var logger = new Mock<ILogger<ITokenService>>();
        var tokenHandler = new TokenHandler();

        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            var result = await tokenHandler.Handle
            (
                command,
                authRepository.Object,
                tokenService.Object,
                tokenValidationParameters.Object,
                null!,
                logger.Object
            );
        });
    }

    [Fact]
    public async Task With_Null_Logger_Parameter_Throws_ArgumentNullException()
    {
        // Arrange
        var command = new TokenRefreshRequest(TestData.AccessToken!, TestData.RefreshToken!);
        var authRepository = new MockAuthenticationRepository(TestData.TestApplicationUser);
        var configuration = new Mock<IConfiguration>();
        var tokenService = new MockTokenService();
        var tokenValidationParameters = new Mock<TokenValidationParameters>();
        var logger = new Mock<ILogger<TokenHandler>>();
        var tokenHandler = new TokenHandler();

        // Act

        // Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            var result = await tokenHandler.Handle
            (
                command,
                authRepository.Object,
                tokenService.Object,
                tokenValidationParameters.Object,
                configuration.Object,
                null!
            );
        });
    }

    [Fact]
    public async Task With_Null_Principal_Returns_401()
    {
        // Arrange
        var command = new TokenRefreshRequest(TestData.AccessToken!, TestData.RefreshToken!);
        var user = TestData.TestApplicationUser;
        user.EmailConfirmed = true;
        var authRepository = new MockAuthenticationRepository(user);
        var tokenService = new MockTokenService(new MockTokenServiceOptions { Token = null! });
        var tokenValidationParameters = new Mock<TokenValidationParameters>();
        var configuration = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<ITokenService>>();
        var tokenHandler = new TokenHandler();

        // Act
        var result = await tokenHandler.Handle
        (
            command,
            authRepository.Object,
            tokenService.Object,
            tokenValidationParameters.Object,
            configuration.Object,
            logger.Object
        );

        // Assert
        Assert.NotNull(result);
        Assert.IsType<TokenResponse>(result);
        Assert.NotNull(result.ApiError);
        Assert.True(result.ApiError?.StatusCode is (int)HttpStatusCode.Unauthorized);
        Assert.True(result.ApiError?.HttpStatusCode == ((int)HttpStatusCode.Unauthorized).ToString());
        Assert.Equal(AuthConstants.Unauthorised, result.ApiError?.ErrorMessage);
    }

    [Fact]
    public async Task With_Null_User_Returns_401()
    {
        // Arrange
        var command = new TokenRefreshRequest(TestData.AccessToken!, TestData.RefreshToken!);
        var authRepository = new MockAuthenticationRepository(nullEmail: true);
        var tokenService = new MockTokenService(new MockTokenServiceOptions { Token = command.RefreshToken });
        var tokenValidationParameters = new Mock<TokenValidationParameters>();
        var configuration = new Mock<IConfiguration>();
        var logger = new Mock<ILogger<ITokenService>>();
        var tokenHandler = new TokenHandler();

        // Act
        var result = await tokenHandler.Handle
        (
            command,
            authRepository.Object,
            tokenService.Object,
            tokenValidationParameters.Object,
            configuration.Object,
            logger.Object
        );

        // Assert
        Assert.NotNull(result);
        Assert.IsType<TokenResponse>(result);
        Assert.NotNull(result.ApiError);
        Assert.True(result.ApiError?.StatusCode is (int)HttpStatusCode.Unauthorized);
        Assert.True(result.ApiError?.HttpStatusCode == ((int)HttpStatusCode.Unauthorized).ToString());
        Assert.Equal(AuthConstants.Unauthorised, result.ApiError?.ErrorMessage);
    }

    [Fact]
    public async Task Returns_TokenResponse()
    {
        // Arrange
        var command = new TokenRefreshRequest(TestData.AccessToken!, TestData.RefreshToken!);
        var authRepository = new MockAuthenticationRepository(TestData.TestApplicationUser);
        var configuration = new Mock<IConfiguration>();
        var tokenService = new MockTokenService(new MockTokenServiceOptions { Token = command.AccessToken });
        var tokenValidationParameters = new Mock<TokenValidationParameters>();
        var logger = new Mock<ILogger<ITokenService>>();
        var tokenHandler = new TokenHandler();

        // Act
        var result = await tokenHandler.Handle
        (
            command,
            authRepository.Object,
            tokenService.Object,
            tokenValidationParameters.Object,
            configuration.Object,
            logger.Object
        );

        // Assert
        Assert.NotNull(result);
        Assert.IsType<TokenResponse>(result);
        Assert.Null(result.ApiError);
        Assert.True(result.Succeeded == true);
        Assert.NotNull(result.JwtToken);
        Assert.NotNull(result.RefreshToken);
    }
}