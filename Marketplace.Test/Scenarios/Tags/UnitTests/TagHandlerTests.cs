using Marketplace.Api.Endpoints.Tag;
using Marketplace.Data;
using Marketplace.Data.Entities;
using Marketplace.Test.Mocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Marketplace.Test.Scenarios.Tags.UnitTests;

public class TagHandlerTests : IDisposable
{
    private readonly MockCurrentUserService _currentUserService;
    private readonly MarketplaceDbContext _dbContext;
    private readonly TagHandler _handler;
    private readonly Mock<ILogger<TagHandler>> _loggerMock;
    private readonly MockValidationService _validationService;

    public TagHandlerTests()
    {
        _loggerMock = new Mock<ILogger<TagHandler>>();
        _currentUserService = new MockCurrentUserService();
        _validationService = new MockValidationService();
        _handler = new TagHandler();

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
    public void CreateTagHandler_Success()
    {
        // Arrange

        // Act
        var handler = new TagHandler();

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task CreateTag_WithValidData_ReturnsTag()
    {
        // Arrange
        var createCommand = new TagCreate
        {
            Name = "Test Tag",
            Description = "Test Description",
            IsEnabled = true
        };

        // Act
        var response = await _handler.Handle(createCommand, _dbContext, _currentUserService, _validationService);

        // Assert
        Assert.NotNull(response.Tag);
        Assert.Equal("Test Tag", response.Tag.Name);
        Assert.Equal("Test Description", response.Tag.Description);
        Assert.True(response.Tag.IsEnabled);
    }

    [Fact]
    public async Task UpdateTag_WithValidData_ReturnsUpdatedTag()
    {
        // Arrange
        var updateCommand = new TagUpdate
        {
            Id = 1,
            Name = "Updated Tag",
            Description = "Updated Description",
            IsEnabled = false
        };

        // Act
        var existingTag = new Tag
        {
            Id = 1,
            Name = "Original Tag",
            Description = "Original Description",
            IsEnabled = true,
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };
        _dbContext.Tags.Add(existingTag);
        await _dbContext.SaveChangesAsync();

        var response = await _handler.Handle(updateCommand, _dbContext, _currentUserService, _validationService);

        // Assert
        Assert.NotNull(response.Tag);
        Assert.Equal("Updated Tag", response.Tag.Name);
        Assert.Equal("Updated Description", response.Tag.Description);
        Assert.False(response.Tag.IsEnabled);
    }

    [Fact]
    public async Task DeleteTag_WithValidId_DeletesTag()
    {
        // Arrange
        var deleteCommand = new TagDelete { Id = 1 };

        // Act
        var existingTag = new Tag
        {
            Id = 1,
            Name = "Tag to Delete",
            Description = "Description",
            IsEnabled = true,
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };
        _dbContext.Tags.Add(existingTag);
        await _dbContext.SaveChangesAsync();

        await _handler.Handle(deleteCommand, _dbContext);

        // Assert
        var tag = await _dbContext.Tags.FindAsync(deleteCommand.Id);
        Assert.Null(tag);
    }
}