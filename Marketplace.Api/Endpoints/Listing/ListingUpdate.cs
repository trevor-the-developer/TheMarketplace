namespace Marketplace.Api.Endpoints.Listing;

public class ListingUpdate
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
