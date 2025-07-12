using Marketplace.Core.Constants;
using Wolverine;

namespace Marketplace.Api.Endpoints.Listing;


public static class ListingEndpoints
{
    public static void MapListingEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost(ApiConstants.ApiSlashListingCreate, async (ListingCreate command, IMessageBus bus) =>
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

        routes.MapPut(ApiConstants.ApiSlashListingUpdate, async (int id, ListingUpdate command, IMessageBus bus) =>
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

        routes.MapDelete(ApiConstants.ApiSlashListingDelete, async (int id, IMessageBus bus) =>
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
        routes.MapPost(ApiConstants.ApiSlashGetListing, async (ListingRequest command, IMessageBus bus) =>
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
        
        routes.MapPost(ApiConstants.ApiSlashGetListingById, async (int listingId, ListingRequest command, IMessageBus bus) =>
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
        
        routes.MapPost(ApiConstants.ApiSlashGetAllListings, async (ListingRequest command, IMessageBus bus) =>
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