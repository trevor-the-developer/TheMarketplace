using Marketplace.Api.Endpoints.ProductDetail;
using Marketplace.Data;
using Marketplace.Data.Entities;
using Marketplace.Test.Mocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Marketplace.Test.Scenarios.ProductDetails.UnitTests;

public class ProductDetailHandlerTests : IDisposable
{
    private readonly MockCurrentUserService _currentUserService;
    private readonly MarketplaceDbContext _dbContext;
    private readonly ProductDetailHandler _handler;
    private readonly Mock<ILogger<ProductDetailHandler>> _loggerMock;
    private readonly MockValidationService _validationService;

    public ProductDetailHandlerTests()
    {
        _loggerMock = new Mock<ILogger<ProductDetailHandler>>();
        _currentUserService = new MockCurrentUserService();
        _validationService = new MockValidationService();
        _handler = new ProductDetailHandler();

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
    public void CreateProductDetailHandler_Success()
    {
        // Arrange

        // Act
        var handler = new ProductDetailHandler();

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task CreateProductDetail_WithValidData_ReturnsProductDetail()
    {
        // Arrange
        var createCommand = new ProductDetailCreate
        {
            Title = "Test Product Detail",
            Description = "Test Description",
            ProductId = 1
        };

        // Act
        var response = await _handler.Handle(createCommand, _dbContext, _currentUserService, _validationService);

        // Assert
        Assert.NotNull(response.ProductDetail);
        Assert.Equal("Test Product Detail", response.ProductDetail.Title);
        Assert.Equal("Test Description", response.ProductDetail.Description);
    }

    [Fact]
    public async Task UpdateProductDetail_WithValidData_ReturnsUpdatedProductDetail()
    {
        // Arrange
        var updateCommand = new ProductDetailUpdate
        {
            Id = 1,
            Title = "Updated Product Detail",
            Description = "Updated Description",
            ProductId = 1
        };

        // Act
        var existingProductDetail = new ProductDetail
        {
            Id = 1,
            Title = "Original Product Detail",
            Description = "Original Description",
            ProductId = 1,
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };
        _dbContext.ProductDetails.Add(existingProductDetail);
        await _dbContext.SaveChangesAsync();

        var response = await _handler.Handle(updateCommand, _dbContext, _currentUserService, _validationService);

        // Assert
        Assert.NotNull(response.ProductDetail);
        Assert.Equal("Updated Product Detail", response.ProductDetail.Title);
        Assert.Equal("Updated Description", response.ProductDetail.Description);
    }

    [Fact]
    public async Task DeleteProductDetail_WithValidId_DeletesProductDetail()
    {
        // Arrange
        var deleteCommand = new ProductDetailDelete { Id = 1 };

        // Act
        var existingProductDetail = new ProductDetail
        {
            Id = 1,
            Title = "Product Detail to Delete",
            Description = "Description",
            ProductId = 1,
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };
        _dbContext.ProductDetails.Add(existingProductDetail);
        await _dbContext.SaveChangesAsync();

        await _handler.Handle(deleteCommand, _dbContext);

        // Assert
        var productDetail = await _dbContext.ProductDetails.FindAsync(deleteCommand.Id);
        Assert.Null(productDetail);
    }
}
