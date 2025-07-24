using Marketplace.Data.Entities;
using Marketplace.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Data.Repositories;

public class TagRepository : GenericRepository<Tag>, ITagRepository
{
    private readonly MarketplaceDbContext _context;

    public TagRepository(MarketplaceDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Tag>> GetTagsByNameAsync(string name)
    {
        return await _context.Tags
            .Where(t => t.Name!.Contains(name))
            .ToListAsync();
    }
}