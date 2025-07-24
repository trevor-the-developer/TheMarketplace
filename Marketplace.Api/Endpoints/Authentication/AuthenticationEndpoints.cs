using Marketplace.Api.Endpoints.Authentication.Login;
using Marketplace.Api.Endpoints.Authentication.Registration;
using Marketplace.Api.Endpoints.Authentication.Token;
using Marketplace.Core.Constants;
using Wolverine;

namespace Marketplace.Api.Endpoints.Authentication;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder routes)
    {
        // Login endpoint
        routes.MapPost(ApiConstants.ApiSlashLogin, async (LoginRequest command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<LoginResponse>(command);

                if (response.ApiError is not null)
                    return Results.Problem(
                        response.ApiError?.ErrorMessage,
                        statusCode: response.ApiError?.StatusCode,
                        type: response.ApiError?.HttpStatusCode,
                        title: "Login endpoint.");

                if (response.Succeeded.HasValue && response.Succeeded.Value) return Results.Ok(response);

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
                    return Results.BadRequest();

                if (response.ApiError != null)
                    return Results.Problem(
                        response.ApiError?.ErrorMessage,
                        statusCode: response.ApiError?.StatusCode,
                        type: response.ApiError?.HttpStatusCode,
                        title: "Registration endpoint.");

                return response.Errors!.Any()
                    ? Results.BadRequest(new { errors = response.Errors })
                    : Results.Ok(response);
            })
            .AllowAnonymous()
            .WithTags(ApiConstants.Authentication)
            .WithName(ApiConstants.Register)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);

        // Email confirmation endpoint
        routes.MapGet(ApiConstants.ApiSlashConfirmEmail,
                async ([AsParameters] ConfirmEmailRequest command, IMessageBus bus) =>
                {
                    var response = await bus.InvokeAsync<ConfirmEmailResponse>(command);

                    if (response.ApiError is not null)
                        return Results.Problem(
                            response.ApiError?.ErrorMessage,
                            statusCode: response.ApiError?.StatusCode,
                            type: response.ApiError?.HttpStatusCode,
                            title: "Login endpoint.");

                    return !string.IsNullOrEmpty(response.ConfirmationCode)
                        ? Results.Ok(response)
                        : Results.UnprocessableEntity(response);
                })
            .AllowAnonymous()
            .WithTags(ApiConstants.Authentication)
            .WithName(ApiConstants.ConfirmEmail)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status409Conflict)
            .Produces(StatusCodes.Status500InternalServerError);

        // Refresh token endpoint
        routes.MapPost(ApiConstants.ApiRefresh, async (TokenRefreshRequest command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<TokenResponse>(command);

                if (!response.Succeeded!.Value && response.ApiError == null) return Results.BadRequest();

                if (response.ApiError == null) return Results.Ok(response);
                return response.ApiError.StatusCode == 401
                    ? Results.Unauthorized()
                    : Results.Problem(response.ApiError?.ErrorMessage);
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

                if (!response.Succeeded!.Value && response.ApiError == null) return Results.BadRequest();

                return response.ApiError is not null
                    ? Results.Problem(response.ApiError?.ErrorMessage)
                    : Results.Ok(response);
            })
            .WithTags(ApiConstants.Authentication)
            .WithName(ApiConstants.Revoke)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}