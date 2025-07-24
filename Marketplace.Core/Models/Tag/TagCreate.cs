namespace Marketplace.Core.Models.Tag;

public class TagCreate
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool? IsEnabled { get; set; } = true;
}