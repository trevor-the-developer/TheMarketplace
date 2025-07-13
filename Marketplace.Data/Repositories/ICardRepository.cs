using Marketplace.Data.Entities;

namespace Marketplace.Data.Repositories;

public interface ICardRepository : IGenericRepository<Card>
{
    Task<Card?> GetCardWithProductsAsync(int cardId);
    Task<IEnumerable<Card>> GetCardsByListingIdAsync(int listingId);
}
