using Marketplace.Data.Entities;

namespace Marketplace.Data.Repositories;

public class ListingRepository : GenericRepository<Listing>, IListingRepository
{
    public ListingRepository(MarketplaceDbContext context) : base(context)
    {
    }

    // Add custom methods for Listing here if needed
}
