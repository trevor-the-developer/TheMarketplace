using Marketplace.Data.Entities;

namespace Marketplace.Data.Interfaces;

public interface IProductDetailRepository : IGenericRepository<ProductDetail>
{
    Task<ProductDetail?> GetProductDetailWithMediaAndDocumentsAsync(int productDetailId);
}