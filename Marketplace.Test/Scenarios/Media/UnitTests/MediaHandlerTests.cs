using Marketplace.Api.Endpoints.Media;
using Marketplace.Core.Interfaces;
using Marketplace.Core.Models.Media;
using Marketplace.Data.Interfaces;
using Marketplace.Test.Mocks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Marketplace.Test.Scenarios.Media.UnitTests;

public class MediaHandlerTests
{
    private readonly MockCurrentUserService _currentUserService;
    private readonly MediaHandler _handler;
    private readonly Mock<ILogger<MediaHandler>> _loggerMock;
    private readonly Mock<IMediaRepository> _mediaRepositoryMock;
    private readonly Mock<IS3MediaService> _s3MediaService;
    private readonly MockValidationService _validationService;

    public MediaHandlerTests()
    {
        _loggerMock = new Mock<ILogger<MediaHandler>>();
        _currentUserService = new MockCurrentUserService();
        _validationService = new MockValidationService();
        _mediaRepositoryMock = new Mock<IMediaRepository>();
        _s3MediaService = new Mock<IS3MediaService>();
        _handler = new MediaHandler();
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

        var expectedMedia = new Marketplace.Data.Entities.Media
        {
            Id = 1,
            Title = "Test Media",
            Description = "Test Description",
            FilePath = "/path/to/file.mp4",
            DirectoryPath = "/path/to/",
            MediaType = "Video",
            ProductDetailId = 1,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.Now
        };

        _mediaRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Marketplace.Data.Entities.Media>()))
            .ReturnsAsync(expectedMedia);
        _mediaRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var response = await _handler.Handle(createCommand, _mediaRepositoryMock.Object, _currentUserService,
            _validationService);

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

        _mediaRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingMedia);
        _mediaRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Marketplace.Data.Entities.Media>()))
            .ReturnsAsync(existingMedia);
        _mediaRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act

        var response = await _handler.Handle(updateCommand, _mediaRepositoryMock.Object, _currentUserService,
            _validationService);

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

        _mediaRepositoryMock.Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingMedia);
        _mediaRepositoryMock.Setup(x => x.DeleteAsync(1))
            .Returns(Task.CompletedTask);
        _mediaRepositoryMock.Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(deleteCommand, _mediaRepositoryMock.Object, _s3MediaService.Object, _loggerMock.Object);

        // Assert
        _mediaRepositoryMock.Verify(x => x.DeleteAsync(1), Times.Once);
        _mediaRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
    }
}