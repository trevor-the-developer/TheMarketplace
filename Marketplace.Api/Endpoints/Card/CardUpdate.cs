namespace Marketplace.Api.Endpoints.Card;

public class CardUpdate
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}