using Marketplace.Core.Constants;
using Wolverine;

namespace Marketplace.Api.Endpoints.Listing;

public static class ListingEndpoints
{
    public static void MapListingEndpoints(this IEndpointRouteBuilder routes)
    {
        // POST /api/listings - Create new listing
        routes.MapPost(ApiConstants.ApiListings, async (ListingCreate command, IMessageBus bus) =>
            {
                var response = await bus.InvokeAsync<ListingResponse>(command);
                return Results.Created($"/api/listings/{response.Listing?.Id}", response);
            })
            .RequireAuthorization()
            .WithTags("Listings")
            .WithName("Create Listing")
            .Produces(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        // PUT /api/listings/{id} - Update existing listing
        routes.MapPut(ApiConstants.ApiListingsById, async (int id, ListingUpdate command, IMessageBus bus) =>
            {
                if (id != command.Id) return Results.BadRequest();

                var response = await bus.InvokeAsync<ListingResponse>(command);
                return response.Listing == null ? Results.NotFound() : Results.Ok(response);
            })
            .RequireAuthorization()
            .WithTags("Listings")
            .WithName("Update Listing")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // DELETE /api/listings/{id} - Delete listing
        routes.MapDelete(ApiConstants.ApiListingsById, async (int id, IMessageBus bus) =>
            {
                await bus.InvokeAsync(new ListingDelete { Id = id });
                return Results.NoContent();
            })
            .RequireAuthorization()
            .WithTags("Listings")
            .WithName("Delete Listing")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/listings - Get all listings
        routes.MapGet(ApiConstants.ApiListings, async (IMessageBus bus) =>
            {
                var command = new ListingRequest { AllListings = true };
                var response = await bus.InvokeAsync<ListingResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(response)
                };
            })
            .RequireAuthorization()
            .WithTags("Listings")
            .WithName("Get All Listings")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status500InternalServerError);

        // GET /api/listings/{id} - Get listing by ID
        routes.MapGet(ApiConstants.ApiListingsById, async (int id, IMessageBus bus) =>
            {
                var command = new ListingRequest { ListingId = id };
                var response = await bus.InvokeAsync<ListingResponse>(command);

                return response switch
                {
                    null => Results.NotFound(),
                    _ => Results.Ok(response)
                };
            })
            .RequireAuthorization()
            .WithTags("Listings")
            .WithName("Get Listing by Id")
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError);
    }
}