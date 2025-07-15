using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Marketplace.Data.Entities;
using Marketplace.Data.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Marketplace.Core.Security;

public interface ITokenService
{
    Task<JwtSecurityToken> GenerateJwtSecurityTokenAsync(IAuthenticationRepository authenticationRepository,
        ApplicationUser applicationUser, IConfiguration configuration);

    string GenerateRefreshToken();

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token,
        TokenValidationParameters tokenValidationParameters, IConfiguration configuration,
        ILogger<ITokenService> logger);
}