using Marketplace.Api.Endpoints.Listing;
using Marketplace.Data;
using Marketplace.Data.Entities;
using Marketplace.Test.Mocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Marketplace.Test.Scenarios.Listings.UnitTests;

public class ListingHandlerTests : IDisposable
{
    private readonly MockCurrentUserService _currentUserService;
    private readonly MarketplaceDbContext _dbContext;
    private readonly ListingHandler _handler;
    private readonly Mock<ILogger<ListingHandler>> _loggerMock;

    public ListingHandlerTests()
    {
        _loggerMock = new Mock<ILogger<ListingHandler>>();
        _currentUserService = new MockCurrentUserService();
        _handler = new ListingHandler();

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

        // Act
        var response = await _handler.Handle(createCommand, _dbContext, _currentUserService);

        // Assert
        Assert.NotNull(response.Listing);
        Assert.Equal("Test Listing", response.Listing.Title);
        Assert.Equal("Test Description", response.Listing.Description);
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

        // Act
        // First create a listing to update
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
        _dbContext.Listings.Add(existingListing);
        await _dbContext.SaveChangesAsync();

        var response = await _handler.Handle(updateCommand, _dbContext, _currentUserService);

        // Assert
        Assert.NotNull(response.Listing);
        Assert.Equal("Updated Listing", response.Listing.Title);
        Assert.Equal("Updated Description", response.Listing.Description);
    }

    [Fact]
    public async Task DeleteListing_WithValidId_DeletesListing()
    {
        // Arrange
        var deleteCommand = new ListingDelete { Id = 1 };

        // Act
        // First create a listing to delete
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
        _dbContext.Listings.Add(existingListing);
        await _dbContext.SaveChangesAsync();

        await _handler.Handle(deleteCommand, _dbContext);

        // Assert
        var listing = await _dbContext.Listings.FindAsync(deleteCommand.Id);
        Assert.Null(listing);
    }
}