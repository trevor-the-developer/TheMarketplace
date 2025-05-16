using Newtonsoft.Json;
using Wolverine;

namespace Marketplace.Api.Endpoints.Card;

public static class CardEndpoints
{
    public static void MapCardEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/get/card/", async (CardRequest command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<CardResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(JsonConvert.SerializeObject(response))
                };
            })
            .RequireAuthorization()
            .WithTags("Card")
            .WithName("Get Card(s)")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);
        
        routes.MapPost("/api/get/card/{cardId}", async (int cardId, 
                CardRequest command, IMessageBus bus) =>
            {
                command.CardId = cardId;
                var response = await bus.InvokeAsync<CardResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(JsonConvert.SerializeObject(response))
                };
            })
            .RequireAuthorization()
            .WithTags("Card")
            .WithName("Get Card by Id")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);
        
        routes.MapPost("/api/get/card/all/", async (int cardId, 
                CardRequest command, IMessageBus bus) =>
            {
                command.AllCards = true;
                var response = await bus.InvokeAsync<CardResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(JsonConvert.SerializeObject(response))
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