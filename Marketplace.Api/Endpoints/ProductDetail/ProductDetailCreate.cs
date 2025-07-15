namespace Marketplace.Api.Endpoints.ProductDetail;

public class ProductDetailCreate
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ProductId { get; set; }
}