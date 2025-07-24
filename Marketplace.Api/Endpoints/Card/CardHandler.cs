using Marketplace.Core;
using Marketplace.Core.Services;
using Marketplace.Core.Validation;
using Marketplace.Data.Interfaces;
using Newtonsoft.Json;
using Wolverine.Attributes;

namespace Marketplace.Api.Endpoints.Card;

[WolverineHandler]
public class CardHandler
{
    [Transactional]
    public async Task<CardResponse> Handle(CardRequest command, ICardRepository cardRepository)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(cardRepository, nameof(cardRepository));

        Data.Entities.Card? card = null;
        if (command.CardId > 0) card = await cardRepository.GetByIdAsync(command.CardId);

        if (command.AllCards)
        {
            var cards = await cardRepository.GetAllAsync();
            return new CardResponse { Cards = cards.ToList() };
        }

        card ??= await cardRepository.GetFirstOrDefaultAsync(c => true);
        return new CardResponse
        {
            Card = card
        };
    }

    [Transactional]
    public async Task<CardResponse> Handle(CardCreate command, ICardRepository cardRepository,
        ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(cardRepository, nameof(cardRepository));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
            return new CardResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status400BadRequest.ToString(),
                    StatusCodes.Status400BadRequest,
                    "Validation failed",
                    JsonConvert.SerializeObject(validationErrors)
                )
            };

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

        await cardRepository.AddAsync(card);
        await cardRepository.SaveChangesAsync();

        return new CardResponse { Card = card };
    }

    [Transactional]
    public async Task<CardResponse> Handle(CardUpdate command, ICardRepository cardRepository,
        ICurrentUserService currentUserService, IValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(cardRepository, nameof(cardRepository));
        ArgumentNullException.ThrowIfNull(currentUserService, nameof(currentUserService));
        ArgumentNullException.ThrowIfNull(validationService, nameof(validationService));

        // Validate input
        var validationErrors = await validationService.ValidateAndGetErrorsAsync(command);
        if (validationErrors.Count != 0)
            return new CardResponse
            {
                ApiError = new ApiError(
                    StatusCodes.Status400BadRequest.ToString(),
                    StatusCodes.Status400BadRequest,
                    "Validation failed",
                    JsonConvert.SerializeObject(validationErrors)
                )
            };

        var card = await cardRepository.GetByIdAsync(command.Id);
        if (card == null) return new CardResponse { Card = null };

        card.Title = command.Title;
        card.Description = command.Description;
        card.ModifiedBy = currentUserService.GetCurrentUserName();
        card.ModifiedDate = DateTime.UtcNow;

        await cardRepository.UpdateAsync(card);
        await cardRepository.SaveChangesAsync();

        return new CardResponse { Card = card };
    }

    [Transactional]
    public async Task Handle(CardDelete command, ICardRepository cardRepository)
    {
        ArgumentNullException.ThrowIfNull(command, nameof(command));
        ArgumentNullException.ThrowIfNull(cardRepository, nameof(cardRepository));

        var card = await cardRepository.GetCardWithProductsAsync(command.Id);

        if (card != null)
        {
            // Delete related Products and their ProductDetails first
            if (card.Products != null && card.Products.Any())
                foreach (var product in card.Products)
                {
                    // Delete related ProductDetail and its children first
                    if (product.ProductDetail != null)
                        // Documents and Media will be deleted automatically due to cascade delete
                        await cardRepository.DeleteAsync(product.ProductDetail.Id);

                    // Delete the product
                    await cardRepository.DeleteAsync(product.Id);
                }

            // Now delete the card
            await cardRepository.DeleteAsync(card.Id);
            await cardRepository.SaveChangesAsync();
        }
    }
}