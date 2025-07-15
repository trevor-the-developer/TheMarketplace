using System.ComponentModel.DataAnnotations;

namespace Marketplace.Data.Entities;

/// <summary>
///     Represents a tag for a given entity (can have one or more tag's).
/// </summary>
public class Tag : BaseEntity
{
    [Required] public string? Name { get; set; }

    public string? Description { get; set; }
    public bool? IsEnabled { get; set; }
    public IEnumerable<Tag>? Tags { get; set; }
}