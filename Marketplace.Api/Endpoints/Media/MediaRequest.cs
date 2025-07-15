namespace Marketplace.Api.Endpoints.Media;

public class MediaRequest
{
    public int? MediaId { get; set; }
    public int? ProductDetailId { get; set; }
    public string? MediaType { get; set; }
    public bool AllMedia { get; set; } = false;
}