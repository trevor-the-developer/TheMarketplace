using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Data.Repositories;

public class ProductDetailRepository : GenericRepository<ProductDetail>, IProductDetailRepository
{
    private readonly MarketplaceDbContext _context;

    public ProductDetailRepository(MarketplaceDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<ProductDetail?> GetProductDetailWithMediaAndDocumentsAsync(int productDetailId)
    {
        return await _context.ProductDetails
            .Include(pd => pd.Documents)
            .Include(pd => pd.Media)
            .FirstOrDefaultAsync(pd => pd.Id == productDetailId);
    }
}
