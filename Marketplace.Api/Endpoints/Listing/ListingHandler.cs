using Marketplace.Data;
using Microsoft.EntityFrameworkCore;
using Wolverine.Attributes;
using Marketplace.Core.Services;

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

    [Transactional]
    public async Task<ListingResponse> Handle(ListingCreate command, MarketplaceDbContext dbContext, ICurrentUserService currentUserService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));

        var currentUser = currentUserService.GetCurrentUserName();
        var listing = new Data.Entities.Listing
        {
            Title = command.Title,
            Description = command.Description,
            CreatedBy = currentUser,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = currentUser,
            ModifiedDate = DateTime.UtcNow
        };

        dbContext.Listings.Add(listing);
        await dbContext.SaveChangesAsync();

        return new ListingResponse { Listing = listing };
    }

    [Transactional]
    public async Task<ListingResponse> Handle(ListingUpdate command, MarketplaceDbContext dbContext, ICurrentUserService currentUserService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));

        var listing = await dbContext.Listings.FindAsync(command.Id);
        if (listing == null)
        {
            return new ListingResponse { Listing = null };
        }

        listing.Title = command.Title;
        listing.Description = command.Description;
        listing.ModifiedBy = currentUserService.GetCurrentUserName();
        listing.ModifiedDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return new ListingResponse { Listing = listing };
    }

    [Transactional]
    public async Task Handle(ListingDelete command, MarketplaceDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        var listing = await dbContext.Listings.FindAsync(command.Id);
        if (listing != null)
        {
            dbContext.Listings.Remove(listing);
            await dbContext.SaveChangesAsync();
        }
    }
}
