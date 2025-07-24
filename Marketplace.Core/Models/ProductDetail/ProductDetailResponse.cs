using System.Collections.Generic;

namespace Marketplace.Core.Models.ProductDetail;

public class ProductDetailResponse
{
    public Data.Entities.ProductDetail? ProductDetail { get; set; }
    public List<Data.Entities.ProductDetail>? ProductDetails { get; set; }
    public ApiError? ApiError { get; set; }
}