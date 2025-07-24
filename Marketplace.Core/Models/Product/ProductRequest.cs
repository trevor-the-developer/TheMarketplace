namespace Marketplace.Core.Models.Product;

public class ProductRequest
{
    public int? ProductId { get; set; }
    public int? CardId { get; set; }
    public string? Category { get; set; }
    public string? ProductType { get; set; }
    public bool? IsEnabled { get; set; }
    public bool? IsDeleted { get; set; }
    public bool AllProducts { get; set; } = false;
}