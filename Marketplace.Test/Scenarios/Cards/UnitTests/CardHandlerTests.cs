using Marketplace.Api.Endpoints.Card;
using Marketplace.Data;
using Marketplace.Data.Entities;
using Marketplace.Test.Mocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Marketplace.Test.Scenarios.Cards.UnitTests;

public class CardHandlerTests : IDisposable
{
    private readonly MockCurrentUserService _currentUserService;
    private readonly MarketplaceDbContext _dbContext;
    private readonly CardHandler _handler;
    private readonly Mock<ILogger<CardHandler>> _loggerMock;

    public CardHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CardHandler>>();
        _currentUserService = new MockCurrentUserService();
        _handler = new CardHandler();

        var options = new DbContextOptionsBuilder<MarketplaceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _dbContext = new MarketplaceDbContext(options);
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    [Fact]
    public void CreateCardHandler_Success()
    {
        // Arrange

        // Act
        var handler = new CardHandler();

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task CreateCard_WithValidData_ReturnsCard()
    {
        // Arrange
        var createCommand = new CardCreate
        {
            Title = "Test Card",
            Description = "Test Description",
            ListingId = 1
        };

        // Act
        var response = await _handler.Handle(createCommand, _dbContext, _currentUserService);

        // Assert
        Assert.NotNull(response.Card);
        Assert.Equal("Test Card", response.Card.Title);
        Assert.Equal("Test Description", response.Card.Description);
    }

    [Fact]
    public async Task UpdateCard_WithValidData_ReturnsUpdatedCard()
    {
        // Arrange
        var updateCommand = new CardUpdate
        {
            Id = 1,
            Title = "Updated Card",
            Description = "Updated Description"
        };

        // Act
        // First create a card to update
        var existingCard = new Card
        {
            Id = 1,
            Title = "Original Card",
            Description = "Original Description",
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };
        _dbContext.Cards.Add(existingCard);
        await _dbContext.SaveChangesAsync();

        var response = await _handler.Handle(updateCommand, _dbContext, _currentUserService);

        // Assert
        Assert.NotNull(response.Card);
        Assert.Equal("Updated Card", response.Card.Title);
        Assert.Equal("Updated Description", response.Card.Description);
    }

    [Fact]
    public async Task DeleteCard_WithValidId_DeletesCard()
    {
        // Arrange
        var deleteCommand = new CardDelete { Id = 1 };

        // Act
        // First create a card to delete
        var existingCard = new Card
        {
            Id = 1,
            Title = "Card to Delete",
            Description = "Description",
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };
        _dbContext.Cards.Add(existingCard);
        await _dbContext.SaveChangesAsync();

        await _handler.Handle(deleteCommand, _dbContext);

        // Assert
        var card = await _dbContext.Cards.FindAsync(deleteCommand.Id);
        Assert.Null(card);
    }
}