using Marketplace.Data.Entities;

namespace Marketplace.Data.Interfaces;

public interface IMediaRepository : IGenericRepository<Media>
{
    Task<IEnumerable<Media>> GetMediaByProductDetailIdAsync(int productDetailId);
}