using Marketplace.Api.Endpoints.Media;
using Marketplace.Data;
using Marketplace.Test.Mocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Marketplace.Test.Scenarios.Media.UnitTests;

public class MediaHandlerTests : IDisposable
{
    private readonly MockCurrentUserService _currentUserService;
    private readonly MarketplaceDbContext _dbContext;
    private readonly MediaHandler _handler;
    private readonly Mock<ILogger<MediaHandler>> _loggerMock;
    private readonly MockValidationService _validationService;

    public MediaHandlerTests()
    {
        _loggerMock = new Mock<ILogger<MediaHandler>>();
        _currentUserService = new MockCurrentUserService();
        _validationService = new MockValidationService();
        _handler = new MediaHandler();

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
    public void CreateMediaHandler_Success()
    {
        // Arrange

        // Act
        var handler = new MediaHandler();

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task CreateMedia_WithValidData_ReturnsMedia()
    {
        // Arrange
        var createCommand = new MediaCreate
        {
            Title = "Test Media",
            Description = "Test Description",
            FilePath = "/path/to/file.mp4",
            DirectoryPath = "/path/to/",
            MediaType = "Video",
            ProductDetailId = 1
        };

        // Act
        var response = await _handler.Handle(createCommand, _dbContext, _currentUserService, _validationService);

        // Assert
        Assert.NotNull(response.Media);
        Assert.Equal("Test Media", response.Media.Title);
        Assert.Equal("Test Description", response.Media.Description);
        Assert.Equal("Video", response.Media.MediaType);
    }

    [Fact]
    public async Task UpdateMedia_WithValidData_ReturnsUpdatedMedia()
    {
        // Arrange
        var updateCommand = new MediaUpdate
        {
            Id = 1,
            Title = "Updated Media",
            Description = "Updated Description",
            FilePath = "/path/to/updated.mp4",
            DirectoryPath = "/path/to/updated/",
            MediaType = "Audio",
            ProductDetailId = 1
        };

        // Act
        var existingMedia = new Marketplace.Data.Entities.Media
        {
            Id = 1,
            Title = "Original Media",
            Description = "Original Description",
            FilePath = "/path/to/original.mp4",
            DirectoryPath = "/path/to/original/",
            MediaType = "Video",
            ProductDetailId = 1,
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };
        _dbContext.Files.Add(existingMedia);
        await _dbContext.SaveChangesAsync();

        var response = await _handler.Handle(updateCommand, _dbContext, _currentUserService, _validationService);

        // Assert
        Assert.NotNull(response.Media);
        Assert.Equal("Updated Media", response.Media.Title);
        Assert.Equal("Updated Description", response.Media.Description);
        Assert.Equal("Audio", response.Media.MediaType);
    }

    [Fact]
    public async Task DeleteMedia_WithValidId_DeletesMedia()
    {
        // Arrange
        var deleteCommand = new MediaDelete { Id = 1 };

        // Act
        var existingMedia = new Marketplace.Data.Entities.Media
        {
            Id = 1,
            Title = "Media to Delete",
            Description = "Description",
            FilePath = "/path/to/file.mp4",
            DirectoryPath = "/path/to/",
            MediaType = "Video",
            ProductDetailId = 1,
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };
        _dbContext.Files.Add(existingMedia);
        await _dbContext.SaveChangesAsync();

        await _handler.Handle(deleteCommand, _dbContext);

        // Assert
        var media = await _dbContext.Files.FindAsync(deleteCommand.Id);
        Assert.Null(media);
    }
}