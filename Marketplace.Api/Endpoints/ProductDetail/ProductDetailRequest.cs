namespace Marketplace.Api.Endpoints.ProductDetail;

public class ProductDetailRequest
{
    public int? ProductDetailId { get; set; }
    public int? ProductId { get; set; }
    public bool AllProductDetails { get; set; } = false;
}