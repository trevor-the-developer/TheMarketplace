namespace Marketplace.Api.Endpoints.Product;

public class ProductCreate
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ProductType { get; set; }
    public string? Category { get; set; }
    public bool? IsEnabled { get; set; } = true;
    public bool? IsDeleted { get; set; } = false;
    public int? CardId { get; set; }
    public int? ProductDetailId { get; set; }
}
