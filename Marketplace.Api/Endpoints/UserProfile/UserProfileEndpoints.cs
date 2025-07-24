using Marketplace.Core.Constants;
using Marketplace.Core.Models;
using Marketplace.Core.Models.UserProfile;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Marketplace.Api.Endpoints.UserProfile;

public static class UserProfileEndpoints
{
    public static void MapUserProfileEndpoints(this IEndpointRouteBuilder routes)
    {
        // POST /api/user-profiles - Create new user profile
        routes.MapPost(ApiConstants.ApiUserProfiles, async (UserProfileCreate command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<UserProfileResponse>(command);
                return Results.Created($"/api/user-profiles/{response.UserProfile?.ApplicationUserId}", response);
            })
            .RequireAuthorization()
            .WithTags("UserProfiles")
            .WithName("Create UserProfile")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        // PUT /api/user-profiles/{id} - Update existing user profile
        routes.MapPut(ApiConstants.ApiUserProfilesById, async (string id, UserProfileUpdate command, IMessageBus bus) =>
            {
                if (id != command.ApplicationUserId) return Results.BadRequest();

                var response = await bus.InvokeAsync<UserProfileResponse>(command);
                return response.UserProfile == null ? Results.NotFound() : Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags("UserProfiles")
            .WithName("Update UserProfile")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE /api/user-profiles/{id} - Delete user profile
        routes.MapDelete(ApiConstants.ApiUserProfilesById, async (string id, IMessageBus bus) =>
            {
                await bus.InvokeAsync(new UserProfileDelete { ApplicationUserId = id });
                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithTags("UserProfiles")
            .WithName("Delete UserProfile")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/user-profiles - Get all user profiles
        routes.MapGet(ApiConstants.ApiUserProfiles, async (IMessageBus bus) =>
            {
                var command = new UserProfileRequest { AllUserProfiles = true };
                var response = await bus.InvokeAsync<UserProfileResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(response)
                };
            })
            .RequireAuthorization()
            .WithTags("UserProfiles")
            .WithName("Get All UserProfiles")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/user-profiles/{id} - Get user profile by ID
        routes.MapGet(ApiConstants.ApiUserProfilesById, async (string id, IMessageBus bus) =>
            {
                var command = new UserProfileRequest { ApplicationUserId = id };
                var response = await bus.InvokeAsync<UserProfileResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(response)
                };
            })
            .RequireAuthorization()
            .WithTags("UserProfiles")
            .WithName("Get UserProfile by Id")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}