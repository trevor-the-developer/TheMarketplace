using Marketplace.Api.Endpoints.Product;
using Marketplace.Data.Entities;
using Marketplace.Data.Interfaces;
using Marketplace.Data.Repositories;
using Marketplace.Test.Mocks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Marketplace.Test.Scenarios.Products.UnitTests;

public class ProductHandlerTests
{
    private readonly MockCurrentUserService _currentUserService;
    private readonly ProductHandler _handler;
    private readonly Mock<ILogger<ProductHandler>> _loggerMock;
    private readonly Mock<IProductDetailRepository> _productDetailRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly MockValidationService _validationService;

    public ProductHandlerTests()
    {
        _loggerMock = new Mock<ILogger<ProductHandler>>();
        _currentUserService = new MockCurrentUserService();
        _validationService = new MockValidationService();
        _productRepositoryMock = new Mock<IProductRepository>();
        _productDetailRepositoryMock = new Mock<IProductDetailRepository>();
        _handler = new ProductHandler();
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

        var expectedProduct = new Product
        {
            Id = 1,
            Title = "Test Product",
            Description = "Test Description",
            ProductType = "Sample Type",
            Category = "Sample Category",
            IsEnabled = true,
            IsDeleted = false,
            CreatedBy = "TestUser",
            CreatedDate = DateTime.UtcNow,
            ModifiedBy = "TestUser",
            ModifiedDate = DateTime.UtcNow
        };

        _productRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) =>
            {
                p.Id = 1;
                return p;
            });
        _productRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var response = await _handler.Handle(createCommand, _productRepositoryMock.Object, _currentUserService,
            _validationService);

        // Assert
        Assert.NotNull(response.Product);
        Assert.Equal("Test Product", response.Product.Title);
        Assert.Equal("Test Description", response.Product.Description);
        _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
        _productRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
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

        _productRepositoryMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);
        _productRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var response = await _handler.Handle(updateCommand, _productRepositoryMock.Object, _currentUserService,
            _validationService);

        // Assert
        Assert.NotNull(response.Product);
        Assert.Equal("Updated Product", response.Product.Title);
        Assert.Equal("Updated Description", response.Product.Description);
        _productRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
        _productRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Product>()), Times.Once);
        _productRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteProduct_WithValidId_DeletesProduct()
    {
        // Arrange
        var deleteCommand = new ProductDelete { Id = 1 };

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

        _productRepositoryMock.Setup(r => r.GetProductWithDetailsAsync(1))
            .ReturnsAsync(existingProduct);
        _productRepositoryMock.Setup(r => r.DeleteAsync(1))
            .Returns(Task.CompletedTask);
        _productRepositoryMock.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        await _handler.Handle(deleteCommand, _productRepositoryMock.Object, _productDetailRepositoryMock.Object);

        // Assert
        _productRepositoryMock.Verify(r => r.GetProductWithDetailsAsync(1), Times.Once);
        _productRepositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
        _productRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}