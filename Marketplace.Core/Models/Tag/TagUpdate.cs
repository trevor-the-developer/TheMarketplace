namespace Marketplace.Core.Models.Tag;

public class TagUpdate
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool? IsEnabled { get; set; }
}