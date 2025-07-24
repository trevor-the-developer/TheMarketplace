using Marketplace.Api.Endpoints.Listing;
using Marketplace.Data.Entities;
using Marketplace.Data.Interfaces;
using Marketplace.Data.Repositories;
using Marketplace.Test.Mocks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Marketplace.Test.Scenarios.Listings.UnitTests;

public class ListingHandlerTests
{
    private readonly MockCurrentUserService _currentUserService;
    private readonly ListingHandler _handler;
    private readonly Mock<IListingRepository> _listingRepositoryMock;
    private readonly Mock<ILogger<ListingHandler>> _loggerMock;
    private readonly MockValidationService _validationService;

    public ListingHandlerTests()
    {
        _loggerMock = new Mock<ILogger<ListingHandler>>();
        _currentUserService = new MockCurrentUserService();
        _validationService = new MockValidationService();
        _listingRepositoryMock = new Mock<IListingRepository>();
        _handler = new ListingHandler();
    }

    [Fact]
    public void CreateListingHandler_Success()
    {
        // Arrange

        // Act
        var handler = new ListingHandler();

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task CreateListing_WithValidData_ReturnsListing()
    {
        // Arrange
        var createCommand = new ListingCreate
        {
            Title = "Test Listing",
            Description = "Test Description"
        };

        _listingRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Listing>()))
            .ReturnsAsync((Listing l) =>
            {
                l.Id = 1;
                return l;
            });
        _listingRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var response = await _handler.Handle(createCommand, _listingRepositoryMock.Object, _currentUserService,
            _validationService);

        // Assert
        Assert.NotNull(response.Listing);
        Assert.Equal("Test Listing", response.Listing.Title);
        Assert.Equal("Test Description", response.Listing.Description);
        _listingRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Listing>()), Times.Once);
        _listingRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateListing_WithValidData_ReturnsUpdatedListing()
    {
        // Arrange
        var updateCommand = new ListingUpdate
        {
            Id = 1,
            Title = "Updated Listing",
            Description = "Updated Description"
        };

        var existingListing = new Listing
        {
            Id = 1,
            Title = "Original Listing",
            Description = "Original Description",
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };

        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingListing);
        _listingRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Listing>()))
            .ReturnsAsync((Listing l) => l);
        _listingRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var response = await _handler.Handle(updateCommand, _listingRepositoryMock.Object, _currentUserService,
            _validationService);

        // Assert
        Assert.NotNull(response.Listing);
        Assert.Equal("Updated Listing", response.Listing.Title);
        Assert.Equal("Updated Description", response.Listing.Description);
        _listingRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _listingRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Listing>()), Times.Once);
        _listingRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteListing_WithValidId_DeletesListing()
    {
        // Arrange
        var deleteCommand = new ListingDelete { Id = 1 };

        var existingListing = new Listing
        {
            Id = 1,
            Title = "Listing to Delete",
            Description = "Description",
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };

        _listingRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingListing);
        _listingRepositoryMock.Setup(r => r.DeleteAsync(It.IsAny<Listing>()))
            .Returns(Task.CompletedTask);
        _listingRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(deleteCommand, _listingRepositoryMock.Object);

        // Assert
        _listingRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _listingRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Listing>()), Times.Once);
        _listingRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}