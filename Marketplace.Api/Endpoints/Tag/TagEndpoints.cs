using Marketplace.Core.Constants;
using Wolverine;

namespace Marketplace.Api.Endpoints.Tag;

public static class TagEndpoints
{
    public static void MapTagEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost(ApiConstants.ApiSlashTagCreate, async (TagCreate command, IMessageBus bus) =>
        {
            var response = await bus.InvokeAsync<TagResponse>(command);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("Tag")
        .WithName("Create Tag")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPut(ApiConstants.ApiSlashTagUpdate, async (int id, TagUpdate command, IMessageBus bus) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest();
            }

            var response = await bus.InvokeAsync<TagResponse>(command);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("Tag")
        .WithName("Update Tag")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapDelete(ApiConstants.ApiSlashTagDelete, async (int id, IMessageBus bus) =>
        {
            await bus.InvokeAsync(new TagDelete { Id = id });
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithTags("Tag")
        .WithName("Delete Tag")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost(ApiConstants.ApiSlashGetTag, async (TagRequest command, IMessageBus bus) =>
        {
            var response = await bus.InvokeAsync<TagResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("Tag")
        .WithName("Get Tag(s)")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost(ApiConstants.ApiSlashGetTagById, async (int tagId, TagRequest command, IMessageBus bus) =>
        {
            command.TagId = tagId;
            var response = await bus.InvokeAsync<TagResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("Tag")
        .WithName("Get Tag by Id")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPost(ApiConstants.ApiSlashGetAllTags, async (TagRequest command, IMessageBus bus) =>
        {
            command.AllTags = true;
            var response = await bus.InvokeAsync<TagResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("Tag")
        .WithName("Get all tags")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}
