using Marketplace.Data.Entities;

namespace Marketplace.Data.Repositories;

public interface IUserProfileRepository : IGenericRepository<UserProfile>
{
    Task<UserProfile?> GetUserProfileByUserIdAsync(string userId);
}