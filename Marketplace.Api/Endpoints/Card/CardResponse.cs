namespace Marketplace.Api.Endpoints.Card;

public class CardResponse
{
    public Data.Entities.Card? Card { get; set; }
    public List<Data.Entities.Card>? Cards { get; set; }
}