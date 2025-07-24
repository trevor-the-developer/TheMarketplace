using System.Collections.Generic;

namespace Marketplace.Core.Models.Document;

public class DocumentResponse
{
    public Data.Entities.Document? Document { get; set; }
    public List<Data.Entities.Document>? Documents { get; set; }
    public ApiError? ApiError { get; set; }
}