namespace Marketplace.Api.Endpoints.Product;

public class ProductUpdate
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ProductType { get; set; }
    public string? Category { get; set; }
    public bool? IsEnabled { get; set; }
    public bool? IsDeleted { get; set; }
    public int? CardId { get; set; }
    public int? ProductDetailId { get; set; }
}