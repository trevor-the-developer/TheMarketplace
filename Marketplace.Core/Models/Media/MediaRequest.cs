namespace Marketplace.Core.Models.Media;

public class MediaRequest
{
    public int MediaId { get; init; }
    public int? ProductDetailId { get; set; }
    public string? MediaType { get; set; }
    public bool AllMedia { get; init; } = false;
}