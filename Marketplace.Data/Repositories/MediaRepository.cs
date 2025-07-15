using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Data.Repositories;

public class MediaRepository : GenericRepository<Media>, IMediaRepository
{
    private readonly MarketplaceDbContext _context;

    public MediaRepository(MarketplaceDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Media>> GetMediaByProductDetailIdAsync(int productDetailId)
    {
        return await _context.Files
            .Where(m => m.ProductDetailId == productDetailId)
            .ToListAsync();
    }
}