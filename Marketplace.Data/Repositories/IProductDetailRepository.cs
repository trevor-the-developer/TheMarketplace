using Marketplace.Data.Entities;

namespace Marketplace.Data.Repositories;

public interface IProductDetailRepository : IGenericRepository<ProductDetail>
{
    Task<ProductDetail?> GetProductDetailWithMediaAndDocumentsAsync(int productDetailId);
}
