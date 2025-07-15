using Marketplace.Api.Endpoints.UserProfile;
using Marketplace.Data;
using Marketplace.Data.Entities;
using Marketplace.Test.Mocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Marketplace.Test.Scenarios.UserProfiles.UnitTests;

public class UserProfileHandlerTests : IDisposable
{
    private readonly MockCurrentUserService _currentUserService;
    private readonly MarketplaceDbContext _dbContext;
    private readonly UserProfileHandler _handler;
    private readonly Mock<ILogger<UserProfileHandler>> _loggerMock;
    private readonly MockValidationService _validationService;

    public UserProfileHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UserProfileHandler>>();
        _currentUserService = new MockCurrentUserService();
        _validationService = new MockValidationService();
        _handler = new UserProfileHandler();

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
    public void CreateUserProfileHandler_Success()
    {
        // Arrange

        // Act
        var handler = new UserProfileHandler();

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task CreateUserProfile_WithValidData_ReturnsUserProfile()
    {
        // Arrange
        var createCommand = new UserProfileCreate
        {
            DisplayName = "Test User",
            Bio = "Test Bio",
            SocialMedia = "test@social.com",
            ApplicationUserId = "test-user-id-12345"
        };

        // Act
        var response = await _handler.Handle(createCommand, _dbContext, _currentUserService, _validationService);

        // Assert
        Assert.NotNull(response.UserProfile);
        Assert.Equal("Test User", response.UserProfile.DisplayName);
        Assert.Equal("Test Bio", response.UserProfile.Bio);
        Assert.Equal("test@social.com", response.UserProfile.SocialMedia);
    }

    [Fact]
    public async Task UpdateUserProfile_WithValidData_ReturnsUpdatedUserProfile()
    {
        // Arrange
        var updateCommand = new UserProfileUpdate
        {
            ApplicationUserId = "test-user-id-12345",
            DisplayName = "Updated User",
            Bio = "Updated Bio",
            SocialMedia = "updated@social.com"
        };

        // Act
        var existingUserProfile = new UserProfile
        {
            Id = 1,
            DisplayName = "Original User",
            Bio = "Original Bio",
            SocialMedia = "original@social.com",
            ApplicationUserId = "test-user-id-12345",
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };
        _dbContext.Profiles.Add(existingUserProfile);
        await _dbContext.SaveChangesAsync();

        var response = await _handler.Handle(updateCommand, _dbContext, _currentUserService, _validationService);

        // Assert
        Assert.NotNull(response.UserProfile);
        Assert.Equal("Updated User", response.UserProfile.DisplayName);
        Assert.Equal("Updated Bio", response.UserProfile.Bio);
        Assert.Equal("updated@social.com", response.UserProfile.SocialMedia);
    }

    [Fact]
    public async Task DeleteUserProfile_WithValidId_DeletesUserProfile()
    {
        // Arrange
        var deleteCommand = new UserProfileDelete { ApplicationUserId = "test-user-id-12345" };

        // Act
        var existingUserProfile = new UserProfile
        {
            Id = 1,
            DisplayName = "User to Delete",
            Bio = "Bio",
            SocialMedia = "delete@social.com",
            ApplicationUserId = "test-user-id-12345",
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };
        _dbContext.Profiles.Add(existingUserProfile);
        await _dbContext.SaveChangesAsync();

        await _handler.Handle(deleteCommand, _dbContext);

        // Assert
        var userProfile =
            await _dbContext.Profiles.FirstOrDefaultAsync(up =>
                up.ApplicationUserId == deleteCommand.ApplicationUserId);
        Assert.Null(userProfile);
    }
}