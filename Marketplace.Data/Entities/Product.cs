using System.ComponentModel.DataAnnotations;

namespace Marketplace.Data.Entities;

/// <summary>
///     Represents a product and/or service.
/// </summary>
public class Product : BaseEntity
{
    [Required] public string? Title { get; set; }

    public string? Description { get; set; }
    public string? ProductType { get; set; }
    public string? Category { get; set; }
    public bool? IsEnabled { get; set; }

    public bool? IsDeleted { get; set; }

    // navigation properties
    public int? CardId { get; set; }
    public virtual Card? Card { get; set; }
    public int? ProductDetailId { get; set; }
    public virtual ProductDetail? ProductDetail { get; set; }
}