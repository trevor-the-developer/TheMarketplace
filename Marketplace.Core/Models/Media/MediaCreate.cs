namespace Marketplace.Core.Models.Media;

public class MediaCreate
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? FilePath { get; set; }
    public string? DirectoryPath { get; set; }
    public string? MediaType { get; set; }
    public int? ProductDetailId { get; set; }
}