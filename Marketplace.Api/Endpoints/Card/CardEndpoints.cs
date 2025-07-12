using Marketplace.Core.Constants;
using Wolverine;

namespace Marketplace.Api.Endpoints.Card;

public static class CardEndpoints
{
    public static void MapCardEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost(ApiConstants.ApiSlashCardCreate, async (CardCreate command, IMessageBus bus) =>
        {
            var response = await bus.InvokeAsync<CardResponse>(command);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("Card")
        .WithName("Create Card")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPut(ApiConstants.ApiSlashCardUpdate, async (int id, CardUpdate command, IMessageBus bus) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest();
            }

            var response = await bus.InvokeAsync<CardResponse>(command);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("Card")
        .WithName("Update Card")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapDelete(ApiConstants.ApiSlashCardDelete, async (int id, IMessageBus bus) =>
        {
            await bus.InvokeAsync(new CardDelete { Id = id });
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithTags("Card")
        .WithName("Delete Card")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);
        
        routes.MapPost(ApiConstants.ApiSlashGetCards, async (CardRequest command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<CardResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(response)
                };
            })
            .RequireAuthorization()
            .WithTags("Card")
            .WithName("Get Card(s)")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);
        
        routes.MapPost(ApiConstants.ApiSlashGetCardById, async (int cardId, 
                CardRequest command, IMessageBus bus) =>
            {
                command.CardId = cardId;
                var response = await bus.InvokeAsync<CardResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(response)
                };
            })
            .RequireAuthorization()
            .WithTags("Card")
            .WithName("Get Card by Id")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);
        
        routes.MapPost(ApiConstants.ApiSlashGetAllCards, async (
                CardRequest command, IMessageBus bus) =>
            {
                command.AllCards = true;
                var response = await bus.InvokeAsync<CardResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(response)
                };
            })
            .RequireAuthorization()
            .WithTags("Card")
            .WithName("Get all cards")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}