using System.ComponentModel.DataAnnotations;

namespace Marketplace.Data.Entities;

/// <summary>
///     Represents any type of computer file e.g. an MP4 video
/// </summary>
public class Media : BaseEntity
{
    [Required] public string? Title { get; set; }

    public string? Description { get; set; }
    public string? FilePath { get; set; }
    public string? DirectoryPath { get; set; }

    public string? MediaType { get; set; }

    // navigation properties
    public int? ProductDetailId { get; set; }
    public virtual ProductDetail? ProductDetail { get; set; }
}