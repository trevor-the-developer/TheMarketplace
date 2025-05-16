using Marketplace.Data;
using Microsoft.EntityFrameworkCore;
using Wolverine.Attributes;

namespace Marketplace.Api.Endpoints.Listing;

[WolverineHandler]
public class ListingHandler
{
    [Transactional]
    public async Task<ListingResponse> Handle(ListingRequest command, MarketplaceDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        Data.Entities.Listing? listing = null;
        if (command.ListingId > 0)
        {
            listing = await dbContext.Listings.FirstOrDefaultAsync(l => l.Id == command.ListingId);
        }
        
        if (command.AllListings)
        {
            var listings = await dbContext.Listings.ToListAsync();
            return new ListingResponse() { Listings = listings };
        }
        
        listing ??= await dbContext.Listings.FirstOrDefaultAsync();
        return new ListingResponse()
        {
            Listing = listing
        };
    }
}