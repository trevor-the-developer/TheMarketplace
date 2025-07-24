using Marketplace.Data.Entities;

namespace Marketplace.Data.Interfaces;

public interface IUserProfileRepository : IGenericRepository<UserProfile>
{
    Task<UserProfile?> GetUserProfileByUserIdAsync(string userId);
}