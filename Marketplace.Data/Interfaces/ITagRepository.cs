using Marketplace.Data.Entities;

namespace Marketplace.Data.Interfaces;

public interface ITagRepository : IGenericRepository<Tag>
{
    Task<IEnumerable<Tag>> GetTagsByNameAsync(string name);
}