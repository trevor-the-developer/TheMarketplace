using Marketplace.Core.Constants;
using Wolverine;

namespace Marketplace.Api.Endpoints.Card;

public static class CardEndpoints
{
    public static void MapCardEndpoints(this IEndpointRouteBuilder routes)
    {
        // POST /api/cards - Create new card
        routes.MapPost(ApiConstants.ApiCards, async (CardCreate command, IMessageBus bus) =>
        {
            var response = await bus.InvokeAsync<CardResponse>(command);
            return Results.Created($"/api/cards/{response.Card?.Id}", response);
        })
        .RequireAuthorization()
        .WithTags("Cards")
        .WithName("Create Card")
        .Produces(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        // PUT /api/cards/{id} - Update existing card
        routes.MapPut(ApiConstants.ApiCardsById, async (int id, CardUpdate command, IMessageBus bus) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest();
            }

            var response = await bus.InvokeAsync<CardResponse>(command);
            return response.Card == null ? Results.NotFound() : Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("Cards")
        .WithName("Update Card")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        // DELETE /api/cards/{id} - Delete card
        routes.MapDelete(ApiConstants.ApiCardsById, async (int id, IMessageBus bus) =>
        {
            await bus.InvokeAsync(new CardDelete { Id = id });
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithTags("Cards")
        .WithName("Delete Card")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
        
        // GET /api/cards - Get all cards
        routes.MapGet(ApiConstants.ApiCards, async (IMessageBus bus) =>
        {
            var command = new CardRequest { AllCards = true };
            var response = await bus.InvokeAsync<CardResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("Cards")
        .WithName("Get All Cards")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);
        
        // GET /api/cards/{id} - Get card by ID
        routes.MapGet(ApiConstants.ApiCardsById, async (int id, IMessageBus bus) =>
        {
            var command = new CardRequest { CardId = id };
            var response = await bus.InvokeAsync<CardResponse>(command);

            return response switch
            {
                null => Results.NotFound(),
                _ => Results.Ok(response)
            };
        })
        .RequireAuthorization()
        .WithTags("Cards")
        .WithName("Get Card by Id")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);
    }
}