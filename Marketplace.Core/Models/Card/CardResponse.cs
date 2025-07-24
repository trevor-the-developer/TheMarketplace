using System.Collections.Generic;

namespace Marketplace.Core.Models.Card;

public class CardResponse
{
    public Data.Entities.Card? Card { get; set; }
    public List<Data.Entities.Card>? Cards { get; set; }
    public ApiError? ApiError { get; set; }
}