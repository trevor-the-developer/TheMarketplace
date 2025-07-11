using Marketplace.Api.Endpoints.Authentication.Login;
using Newtonsoft.Json;
using Wolverine;

namespace Marketplace.Api.Endpoints.Listing;


public static class ListingEndpoints
{
    public static void MapListingEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost("/api/listing/create", async (ListingCreate command, IMessageBus bus) =>
        {
            var response = await bus.InvokeAsync<ListingResponse>(command);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("Listing")
        .WithName("Create Listing")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapPut("/api/listing/update/{id}", async (int id, ListingUpdate command, IMessageBus bus) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest();
            }

            var response = await bus.InvokeAsync<ListingResponse>(command);
            return Results.Ok(response);
        })
        .RequireAuthorization()
        .WithTags("Listing")
        .WithName("Update Listing")
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status500InternalServerError);

        routes.MapDelete("/api/listing/delete/{id}", async (int id, IMessageBus bus) =>
        {
            await bus.InvokeAsync(new ListingDelete { Id = id });
            return Results.NoContent();
        })
        .RequireAuthorization()
        .WithTags("Listing")
        .WithName("Delete Listing")
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError);
        routes.MapPost("/api/get/listing/", async (ListingRequest command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<ListingResponse>(command);

                return response switch
                {
                    null => Results.Unauthorized(),
                    _ => Results.Ok(response)
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
                    _ => Results.Ok(response)
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
                    _ => Results.Ok(response)
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