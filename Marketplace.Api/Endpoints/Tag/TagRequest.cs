namespace Marketplace.Api.Endpoints.Tag;

public class TagRequest
{
    public int? TagId { get; set; }
    public string? Name { get; set; }
    public bool? IsEnabled { get; set; }
    public bool AllTags { get; set; } = false;
}
