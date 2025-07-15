using System.ComponentModel.DataAnnotations;

namespace Marketplace.Data.Entities;

/// <summary>
///     Represents a listing (collection of cards)
/// </summary>
public class Listing : BaseEntity
{
    [Required] public string? Title { get; set; }

    public string? Description { get; set; }
    public ICollection<Card>? Cards { get; set; }
}