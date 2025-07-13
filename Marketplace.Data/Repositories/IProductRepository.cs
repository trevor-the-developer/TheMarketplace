using Marketplace.Data.Entities;

namespace Marketplace.Data.Repositories;

public interface IProductRepository : IGenericRepository<Product>
{
    Task<Product?> GetProductWithDetailsAsync(int productId);
    Task<IEnumerable<Product>> GetProductsByCardIdAsync(int cardId);
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(string category);
    Task<IEnumerable<Product>> GetProductsByTypeAsync(string productType);
    Task<IEnumerable<Product>> GetEnabledProductsAsync();
}
