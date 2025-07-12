using Marketplace.Api.Endpoints.Product;
using Marketplace.Data;
using Marketplace.Data.Entities;
using Marketplace.Test.Mocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Marketplace.Test.Scenarios.Products.UnitTests;

public class ProductHandlerTests : IDisposable
{
    private readonly MockCurrentUserService _currentUserService;
    private readonly MarketplaceDbContext _dbContext;
    private readonly ProductHandler _handler;
    private readonly Mock<ILogger<ProductHandler>> _loggerMock;
    private readonly MockValidationService _validationService;

    public ProductHandlerTests()
    {
        _loggerMock = new Mock<ILogger<ProductHandler>>();
        _currentUserService = new MockCurrentUserService();
        _validationService = new MockValidationService();
        _handler = new ProductHandler();

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
    public void CreateProductHandler_Success()
    {
        // Arrange

        // Act
        var handler = new ProductHandler();

        // Assert
        Assert.NotNull(handler);
    }

    [Fact]
    public async Task CreateProduct_WithValidData_ReturnsProduct()
    {
        // Arrange
        var createCommand = new ProductCreate
        {
            Title = "Test Product",
            Description = "Test Description",
            ProductType = "Sample Type",
            Category = "Sample Category",
            IsEnabled = true,
            IsDeleted = false
        };

        // Act
        var response = await _handler.Handle(createCommand, _dbContext, _currentUserService, _validationService);

        // Assert
        Assert.NotNull(response.Product);
        Assert.Equal("Test Product", response.Product.Title);
        Assert.Equal("Test Description", response.Product.Description);
    }

    [Fact]
    public async Task UpdateProduct_WithValidData_ReturnsUpdatedProduct()
    {
        // Arrange
        var updateCommand = new ProductUpdate
        {
            Id = 1,
            Title = "Updated Product",
            Description = "Updated Description",
            ProductType = "Updated Type",
            Category = "Updated Category",
            IsEnabled = true,
            IsDeleted = false
        };

        // Act
        var existingProduct = new Product
        {
            Id = 1,
            Title = "Original Product",
            Description = "Original Description",
            ProductType = "Original Type",
            Category = "Original Category",
            IsEnabled = true,
            IsDeleted = false,
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };
        _dbContext.Products.Add(existingProduct);
        await _dbContext.SaveChangesAsync();

        var response = await _handler.Handle(updateCommand, _dbContext, _currentUserService, _validationService);

        // Assert
        Assert.NotNull(response.Product);
        Assert.Equal("Updated Product", response.Product.Title);
        Assert.Equal("Updated Description", response.Product.Description);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_DeletesProduct()
    {
        // Arrange
        var deleteCommand = new ProductDelete { Id = 1 };

        // Act
        var existingProduct = new Product
        {
            Id = 1,
            Title = "Product to Delete",
            Description = "Description",
            ProductType = "Type",
            Category = "Category",
            IsEnabled = true,
            IsDeleted = false,
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };
        _dbContext.Products.Add(existingProduct);
        await _dbContext.SaveChangesAsync();

        await _handler.Handle(deleteCommand, _dbContext);

        // Assert
        var product = await _dbContext.Products.FindAsync(deleteCommand.Id);
        Assert.Null(product);
    }
}
