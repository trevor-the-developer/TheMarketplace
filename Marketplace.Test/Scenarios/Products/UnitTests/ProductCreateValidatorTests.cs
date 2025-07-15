using FluentValidation.TestHelper;
using Marketplace.Api.Endpoints.Product;
using Xunit;

namespace Marketplace.Test.Scenarios.Products.UnitTests;

public class ProductCreateValidatorTests
{
    private readonly ProductCreateValidator _validator;

    public ProductCreateValidatorTests()
    {
        _validator = new ProductCreateValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        // Arrange
        var request = new ProductCreate { Title = "" };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required");
    }

    [Fact]
    public void Should_Have_Error_When_Title_Exceeds_Maximum_Length()
    {
        // Arrange
        var longTitle = new string('a', 101); // 101 characters
        var request = new ProductCreate { Title = longTitle };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title must not exceed 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_Description_Exceeds_Maximum_Length()
    {
        // Arrange
        var longDescription = new string('a', 501); // 501 characters
        var request = new ProductCreate { Title = "Test title", Description = longDescription };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 500 characters");
    }

    [Fact]
    public void Should_Have_Error_When_ProductType_Exceeds_Maximum_Length()
    {
        // Arrange
        var longProductType = new string('a', 51); // 51 characters
        var request = new ProductCreate { Title = "Test title", ProductType = longProductType };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ProductType)
            .WithErrorMessage("ProductType must not exceed 50 characters");
    }

    [Fact]
    public void Should_Have_Error_When_Category_Exceeds_Maximum_Length()
    {
        // Arrange
        var longCategory = new string('a', 51); // 51 characters
        var request = new ProductCreate { Title = "Test title", Category = longCategory };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Category)
            .WithErrorMessage("Category must not exceed 50 characters");
    }

    [Fact]
    public void Should_Have_Error_When_CardId_Is_Zero()
    {
        // Arrange
        var request = new ProductCreate { Title = "Test title", CardId = 0 };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CardId)
            .WithErrorMessage("CardId must be greater than 0");
    }

    [Fact]
    public void Should_Have_Error_When_CardId_Is_Negative()
    {
        // Arrange
        var request = new ProductCreate { Title = "Test title", CardId = -1 };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.CardId)
            .WithErrorMessage("CardId must be greater than 0");
    }

    [Fact]
    public void Should_Have_Error_When_ProductDetailId_Is_Zero()
    {
        // Arrange
        var request = new ProductCreate { Title = "Test title", ProductDetailId = 0 };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ProductDetailId)
            .WithErrorMessage("ProductDetailId must be greater than 0");
    }

    [Fact]
    public void Should_Have_Error_When_ProductDetailId_Is_Negative()
    {
        // Arrange
        var request = new ProductCreate { Title = "Test title", ProductDetailId = -1 };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ProductDetailId)
            .WithErrorMessage("ProductDetailId must be greater than 0");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        // Arrange
        var request = new ProductCreate
        {
            Title = "Test title",
            Description = "Test description",
            ProductType = "Test type",
            Category = "Test category",
            IsEnabled = true,
            IsDeleted = false,
            CardId = 1,
            ProductDetailId = 1
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Optional_Fields_Are_Null()
    {
        // Arrange
        var request = new ProductCreate
        {
            Title = "Test title",
            Description = null,
            ProductType = null,
            Category = null,
            CardId = null,
            ProductDetailId = null
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Optional_Fields_Are_Empty()
    {
        // Arrange
        var request = new ProductCreate
        {
            Title = "Test title",
            Description = "",
            ProductType = "",
            Category = ""
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductType);
        result.ShouldNotHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Fields_Are_At_Maximum_Length()
    {
        // Arrange
        var request = new ProductCreate
        {
            Title = new string('a', 100), // Exactly 100 characters
            Description = new string('b', 500), // Exactly 500 characters
            ProductType = new string('c', 50), // Exactly 50 characters
            Category = new string('d', 50) // Exactly 50 characters
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductType);
        result.ShouldNotHaveValidationErrorFor(x => x.Category);
    }

    [Fact]
    public void Should_Validate_Multiple_Errors()
    {
        // Arrange
        var request = new ProductCreate
        {
            Title = "", // Too short
            Description = new string('a', 501), // Too long
            ProductType = new string('b', 51), // Too long
            Category = new string('c', 51), // Too long
            CardId = 0, // Invalid
            ProductDetailId = -1 // Invalid
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Title);
        result.ShouldHaveValidationErrorFor(x => x.Description);
        result.ShouldHaveValidationErrorFor(x => x.ProductType);
        result.ShouldHaveValidationErrorFor(x => x.Category);
        result.ShouldHaveValidationErrorFor(x => x.CardId);
        result.ShouldHaveValidationErrorFor(x => x.ProductDetailId);
    }
}