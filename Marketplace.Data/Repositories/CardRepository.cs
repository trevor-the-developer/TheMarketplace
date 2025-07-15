using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Data.Repositories;

public class CardRepository : GenericRepository<Card>, ICardRepository
{
    private readonly MarketplaceDbContext _context;

    public CardRepository(MarketplaceDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Card?> GetCardWithProductsAsync(int cardId)
    {
        return await _context.Cards
            .Include(c => c.Products!)
            .ThenInclude(p => p.ProductDetail)
            .ThenInclude(pd => pd!.Documents)
            .Include(c => c.Products!)
            .ThenInclude(p => p.ProductDetail)
            .ThenInclude(pd => pd!.Media)
            .FirstOrDefaultAsync(c => c.Id == cardId);
    }

    public async Task<IEnumerable<Card>> GetCardsByListingIdAsync(int listingId)
    {
        return await _context.Cards
            .Where(c => c.ListingId == listingId)
            .ToListAsync();
    }
}