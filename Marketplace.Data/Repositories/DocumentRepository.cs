using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Data.Repositories;

public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
{
    private readonly MarketplaceDbContext _context;

    public DocumentRepository(MarketplaceDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Document>> GetDocumentsByProductDetailIdAsync(int productDetailId)
    {
        return await _context.Documents
            .Where(d => d.ProductDetailId == productDetailId)
            .ToListAsync();
    }
}
