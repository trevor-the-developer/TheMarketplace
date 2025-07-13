using Marketplace.Data.Entities;

namespace Marketplace.Data.Repositories;

public interface IDocumentRepository : IGenericRepository<Document>
{
    Task<IEnumerable<Document>> GetDocumentsByProductDetailIdAsync(int productDetailId);
}
