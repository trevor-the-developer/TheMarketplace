using Marketplace.Core.Constants;
using Wolverine;

namespace Marketplace.Api.Endpoints.Media;

public static class MediaEndpoints
{
    public static void MapMediaEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost(ApiConstants.ApiSlashMediaCreate, async (MediaCreate command, IMessageBus bus) =>
        {
            var response = await bus.InvokeAsync<MediaResponse>(command);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("Media")
        .WithName("Create Media")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPut(ApiConstants.ApiSlashMediaUpdate, async (int id, MediaUpdate command, IMessageBus bus) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest();
            }

            var response = await bus.InvokeAsync<MediaResponse>(command);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("Media")
        .WithName("Update Media")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapDelete(ApiConstants.ApiSlashMediaDelete, async (int id, IMessageBus bus) =>
        {
            await bus.InvokeAsync(new MediaDelete { Id = id });
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithTags("Media")
        .WithName("Delete Media")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost(ApiConstants.ApiSlashGetMedia, async (MediaRequest command, IMessageBus bus) =>
        {
            var response = await bus.InvokeAsync<MediaResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("Media")
        .WithName("Get Media(s)")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost(ApiConstants.ApiSlashGetMediaById, async (int mediaId, MediaRequest command, IMessageBus bus) =>
        {
            command.MediaId = mediaId;
            var response = await bus.InvokeAsync<MediaResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("Media")
        .WithName("Get Media by Id")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost(ApiConstants.ApiSlashGetAllMedia, async (MediaRequest command, IMessageBus bus) =>
        {
            command.AllMedia = true;
            var response = await bus.InvokeAsync<MediaResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("Media")
        .WithName("Get all media")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}
