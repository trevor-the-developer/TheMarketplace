namespace Marketplace.Core.Models.ProductDetail;

public class ProductDetailRequest
{
    public int? ProductDetailId { get; set; }
    public int? ProductId { get; set; }
    public bool AllProductDetails { get; set; } = false;
}