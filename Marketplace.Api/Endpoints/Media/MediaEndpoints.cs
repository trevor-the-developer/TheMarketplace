using Marketplace.Core.Constants;
using Wolverine;

namespace Marketplace.Api.Endpoints.Media;

public static class MediaEndpoints
{
    public static void MapMediaEndpoints(this IEndpointRouteBuilder routes)
    {
        // POST /api/media - Create new media
        routes.MapPost(ApiConstants.ApiMedia, async (MediaCreate command, IMessageBus bus) =>
        {
            var response = await bus.InvokeAsync<MediaResponse>(command);
            return Results.Created($"/api/media/{response.Media?.Id}", response);
        })
        .RequireAuthorization()
        .WithTags("Media")
        .WithName("Create Media")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        // PUT /api/media/{id} - Update existing media
        routes.MapPut(ApiConstants.ApiMediaById, async (int id, MediaUpdate command, IMessageBus bus) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest();
            }

            var response = await bus.InvokeAsync<MediaResponse>(command);
            return response.Media == null ? Results.NotFound() : Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("Media")
        .WithName("Update Media")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        // DELETE /api/media/{id} - Delete media
        routes.MapDelete(ApiConstants.ApiMediaById, async (int id, IMessageBus bus) =>
        {
            await bus.InvokeAsync(new MediaDelete { Id = id });
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithTags("Media")
        .WithName("Delete Media")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/media - Get all media
        routes.MapGet(ApiConstants.ApiMedia, async (IMessageBus bus) =>
        {
            var command = new MediaRequest { AllMedia = true };
            var response = await bus.InvokeAsync<MediaResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("Media")
        .WithName("Get All Media")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/media/{id} - Get media by ID
        routes.MapGet(ApiConstants.ApiMediaById, async (int id, IMessageBus bus) =>
        {
            var command = new MediaRequest { MediaId = id };
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
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}
