using Marketplace.Api.Endpoints.Authentication.Login;
using Newtonsoft.Json;
using Wolverine;

namespace Marketplace.Api.Endpoints.Listing;


public static class ListingEndpoints
{
    public static void MapListingEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/get/listing/", async (ListingRequest command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<ListingResponse>(command);

                return response switch
                {
                    null => Results.Unauthorized(),
                    _ => Results.Ok(JsonConvert.SerializeObject(response))
                };
            })
            .RequireAuthorization()
            .WithTags("Listing")
            .WithName("Get Listing(s)")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);
        
        routes.MapPost("/api/get/listing/{listingId}", async (int listingId, ListingRequest command, IMessageBus bus) =>
            {
                command.ListingId = listingId;
                var response = await bus.InvokeAsync<ListingResponse>(command);

                return response switch
                {
                    null => Results.Unauthorized(),
                    _ => Results.Ok(JsonConvert.SerializeObject(response))
                };
            })
            .RequireAuthorization()
            .WithTags("Listing")
            .WithName("Get Listing by Id")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);
        
        routes.MapPost("/api/get/all/listing/", async (ListingRequest command, IMessageBus bus) =>
            {
                command.AllListings = true;
                var response = await bus.InvokeAsync<ListingResponse>(command);

                return response switch
                {
                    null => Results.Unauthorized(),
                    _ => Results.Ok(JsonConvert.SerializeObject(response))
                };
            })
            .RequireAuthorization()
            .WithTags("Listing")
            .WithName("Get all listings")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}