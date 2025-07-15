using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Data.Repositories;

public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    private readonly MarketplaceDbContext _context;

    public ProductRepository(MarketplaceDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<Product?> GetProductWithDetailsAsync(int productId)
    {
        return await _context.Products
            .Include(p => p.ProductDetail)
            .ThenInclude(pd => pd!.Documents)
            .Include(p => p.ProductDetail)
            .ThenInclude(pd => pd!.Media)
            .FirstOrDefaultAsync(p => p.Id == productId);
    }

    public async Task<IEnumerable<Product>> GetProductsByCardIdAsync(int cardId)
    {
        return await _context.Products
            .Where(p => p.CardId == cardId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category)
    {
        return await _context.Products
            .Where(p => p.Category == category)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByTypeAsync(string productType)
    {
        return await _context.Products
            .Where(p => p.ProductType == productType)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetEnabledProductsAsync()
    {
        return await _context.Products
            .Where(p => p.IsEnabled == true)
            .ToListAsync();
    }
}