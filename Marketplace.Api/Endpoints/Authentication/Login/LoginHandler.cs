using System.IdentityModel.Tokens.Jwt;
using Marketplace.Core;
using Marketplace.Core.Constants;
using Marketplace.Core.Security;
using Marketplace.Core.Validation;
using Marketplace.Data.Interfaces;
using Newtonsoft.Json;
using Wolverine.Attributes;

namespace Marketplace.Api.Endpoints.Authentication.Login;

[WolverineHandler]
public class LoginHandler
{
    private const string LoginRequest = "Login request.";

    [Transactional]
    public async Task<LoginResponse> Handle(LoginRequest command, IAuthenticationRepository authenticationRepository,
        IConfiguration configuration, ITokenService tokenService, IValidationService validationService,
        ILogger<LoginHandler> logger)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(authenticationRepository, nameof(authenticationRepository));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        ArgumentNullException.ThrowIfNull(tokenService, nameof(tokenService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));

        logger.LogInformation(LoginRequest);

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Any())
        {
            logger.LogError("Login request validation failed: {Errors}", string.Join(", ", validationErrors));
            return new LoginResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status400BadRequest.ToString(),
                    StatusCodes.Status400BadRequest,
                    "Validation failed",
                    JsonConvert.SerializeObject(validationErrors)
                )
            };
        }

        // try to find the user in the identity store
        var user = await authenticationRepository.FindUserByEmailAsync(command.Email);
        if (user is null)
        {
            logger.LogError(AuthConstants.UserDoesntExist);
            return new LoginResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status401Unauthorized.ToString(),
                    StatusCodes.Status401Unauthorized,
                    AuthConstants.UserDoesntExist,
                    null
                )
            };
        }

        var result = await authenticationRepository.CheckPasswordAsync(user, command.Password);
        if (!result)
        {
            logger.LogError(AuthConstants.InvalidEmailPassword);
            return new LoginResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status401Unauthorized.ToString(),
                    StatusCodes.Status401Unauthorized,
                    AuthConstants.InvalidEmailPassword,
                    null
                )
            };
        }

        if (user.EmailConfirmed.HasValue && !user.EmailConfirmed.Value)
        {
            logger.LogError(AuthConstants.UserEmailNotConfirmed);
            return new LoginResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status401Unauthorized.ToString(),
                    StatusCodes.Status401Unauthorized,
                    AuthConstants.UserEmailNotConfirmed,
                    null
                )
            };
        }

        var token = await tokenService.GenerateJwtSecurityTokenAsync(authenticationRepository, user, configuration);
        ArgumentNullException.ThrowIfNull(token, nameof(token));
        var refreshToken = tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        ArgumentNullException.ThrowIfNull(refreshToken, nameof(refreshToken));
        // developer note: set AddMinutes to 1 when debugging auth-authz related flows
        user.RefreshTokenExpiry = DateTime.UtcNow.AddMinutes(30);

        var loginResponse = new LoginResponse();

        var updateResult = await authenticationRepository.UpdateUserAsync(user);
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
            loginResponse.ApiError = new ApiError(
                StatusCodes.Status500InternalServerError.ToString(),
                StatusCodes.Status500InternalServerError,
                AuthConstants.LoginFailed,
                JsonConvert.SerializeObject(updateResult.Errors)
            );
        }

        logger.LogInformation(AuthConstants.LoginSucceeded);

        return loginResponse;
    }
}