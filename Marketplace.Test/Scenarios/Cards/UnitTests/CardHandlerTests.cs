using System;
using System.Threading.Tasks;
using Marketplace.Api.Endpoints.Card;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Card;
using Marketplace.Data.Entities;
using Marketplace.Data.Interfaces;
using Marketplace.Test.Mocks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Marketplace.Test.Scenarios.Cards.UnitTests;

public class CardHandlerTests
{
    private readonly Mock<ICardRepository> _cardRepositoryMock;
    private readonly MockCurrentUserService _currentUserService;
    private readonly CardHandler _handler;
    private readonly Mock<ILogger<CardHandler>> _loggerMock;
    private readonly MockValidationService _validationService;

    public CardHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CardHandler>>();
        _currentUserService = new MockCurrentUserService();
        _validationService = new MockValidationService();
        _cardRepositoryMock = new Mock<ICardRepository>();
        _handler = new CardHandler();
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

        var expectedCard = new Card
        {
            Id = 1,
            Title = "Test Card",
            Description = "Test Description",
            ListingId = 1,
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };

        _cardRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Card>()))
            .ReturnsAsync(expectedCard);
        _cardRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var response = await _handler.Handle(createCommand, _cardRepositoryMock.Object, _currentUserService,
            _validationService);

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

        _cardRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingCard);
        _cardRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Card>()))
            .ReturnsAsync(existingCard);
        _cardRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var response = await _handler.Handle(updateCommand, _cardRepositoryMock.Object, _currentUserService,
            _validationService);

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

        _cardRepositoryMock.Setup(x => x.GetCardWithProductsAsync(1))
            .ReturnsAsync(existingCard);
        _cardRepositoryMock.Setup(x => x.DeleteAsync(1))
            .Returns(Task.CompletedTask);
        _cardRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(deleteCommand, _cardRepositoryMock.Object);

        // Assert
        _cardRepositoryMock.Verify(x => x.DeleteAsync(1), Times.Once);
        _cardRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}