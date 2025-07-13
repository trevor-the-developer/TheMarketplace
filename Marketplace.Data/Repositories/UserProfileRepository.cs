using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Data.Repositories;

public class UserProfileRepository : GenericRepository<UserProfile>, IUserProfileRepository
{
    private readonly MarketplaceDbContext _context;

    public UserProfileRepository(MarketplaceDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<UserProfile?> GetUserProfileByUserIdAsync(string userId)
    {
        return await _context.Profiles
            .FirstOrDefaultAsync(up => up.ApplicationUserId == userId);
    }
}
