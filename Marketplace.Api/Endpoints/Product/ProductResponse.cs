using Marketplace.Data.Entities;

namespace Marketplace.Api.Endpoints.Product;

public class ProductResponse
{
    public Data.Entities.Product? Product { get; set; }
    public List<Data.Entities.Product>? Products { get; set; }
}
