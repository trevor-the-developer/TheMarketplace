using Microsoft.AspNetCore.Http;

namespace Marketplace.Core.Models.Media;

public record MediaCreateRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? MediaType { get; set; }
    public int? ProductDetailId { get; set; }
    public IFormFile? File { get; set; }
}