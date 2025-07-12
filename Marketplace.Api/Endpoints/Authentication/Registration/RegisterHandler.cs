using Marketplace.Core;
using Marketplace.Core.Constants;
using Marketplace.Data;
using Marketplace.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Wolverine.Attributes;
using Marketplace.Core.Validation;
using Newtonsoft.Json;

namespace Marketplace.Api.Endpoints.Authentication.Registration
{
    [WolverineHandler]
    public class RegisterHandler
    {
        private const string RegistrationSuccessfulForUser = "Registration successful for user";
        private const string RegistrationUnsuccessfulForUser = "Registration unsuccessful for user";
        private const string RegistrationFailed = "Registration failed";
        private const string RegistrationStepTwoSuccessful = "Registration step two successful";
        private const string RegistrationStepTwoFailed = "Registration step two failed";
        
        [Transactional]
        public async Task<RegisterStepOneResponse> Handle(RegisterRequest command, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, ILogger<RegisterHandler> logger, MarketplaceDbContext dbContext, IValidationService validationService)
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
                    ApiError = new Core.ApiError(
                        HttpStatusCode: StatusCodes.Status400BadRequest.ToString(),
                        StatusCode: StatusCodes.Status400BadRequest,
                        ErrorMessage: "Validation failed",
                        StackTrace: JsonConvert.SerializeObject(validationErrors)
                    )
                };
            }

            var user = await userManager.FindByEmailAsync(command.Email);
            if (user is not null)
            {
                logger.LogError(ApiConstants.UserAlreadyExists);
                return new RegisterStepOneResponse
                {
                    RegistrationStepOne = false,
                    ApiError = new Core.ApiError(
                        HttpStatusCode: StatusCodes.Status500InternalServerError.ToString(),
                        StatusCode: StatusCodes.Status500InternalServerError,
                        ErrorMessage: AuthConstants.UserAlreadyExists,
                        StackTrace: null)
                };
            }

            user = BuildApplicationUser(command);

            var registrationResponse = new RegisterStepOneResponse();

            var result = await userManager.CreateAsync(user, command.Password);

            IdentityRole? role = null;
            
            role = await CreateRoleAsync(userManager, roleManager, logger, dbContext, user);

            // if succeeded ensure the role is created and if that fails make sure
            // the user is deleted so it doesn't cause a duplicate user validation error
            if (result.Succeeded)
            {
                user.EmailConfirmed = false;

                await AddUserToRoleAsync(userManager, logger, dbContext, user, role);

                // Generate email confirmation token and simple URL
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                registrationResponse.ConfirmationEmailLink = $"/api/confirm_email?userId={Uri.EscapeDataString(user.Id!)}&token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email!)}";
            }
            else
            {
                logger.LogError(RegistrationFailed);
                registrationResponse.ApiError = new Core.ApiError(
                    HttpStatusCode: StatusCodes.Status400BadRequest.ToString(),
                    StatusCode: StatusCodes.Status400BadRequest,
                    ErrorMessage: AuthConstants.RegistrationFailed,
                    StackTrace: null);
            }

            registrationResponse.UserId = user.Id ?? user.UserName;
            registrationResponse.RegistrationStepOne = result.Succeeded;
            registrationResponse.Errors = result.Errors;

            return registrationResponse;
        }

        [Transactional]
        public async Task<ConfirmEmailResponse> Handle(ConfirmEmailRequest command,
            UserManager<ApplicationUser> userManager, ILogger<RegisterHandler> logger, MarketplaceDbContext dbContext)
        {
            ArgumentNullException.ThrowIfNull(command, nameof(command));
            ArgumentNullException.ThrowIfNull(command.UserId, nameof(command.UserId));
            ArgumentNullException.ThrowIfNull(command.Token, nameof(command.Token));

            logger.LogInformation(ApiConstants.ConfirmEmail);

            var user = await userManager.FindByIdAsync(command.UserId);
            
            if (user == null)
            {
                logger.LogError(AuthConstants.UserDoesntExist);
                return new ConfirmEmailResponse
                {
                    ApiError = new Core.ApiError(
                        HttpStatusCode: StatusCodes.Status404NotFound.ToString(),
                        StatusCode: StatusCodes.Status404NotFound,
                        ErrorMessage: AuthConstants.UserDoesntExist,
                        StackTrace: null)
                };
            }

            var result = await userManager.ConfirmEmailAsync(user, command.Token);
            
            if (!result.Succeeded)
            {
                logger.LogError("Email confirmation failed for user {UserId}", command.UserId);
                return new ConfirmEmailResponse
                {
                    ApiError = new Core.ApiError(
                        HttpStatusCode: StatusCodes.Status400BadRequest.ToString(),
                        StatusCode: StatusCodes.Status400BadRequest,
                        ErrorMessage: "Email confirmation failed",
                        StackTrace: null)
                };
            }

            logger.LogInformation("Email confirmed for user {UserId}", command.UserId);
            
            return new ConfirmEmailResponse
            {
                UserId = user.Id,
                Email = user.Email,
                ConfirmationCode = "EmailConfirmed"
            };
        }
        
        [Transactional]
        public async Task<RegisterStepTwoResponse> Handle(RegisterStepTwoRequest command,
            UserManager<ApplicationUser> userManager, ILogger<RegisterHandler> logger, MarketplaceDbContext dbContext)
        {
            ArgumentNullException.ThrowIfNull(command, nameof(command));
            ArgumentNullException.ThrowIfNull(command.UserId, nameof(command.UserId));
            ArgumentNullException.ThrowIfNull(command.Email, nameof(command.Email));
            ArgumentNullException.ThrowIfNull(command.Token, nameof(command.Token));
            
            logger.LogInformation("Registration step two handler called for user {UserId}", command.UserId);
            
            var user = await userManager.FindByIdAsync(command.UserId);
            
            if (user == null)
            {
                logger.LogError(AuthConstants.UserDoesntExist);
                return new RegisterStepTwoResponse
                {
                    RegistrationStepTwo = false,
                    ApiError = new Core.ApiError(
                        HttpStatusCode: StatusCodes.Status404NotFound.ToString(),
                        StatusCode: StatusCodes.Status404NotFound,
                        ErrorMessage: AuthConstants.UserDoesntExist,
                        StackTrace: null)
                };
            }
            
            // Verify the email confirmation token
            var result = await userManager.ConfirmEmailAsync(user, command.Token);
            
            if (!result.Succeeded)
            {
                logger.LogError(RegistrationStepTwoFailed);
                return new RegisterStepTwoResponse
                {
                    RegistrationStepTwo = false,
                    ApiError = new Core.ApiError(
                        HttpStatusCode: StatusCodes.Status400BadRequest.ToString(),
                        StatusCode: StatusCodes.Status400BadRequest,
                        ErrorMessage: "Email confirmation failed",
                        StackTrace: null),
                    Errors = result.Errors
                };
            }
            
            // Update user's email confirmation status
            user.EmailConfirmed = true;
            await userManager.UpdateAsync(user);
            await dbContext.SaveChangesAsync();
            
            logger.LogInformation(RegistrationStepTwoSuccessful);
            
            return new RegisterStepTwoResponse
            {
                UserId = user.Id,
                RegistrationStepTwo = true,
                ConfirmationCode = "RegistrationComplete"
            };
        }

        private static async Task<IdentityRole> CreateRoleAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, ILogger<RegisterHandler> logger,
            MarketplaceDbContext dbContext, ApplicationUser user)
        {
            IdentityRole? role = null;
            try
            {
                role = new IdentityRole(AuthConstants.UserRole);
                var roleManagerCreateResult = await roleManager.CreateAsync(role);
                if (roleManagerCreateResult.Succeeded)
                {
                    logger.LogInformation(AuthConstants.CreatedRoleUser);   
                }
                else
                {
                    logger.LogError(AuthConstants.ErrorCreatingRoleUser);
                }
            }
            catch (Exception ex)
            {
                var deleteUserResult = await userManager.DeleteAsync(user);
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
            finally
            {
                await dbContext.SaveChangesAsync();
            }

            return role = role ?? new IdentityRole(AuthConstants.UserRole);
        }


        private static async Task AddUserToRoleAsync(UserManager<ApplicationUser> userManager, ILogger<RegisterHandler> logger, MarketplaceDbContext dbContext,
            ApplicationUser user, IdentityRole role)
        {
            try
            {
                await userManager.AddToRoleAsync(user, role.Name!);
                logger.LogInformation($"{RegistrationSuccessfulForUser} {user.UserName}");
            }
            catch (Exception ex)
            {
                logger.LogInformation($"{RegistrationUnsuccessfulForUser} {user.UserName}");
                logger.LogError(ex.Message);
                await userManager.DeleteAsync(user);
            }
            finally
            {
                await dbContext.SaveChangesAsync();
            }
        }

        private static ApplicationUser BuildApplicationUser(RegisterRequest command)
        {
            var user = new ApplicationUser()
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
}