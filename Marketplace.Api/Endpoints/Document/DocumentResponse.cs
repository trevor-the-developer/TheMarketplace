namespace Marketplace.Api.Endpoints.Document;

public class DocumentResponse
{
    public Data.Entities.Document? Document { get; set; }
    public List<Data.Entities.Document>? Documents { get; set; }
}
