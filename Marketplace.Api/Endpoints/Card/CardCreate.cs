namespace Marketplace.Api.Endpoints.Card;

public class CardCreate
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int ListingId { get; set; }
}