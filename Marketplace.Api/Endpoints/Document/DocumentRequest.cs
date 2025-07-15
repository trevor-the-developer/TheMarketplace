namespace Marketplace.Api.Endpoints.Document;

public class DocumentRequest
{
    public int? DocumentId { get; set; }
    public int? ProductDetailId { get; set; }
    public string? DocumentType { get; set; }
    public bool AllDocuments { get; set; } = false;
}