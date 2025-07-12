using Marketplace.Core.Constants;
using Wolverine;

namespace Marketplace.Api.Endpoints.UserProfile;

public static class UserProfileEndpoints
{
    public static void MapUserProfileEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost(ApiConstants.ApiSlashUserProfileCreate, async (UserProfileCreate command, IMessageBus bus) =>
        {
            var response = await bus.InvokeAsync<UserProfileResponse>(command);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("UserProfile")
        .WithName("Create UserProfile")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPut(ApiConstants.ApiSlashUserProfileUpdate, async (string applicationUserId, UserProfileUpdate command, IMessageBus bus) =>
        {
            if (applicationUserId != command.ApplicationUserId)
            {
                return Results.BadRequest();
            }

            var response = await bus.InvokeAsync<UserProfileResponse>(command);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("UserProfile")
        .WithName("Update UserProfile")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapDelete(ApiConstants.ApiSlashUserProfileDelete, async (string applicationUserId, IMessageBus bus) =>
        {
            await bus.InvokeAsync(new UserProfileDelete { ApplicationUserId = applicationUserId });
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithTags("UserProfile")
        .WithName("Delete UserProfile")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost(ApiConstants.ApiSlashGetUserProfile, async (UserProfileRequest command, IMessageBus bus) =>
        {
            var response = await bus.InvokeAsync<UserProfileResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("UserProfile")
        .WithName("Get UserProfile(s)")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost(ApiConstants.ApiSlashGetUserProfileById, async (string applicationUserId, UserProfileRequest command, IMessageBus bus) =>
        {
            command.ApplicationUserId = applicationUserId;
            var response = await bus.InvokeAsync<UserProfileResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("UserProfile")
        .WithName("Get UserProfile by UserId")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost(ApiConstants.ApiSlashGetAllUserProfiles, async (UserProfileRequest command, IMessageBus bus) =>
        {
            command.AllUserProfiles = true;
            var response = await bus.InvokeAsync<UserProfileResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("UserProfile")
        .WithName("Get all user profiles")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}
