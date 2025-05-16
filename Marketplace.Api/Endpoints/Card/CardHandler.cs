using Marketplace.Data;
using Microsoft.EntityFrameworkCore;
using Wolverine.Attributes;

namespace Marketplace.Api.Endpoints.Card;

[WolverineHandler]
public class CardHandler
{
    [Transactional]
    public async Task<CardResponse> Handle(CardRequest command, MarketplaceDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        Data.Entities.Card? card = null;
        if (command.CardId > 0)
        {
            card = await dbContext.Cards.FindAsync(command.CardId);
        }

        if (command.AllCards)
        {
            var cards = await dbContext.Cards.ToListAsync();
            return new CardResponse() { Cards = cards };
        }

        card ??= await dbContext.Cards.FirstOrDefaultAsync();
        return new CardResponse()
        {
            Card = card
        };
    }
}