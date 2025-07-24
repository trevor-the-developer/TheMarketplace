using Marketplace.Data.Entities;

namespace Marketplace.Data.Interfaces;

public interface IDocumentRepository : IGenericRepository<Document>
{
    Task<IEnumerable<Document>> GetDocumentsByProductDetailIdAsync(int productDetailId);
}