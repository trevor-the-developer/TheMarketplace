using Marketplace.Data.Entities;

namespace Marketplace.Data.Repositories;

public interface ITagRepository : IGenericRepository<Tag>
{
    Task<IEnumerable<Tag>> GetTagsByNameAsync(string name);
}