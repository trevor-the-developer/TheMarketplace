using Marketplace.Data.Entities;
using Marketplace.Core;

namespace Marketplace.Api.Endpoints.Product;

public class ProductResponse
{
    public Data.Entities.Product? Product { get; set; }
    public List<Data.Entities.Product>? Products { get; set; }
    public ApiError? ApiError { get; set; }
}
