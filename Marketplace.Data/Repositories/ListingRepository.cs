using Marketplace.Data.Entities;
using Marketplace.Data.Interfaces;

namespace Marketplace.Data.Repositories;

public class ListingRepository : GenericRepository<Listing>, IListingRepository
{
    public ListingRepository(MarketplaceDbContext context) : base(context)
    {
    }

    // Add custom methods for Listing here if needed
}