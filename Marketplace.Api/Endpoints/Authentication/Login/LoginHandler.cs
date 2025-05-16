using System.IdentityModel.Tokens.Jwt;
using Marketplace.Api.Endpoints.Authentication.Token;
using Marketplace.Core;
using Marketplace.Core.Constants;
using Marketplace.Core.Security;
using Marketplace.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Wolverine.Attributes;

namespace Marketplace.Api.Endpoints.Authentication.Login;

[WolverineHandler]
public class LoginHandler
{
    private const string LoginRequest = "Login request.";

    [Transactional]
    public async Task<LoginResponse> Handle(LoginRequest command, UserManager<ApplicationUser> userManager, 
        IConfiguration configuration, ITokenService tokenService,
        ILogger<LoginHandler> logger)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(userManager, nameof(userManager));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(tokenService, nameof(tokenService));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        
        logger.LogInformation(LoginRequest);

        // try to find the user in the identity store
        var user = await userManager.FindByEmailAsync(command.Email);
        if (user is null)
        {
            logger.LogError(AuthConstants.UserDoesntExist);
            return new LoginResponse
            {
                ApiError = new Core.ApiError(
                    HttpStatusCode: StatusCodes.Status401Unauthorized.ToString(),
                    StatusCode: StatusCodes.Status401Unauthorized,
                    ErrorMessage: AuthConstants.UserDoesntExist, 
                    StackTrace: null
                )
            };
        }

        var result = await userManager.CheckPasswordAsync(user, command.Password);
        if (!result)
        {
            logger.LogError(AuthConstants.InvalidEmailPassword);
            return new LoginResponse
            {
                ApiError = new Core.ApiError(
                    HttpStatusCode: StatusCodes.Status401Unauthorized.ToString(),
                    StatusCode: StatusCodes.Status401Unauthorized,
                    ErrorMessage: AuthConstants.InvalidEmailPassword, 
                    StackTrace: null
                )
            };
        }

        if (user.EmailConfirmed.HasValue && !user.EmailConfirmed.Value)
        {
            logger.LogError(AuthConstants.UserEmailNotConfirmed);
            return new LoginResponse
            {
                ApiError = new Core.ApiError(
                    HttpStatusCode: StatusCodes.Status401Unauthorized.ToString(),
                    StatusCode: StatusCodes.Status401Unauthorized,
                    ErrorMessage: AuthConstants.UserEmailNotConfirmed, 
                    StackTrace: null
                )
            };
        }

        var token = await tokenService.GenerateJwtSecurityTokenAsync(userManager, user, configuration);
        ArgumentNullException.ThrowIfNull(token, nameof(token));
        var refreshToken = tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        ArgumentNullException.ThrowIfNull(refreshToken, nameof(refreshToken));
        // developer note: set AddMinutes to 1 when debugging auth-authz related flows
        user.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(30);

        var loginResponse = new LoginResponse();

        var updateResult = await userManager.UpdateAsync(user);
        if (updateResult.Succeeded)
        {
            logger.LogInformation(AuthConstants.LoginSucceeded);
            loginResponse.Succeeded = true;
            loginResponse.Email = command.Email;
            loginResponse.RefreshToken = refreshToken;
            loginResponse.SecurityToken = new JwtSecurityTokenHandler().WriteToken(token);
            loginResponse.Expiration = token.ValidTo;
        }
        else
        {
            logger.LogError(AuthConstants.LoginFailed);
            loginResponse.ApiError = new Core.ApiError(
                HttpStatusCode: StatusCodes.Status500InternalServerError.ToString(),
                StatusCode: StatusCodes.Status500InternalServerError,
                ErrorMessage: AuthConstants.LoginFailed,
                StackTrace: JsonConvert.SerializeObject(updateResult.Errors)
            );
        }

        logger.LogInformation(AuthConstants.LoginSucceeded);

        return loginResponse;
    }
}