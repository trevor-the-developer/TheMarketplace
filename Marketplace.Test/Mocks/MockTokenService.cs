using System.Security.Claims;
using Marketplace.Core.Interfaces;
using Marketplace.Data.Entities;
using Marketplace.Data.Interfaces;
using Marketplace.Test.Data;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Moq;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Marketplace.Test.Mocks;

public class MockTokenService : Mock<ITokenService>
{
    public MockTokenService(MockTokenServiceOptions? options = null)
    {
        options ??= new MockTokenServiceOptions();

        // Setup JWT token generation
        Setup(x => x.GenerateJwtSecurityTokenAsync(
                It.IsAny<IAuthenticationRepository>(),
                It.IsAny<ApplicationUser>(),
                It.IsAny<IConfiguration>()))
            .ReturnsAsync(options.ReturnNullToken ? null! : TestData.JwtSecurityToken);

        // Setup refresh token generation
        Setup(x => x.GenerateRefreshToken())
            .Returns(options.ReturnNullRefreshToken ? null! : TestData.RefreshToken!);

        // Setup principal from expired token with correct test data
        if (!string.IsNullOrEmpty(options.Token))
            Setup(x => x.GetPrincipalFromExpiredToken(
                    It.IsAny<string>(), // Changed to accept any string to handle multiple calls
                    It.IsAny<TokenValidationParameters>(),
                    It.IsAny<IConfiguration>(),
                    It.IsAny<ILogger<ITokenService>>()))
                .Returns(() => new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new(ClaimTypes.Name, TestData.TestUserOne), // Use TestData email
                    new(ClaimTypes.NameIdentifier, TestData.TestId),
                    new("name", "Test User")
                }, "TestAuthType")));
        else
            Setup(x => x.GetPrincipalFromExpiredToken(
                    It.IsAny<string>(),
                    It.IsAny<TokenValidationParameters>(),
                    It.IsAny<IConfiguration>(),
                    It.IsAny<ILogger<ITokenService>>()))
                .Returns((ClaimsPrincipal?)null);
    }
}

public class MockTokenServiceOptions
{
    public bool ReturnNullToken { get; init; }
    public bool ReturnNullRefreshToken { get; init; }
    public string? Token { get; init; }
}