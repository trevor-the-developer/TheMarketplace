using System.ComponentModel.DataAnnotations;

namespace Marketplace.Data.Entities;

/// <summary>
///     Represents a visual card (collection of products)
/// </summary>
public class Card : BaseEntity
{
    [Required] public string? Title { get; set; }

    public string? Description { get; set; }
    public bool? IsEnabled { get; set; }

    public string? Colour { get; set; }

    // navigation properties
    public int? ListingId { get; set; }
    public virtual Listing? Listing { get; set; }
    public ICollection<Product>? Products { get; set; }
}