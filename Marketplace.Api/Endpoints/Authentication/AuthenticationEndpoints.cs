using System.Net;
using Marketplace.Api.Endpoints.Authentication.Login;
using Marketplace.Api.Endpoints.Authentication.Registration;
using Marketplace.Api.Endpoints.Authentication.Token;
using Marketplace.Core.Constants;
using Microsoft.AspNetCore.Http.HttpResults;
using Newtonsoft.Json;
using Wolverine;

namespace Marketplace.Api.Endpoints.Authentication
{
    public static class AuthenticationEndpoints
    {
        public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder routes)
        {
            // Login endpoint
            routes.MapPost(ApiConstants.ApiSlashLogin, async (LoginRequest command, IMessageBus bus) =>
                {
                    var response = await bus.InvokeAsync<LoginResponse>(command);

                    if (response.ApiError is not null)
                    {
                        return Results.Problem(
                            detail: response.ApiError?.ErrorMessage, 
                            statusCode: response.ApiError?.StatusCode,
                            type: response.ApiError?.HttpStatusCode,
                            title: "Login endpoint.");
                    }

                    if (response.Succeeded.HasValue && response.Succeeded.Value)
                    {
                        return Results.Ok(response);
                    }

                    return Results.UnprocessableEntity(response);
                })
            .AllowAnonymous()
            .WithTags(ApiConstants.Authentication)
            .WithName(ApiConstants.Login)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

            // Registration endpoint
            routes.MapPost(ApiConstants.ApiSlashRegister, async (RegisterRequest command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<RegisterStepOneResponse>(command);
                if (!response.RegistrationStepOne.HasValue && !response.RegistrationStepOne!.Value)
                {
                    return Results.BadRequest();
                }

                if(response.ApiError != null)
                {
                    return Results.Problem(response.ApiError?.ErrorMessage);
                }

                if (response.Errors!.Any())
                {
                    return Results.Problem(JsonConvert.SerializeObject(response.Errors));
                }

                return Results.Ok(JsonConvert.SerializeObject(response));
            })
            .AllowAnonymous()
            .WithTags(ApiConstants.Authentication)
            .WithName(ApiConstants.Register)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);
            
            // Email confirmation endpoint
            routes.MapGet(ApiConstants.ApiSlashConfirmEmail, async (ConfirmEmailRequest command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<ConfirmEmailResponse>(command);
                
                if (response.ApiError is not null)
                {
                    return Results.Problem(
                        detail: response.ApiError?.ErrorMessage, 
                        statusCode: response.ApiError?.StatusCode,
                        type: response.ApiError?.HttpStatusCode,
                        title: "Login endpoint.");
                }

                if (!string.IsNullOrEmpty(response.ConfirmationCode))
                {
                    return Results.Ok(response);
                }

                return Results.UnprocessableEntity(response);                
            })
            .AllowAnonymous()
            .WithTags(ApiConstants.Authentication)
            .WithName(ApiConstants.ConfirmEmail)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);
            
            // Registration step two endpoint
            routes.MapPost("/api/register/step-two", async (RegisterStepTwoRequest command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<RegisterStepTwoResponse>(command);
                
                if (response.ApiError is not null)
                {
                    return Results.Problem(
                        detail: response.ApiError?.ErrorMessage, 
                        statusCode: response.ApiError?.StatusCode,
                        type: response.ApiError?.HttpStatusCode,
                        title: "Registration step two endpoint.");
                }

                if (response.RegistrationStepTwo.HasValue && response.RegistrationStepTwo.Value)
                {
                    return Results.Ok(JsonConvert.SerializeObject(response));
                }

                return Results.UnprocessableEntity(response);
            })
            .AllowAnonymous()
            .WithTags(ApiConstants.Authentication)
            .WithName("RegisterStepTwo")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);                

            // Refresh token endpoint
            routes.MapPost(ApiConstants.ApiRefresh, async (TokenRefreshRequest command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<TokenResponse>(command);

                if (!response.Succeeded!.Value && response.ApiError == null)
                {
                    return Results.BadRequest();
                }

                if (response.ApiError != null)
                {
                    if (response.ApiError.StatusCode == 401)
                    {
                        return Results.Unauthorized();
                    }
                    else
                    {
                        return Results.Problem(response.ApiError?.ErrorMessage);
                    }
                }
                else
                {
                    return Results.Ok(JsonConvert.SerializeObject(response));
                }
            })
            .RequireAuthorization()
            .WithTags(ApiConstants.Authentication)
            .WithName(ApiConstants.Refresh)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

            // Revoke token endpoint
            routes.MapPost(ApiConstants.ApiSlashLogout, async (TokenRevokeRequest command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<TokenResponse>(command);

                if (!response.Succeeded!.Value && response.ApiError == null)
                {
                    return Results.BadRequest();
                }

                if (response.ApiError is not null)
                {
                    return Results.Problem(response.ApiError?.ErrorMessage);
                }

                return Results.Ok(JsonConvert.SerializeObject(response));
            })
            .WithTags(ApiConstants.Authentication)
            .WithName(ApiConstants.Revoke)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);
        }
    }
}
