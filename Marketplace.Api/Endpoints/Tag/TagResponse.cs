namespace Marketplace.Api.Endpoints.Tag;

public class TagResponse
{
    public Data.Entities.Tag? Tag { get; set; }
    public List<Data.Entities.Tag>? Tags { get; set; }
}
