using Marketplace.Core;
using Marketplace.Core.Constants;
using Marketplace.Core.Services;
using Marketplace.Core.Validation;
using Marketplace.Data.Entities;
using Marketplace.Data.Interfaces;
using Marketplace.Data.Repositories;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Wolverine.Attributes;

namespace Marketplace.Api.Endpoints.Authentication.Registration;

[WolverineHandler]
public class RegisterHandler
{
    private const string RegistrationSuccessfulForUser = "Registration successful for user";
    private const string RegistrationUnsuccessfulForUser = "Registration unsuccessful for user";
    private const string RegistrationFailed = "Registration failed";

    [Transactional]
    public async Task<RegisterStepOneResponse> Handle(RegisterRequest command,
        IAuthenticationRepository authenticationRepository,
        ILogger<RegisterHandler> logger, IValidationService validationService, IEmailService emailService,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        logger.LogInformation(ApiConstants.RegisterHandlerCalled);

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
        {
            logger.LogError("Registration request validation failed: {Errors}", string.Join(", ", validationErrors));
            return new RegisterStepOneResponse
            {
                RegistrationStepOne = false,
                ApiError = new ApiError(
                    StatusCodes.Status400BadRequest.ToString(),
                    StatusCodes.Status400BadRequest,
                    "Validation failed",
                    JsonConvert.SerializeObject(validationErrors)
                )
            };
        }

        var user = await authenticationRepository.FindUserByEmailAsync(command.Email);
        if (user is not null)
        {
            logger.LogError(ApiConstants.UserAlreadyExists);
            return new RegisterStepOneResponse
            {
                RegistrationStepOne = false,
                ApiError = new ApiError(
                    StatusCodes.Status500InternalServerError.ToString(),
                    StatusCodes.Status500InternalServerError,
                    AuthConstants.UserAlreadyExists,
                    null)
            };
        }

        user = BuildApplicationUser(command);

        var registrationResponse = new RegisterStepOneResponse();

        var result = await authenticationRepository.CreateUserAsync(user, command.Password);

        IdentityRole? role = null;

        role = await CreateRoleAsync(authenticationRepository, logger, user);

        // if succeeded ensure the role is created and if that fails make sure
        // the user is deleted so it doesn't cause a duplicate user validation error
        if (result.Succeeded)
        {
            user.EmailConfirmed = false;

            await AddUserToRoleAsync(authenticationRepository, logger, user, role);

            // Generate email confirmation token and absolute URL
            var token = await authenticationRepository.GenerateEmailConfirmationTokenAsync(user);
            var frontendBaseUrl = configuration["FrontendSettings:BaseUrl"] ?? ApiConstants.DefaultFrontendBaseUrl;
            registrationResponse.ConfirmationEmailLink =
                $"{frontendBaseUrl}/api/auth/confirm-email?userId={Uri.EscapeDataString(user.Id!)}\u0026token={Uri.EscapeDataString(token)}\u0026email={Uri.EscapeDataString(user.Email!)}";

            // Send confirmation email
            try
            {
                await emailService.SendConfirmationEmailAsync(user.Email!, registrationResponse.ConfirmationEmailLink);
                logger.LogInformation("Confirmation email sent to {Email}", user.Email);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send confirmation email to {Email}", user.Email);
                registrationResponse.ApiError = new ApiError(
                    StatusCodes.Status500InternalServerError.ToString(),
                    StatusCodes.Status500InternalServerError,
                    "Failed to send confirmation email",
                    null);
            }
        }
        else
        {
            logger.LogError(RegistrationFailed);
            registrationResponse.ApiError = new ApiError(
                StatusCodes.Status400BadRequest.ToString(),
                StatusCodes.Status400BadRequest,
                AuthConstants.RegistrationFailed,
                null);
        }

        registrationResponse.UserId = user.Id ?? user.UserName;
        registrationResponse.RegistrationStepOne = result.Succeeded;
        registrationResponse.Errors = result.Errors;

        return registrationResponse;
    }

    [Transactional]
    public async Task<ConfirmEmailResponse> Handle(ConfirmEmailRequest command,
        IAuthenticationRepository authenticationRepository, ILogger<RegisterHandler> logger)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(command.UserId, nameof(command.UserId));
        ArgumentNullException.ThrowIfNull(command.Token, nameof(command.Token));

        logger.LogInformation(ApiConstants.ConfirmEmail);

        var user = await authenticationRepository.FindUserByIdAsync(command.UserId);

        if (user == null)
        {
            logger.LogError(AuthConstants.UserDoesntExist);
            return new ConfirmEmailResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status404NotFound.ToString(),
                    StatusCodes.Status404NotFound,
                    AuthConstants.UserDoesntExist,
                    null)
            };
        }

        var result = await authenticationRepository.ConfirmEmailAsync(user, command.Token);

        if (!result.Succeeded)
        {
            logger.LogError("Email confirmation failed for user {UserId}", command.UserId);
            return new ConfirmEmailResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status400BadRequest.ToString(),
                    StatusCodes.Status400BadRequest,
                    "Email confirmation failed",
                    null)
            };
        }

        // Complete registration in one step
        user.EmailConfirmed = true;
        await authenticationRepository.UpdateUserAsync(user);

        logger.LogInformation("Email confirmed and registration completed for user {UserId}", command.UserId);

        return new ConfirmEmailResponse
        {
            UserId = user.Id,
            Email = user.Email,
            ConfirmationCode = "RegistrationComplete",
            RegistrationCompleted = true
        };
    }

    private static async Task<IdentityRole> CreateRoleAsync(IAuthenticationRepository authenticationRepository,
        ILogger<RegisterHandler> logger,
        ApplicationUser user)
    {
        IdentityRole? role = null;
        try
        {
            role = new IdentityRole(AuthConstants.UserRole);
            var roleManagerCreateResult = await authenticationRepository.CreateRoleAsync(AuthConstants.UserRole);
            if (roleManagerCreateResult.Succeeded)
                logger.LogInformation(AuthConstants.CreatedRoleUser);
            else
                logger.LogError(AuthConstants.ErrorCreatingRoleUser);
        }
        catch (Exception ex)
        {
            var deleteUserResult = await authenticationRepository.DeleteUserAsync(user);
            if (deleteUserResult.Succeeded)
            {
                logger.LogError(AuthConstants.ErrorCreatingRoleUser);
                logger.LogError(ex.Message);
            }
            else
            {
                logger.LogError(AuthConstants.ErrorCreatingRoleUser);
                throw new SystemException(AuthConstants.ErrorCreatingRoleUser);
            }
        }

        return role = role ?? new IdentityRole(AuthConstants.UserRole);
    }


    private static async Task AddUserToRoleAsync(IAuthenticationRepository authenticationRepository,
        ILogger<RegisterHandler> logger,
        ApplicationUser user, IdentityRole role)
    {
        try
        {
            await authenticationRepository.AddToRoleAsync(user, role.Name!);
            logger.LogInformation($"{RegistrationSuccessfulForUser} {user.UserName}");
        }
        catch (Exception ex)
        {
            logger.LogInformation($"{RegistrationUnsuccessfulForUser} {user.UserName}");
            logger.LogError(ex.Message);
            await authenticationRepository.DeleteUserAsync(user);
        }
    }

    private static ApplicationUser BuildApplicationUser(RegisterRequest command)
    {
        var user = new ApplicationUser
        {
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            UserName = command.Email,
            DateOfBirth = command.DateOfBirth
        };
        return user;
    }
}