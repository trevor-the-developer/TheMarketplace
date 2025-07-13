using Marketplace.Data.Entities;
using Marketplace.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Marketplace.Core.Constants;
using Microsoft.Extensions.Logging;

namespace Marketplace.Core.Security
{
    public class TokenService : ITokenService
    {
        public async Task<JwtSecurityToken> GenerateJwtSecurityTokenAsync(IAuthenticationRepository authenticationRepository, 
            ApplicationUser applicationUser, IConfiguration configuration)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"] ?? string.Empty));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var roles = await authenticationRepository.GetRolesAsync(applicationUser);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();
            var userClaims = await authenticationRepository.GetClaimsAsync(applicationUser);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Name, applicationUser.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.Sub, applicationUser.Email ?? string.Empty),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new (JwtRegisteredClaimNames.Email, applicationUser.Email ?? string.Empty),
                new ("userId", applicationUser.Id),
            }.Union(userClaims).Union(roleClaims);

            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(Convert.ToInt32(configuration["JwtSettings:DurationInHours"])).AddMinutes(30),
                signingCredentials: credentials
                );

            return token;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        /// <summary>
        /// Get's a <see cref="ClaimsPrincipal"/> for the given token
        /// </summary>
        /// <param name="token">The JWT Token</param>
        /// <param name="tokenValidationParameters">Contains a set of parameters that are used by a
        /// <see cref="SecurityTokenHandler"/> when validating a <see cref="SecurityToken"/>.</param>
        /// <param name="configuration">The configuration</param>
        /// <returns>Validated <see cref="ClaimsPrincipal"/></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token, 
            TokenValidationParameters tokenValidationParameters, IConfiguration configuration, ILogger<ITokenService> logger)
        {
            var secret = configuration[AuthConstants.JwtSettingsKey] ??
                         throw new InvalidOperationException(AuthConstants.SecretKeyNotConfigured);

            tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                ValidateIssuer = true,
                ValidIssuer = configuration[AuthConstants.JwtSettingsIssuer],
                ValidateAudience = true,
                ValidAudience = configuration[AuthConstants.JwtSettingsAudience],
                ValidateLifetime = false,
                NameClaimType = "sub",
                RoleClaimType = "role",
                ClockSkew = TimeSpan.Zero
            };

            ClaimsPrincipal? validationResult;
            
            try
            {
                validationResult = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out _);
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Invalid token.   {e.Message}");
                return null;
            }
            return validationResult;
        }
    }
}
