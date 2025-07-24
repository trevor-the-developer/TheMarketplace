namespace Marketplace.Core.Models.ProductDetail;

public class ProductDetailUpdate
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ProductId { get; set; }
}