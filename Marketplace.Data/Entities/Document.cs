using System.ComponentModel.DataAnnotations;

namespace Marketplace.Data.Entities;

/// <summary>
///     Represents files and text objects.
/// </summary>
public class Document : BaseEntity
{
    [Required] public string? Title { get; set; }

    public string? Description { get; set; }
    public string? Text { get; set; }
    public string? Bytes { get; set; }

    [Required] public string? DocumentType { get; set; }

    // navigation properties
    public int? ProductDetailId { get; set; }
    public virtual ProductDetail? ProductDetail { get; set; }
}