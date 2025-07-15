using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Marketplace.Core;
using Marketplace.Core.Constants;
using Marketplace.Core.Security;
using Marketplace.Data.Entities;
using Marketplace.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Wolverine.Attributes;

namespace Marketplace.Api.Endpoints.Authentication.Token;

[WolverineHandler]
public class TokenHandler
{
    public async Task<TokenResponse> Handle(TokenRefreshRequest command,
        IAuthenticationRepository authenticationRepository,
        ITokenService tokenService, TokenValidationParameters tokenValidationParameters,
        IConfiguration configuration, ILogger<ITokenService> logger)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(authenticationRepository, nameof(authenticationRepository));
        ArgumentNullException.ThrowIfNull(tokenService, nameof(tokenService));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        logger.LogInformation(AuthConstants.TokenRefreshRequest);

        ClaimsPrincipal? principal;
        // surface the underlying error
        try
        {
            principal = tokenService.GetPrincipalFromExpiredToken(command.AccessToken, tokenValidationParameters,
                configuration, logger);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Invalid token.   {e.Message}");
            return new TokenResponse
            {
                Succeeded = false,
                RefreshToken = command.RefreshToken,
                ApiError = new ApiError(
                    StatusCodes.Status401Unauthorized.ToString(),
                    StatusCodes.Status401Unauthorized,
                    $"Invalid token.   {e.Message}",
                    e.StackTrace
                )
            };
        }

        if (principal?.Identity?.Name is null)
        {
            logger.LogError(AuthConstants.Unauthorised);
            return Unauthorised();
        }

        var user = await authenticationRepository.FindUserByNameAsync(principal.Identity.Name);

        if (user is null || user.RefreshToken != command.RefreshToken || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            logger.LogError(AuthConstants.Unauthorised);
            return Unauthorised();
        }

        var token = await tokenService.GenerateJwtSecurityTokenAsync(authenticationRepository, user, configuration);

        logger.LogInformation(AuthConstants.RefreshSucceeded);

        return new TokenResponse
        {
            Succeeded = true,
            JwtToken = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = token.ValidTo,
            RefreshToken = command.RefreshToken
        };
    }

    public async Task<TokenResponse> Handle(TokenRevokeRequest command,
        UserManager<ApplicationUser> userManager, ITokenService tokenService,
        TokenValidationParameters tokenValidationParameters,
        ILogger<ITokenService> logger, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(userManager, nameof(userManager));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));

        logger.LogInformation(AuthConstants.TokenRevokeRequest);

        var principal =
            tokenService.GetPrincipalFromExpiredToken(command.AccessToken, tokenValidationParameters, configuration,
                logger);

        if (principal?.Identity?.Name is null)
        {
            logger.LogError(AuthConstants.Unauthorised);
            return Unauthorised();
        }

        var user = await userManager.FindByNameAsync(principal.Identity.Name);
        if (user is null) return Unauthorised();

        var tokenResponse = new TokenResponse();

        user.RefreshToken = null;
        var result = await userManager.UpdateAsync(user);
        if (result.Succeeded)
        {
            logger.LogInformation(AuthConstants.RevokeSucceeded);
            tokenResponse.Succeeded = true;
        }
        else
        {
            logger.LogError(AuthConstants.RevokeFailed);
            tokenResponse.ApiError = new ApiError(
                StatusCodes.Status500InternalServerError.ToString(),
                StatusCodes.Status500InternalServerError,
                AuthConstants.RevokeFailed,
                JsonConvert.SerializeObject(result.Errors)
            );
        }

        return tokenResponse;
    }

    private static TokenResponse Unauthorised()
    {
        return new TokenResponse
        {
            Succeeded = false,
            ApiError = new ApiError(
                StatusCodes.Status401Unauthorized.ToString(),
                StatusCodes.Status401Unauthorized,
                AuthConstants.Unauthorised,
                null)
        };
    }
}