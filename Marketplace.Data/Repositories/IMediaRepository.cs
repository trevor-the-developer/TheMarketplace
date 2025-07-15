using Marketplace.Data.Entities;

namespace Marketplace.Data.Repositories;

public interface IMediaRepository : IGenericRepository<Media>
{
    Task<IEnumerable<Media>> GetMediaByProductDetailIdAsync(int productDetailId);
}