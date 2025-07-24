using Marketplace.Core.Constants;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Tag;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Wolverine;

namespace Marketplace.Api.Endpoints.Tag;

public static class TagEndpoints
{
    public static void MapTagEndpoints(this IEndpointRouteBuilder routes)
    {
        // POST /api/tags - Create new tag
        routes.MapPost(ApiConstants.ApiTags, async (TagCreate command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<TagResponse>(command);
                return Results.Created($"/api/tags/{response.Tag?.Id}", response);
            })
            .RequireAuthorization()
            .WithTags("Tags")
            .WithName("Create Tag")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        // PUT /api/tags/{id} - Update existing tag
        routes.MapPut(ApiConstants.ApiTagsById, async (int id, TagUpdate command, IMessageBus bus) =>
            {
                if (id != command.Id) return Results.BadRequest();

                var response = await bus.InvokeAsync<TagResponse>(command);
                return response.Tag == null ? Results.NotFound() : Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags("Tags")
            .WithName("Update Tag")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE /api/tags/{id} - Delete tag
        routes.MapDelete(ApiConstants.ApiTagsById, async (int id, IMessageBus bus) =>
            {
                await bus.InvokeAsync(new TagDelete { Id = id });
                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithTags("Tags")
            .WithName("Delete Tag")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/tags - Get all tags
        routes.MapGet(ApiConstants.ApiTags, async (IMessageBus bus) =>
            {
                var command = new TagRequest { AllTags = true };
                var response = await bus.InvokeAsync<TagResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(response)
                };
            })
            .RequireAuthorization()
            .WithTags("Tags")
            .WithName("Get All Tags")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/tags/{id} - Get tag by ID
        routes.MapGet(ApiConstants.ApiTagsById, async (int id, IMessageBus bus) =>
            {
                var command = new TagRequest { TagId = id };
                var response = await bus.InvokeAsync<TagResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(response)
                };
            })
            .RequireAuthorization()
            .WithTags("Tags")
            .WithName("Get Tag by Id")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}