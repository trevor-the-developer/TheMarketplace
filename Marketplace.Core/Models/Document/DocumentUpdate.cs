namespace Marketplace.Core.Models.Document;

public class DocumentUpdate
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Text { get; set; }
    public string? Bytes { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public int? ProductDetailId { get; set; }
}