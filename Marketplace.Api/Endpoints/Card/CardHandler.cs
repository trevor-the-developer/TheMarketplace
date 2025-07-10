using Marketplace.Data;
using Microsoft.EntityFrameworkCore;
using Wolverine.Attributes;
using Marketplace.Core.Services;

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

    [Transactional]
    public async Task<CardResponse> Handle(CardCreate command, MarketplaceDbContext dbContext, ICurrentUserService currentUserService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));

        var currentUser = currentUserService.GetCurrentUserName();
        var card = new Data.Entities.Card
        {
            Title = command.Title,
            Description = command.Description,
            ListingId = command.ListingId,
            CreatedBy = currentUser,
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = currentUser,
            ModifiedDate = DateTime.UtcNow
        };

        dbContext.Cards.Add(card);
        await dbContext.SaveChangesAsync();

        return new CardResponse { Card = card };
    }

    [Transactional]
    public async Task<CardResponse> Handle(CardUpdate command, MarketplaceDbContext dbContext, ICurrentUserService currentUserService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));

        var card = await dbContext.Cards.FindAsync(command.Id);
        if (card == null)
        {
            return new CardResponse { Card = null };
        }

        card.Title = command.Title;
        card.Description = command.Description;
        card.ModifiedBy = currentUserService.GetCurrentUserName();
        card.ModifiedDate = DateTime.UtcNow;

        await dbContext.SaveChangesAsync();

        return new CardResponse { Card = card };
    }

    [Transactional]
    public async Task Handle(CardDelete command, MarketplaceDbContext dbContext)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(dbContext, nameof(dbContext));

        var card = await dbContext.Cards.FindAsync(command.Id);
        if (card != null)
        {
            dbContext.Cards.Remove(card);
            await dbContext.SaveChangesAsync();
        }
    }
}
