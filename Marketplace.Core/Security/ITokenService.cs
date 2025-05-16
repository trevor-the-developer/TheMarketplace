using Marketplace.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Marketplace.Core.Security
{
    public interface ITokenService
    {
        Task<JwtSecurityToken> GenerateJwtSecurityTokenAsync(UserManager<ApplicationUser> userManager, ApplicationUser applicationUser, IConfiguration configuration);
        string GenerateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token,
            TokenValidationParameters tokenValidationParameters, IConfiguration configuration, ILogger<ITokenService> logger);
    }
}