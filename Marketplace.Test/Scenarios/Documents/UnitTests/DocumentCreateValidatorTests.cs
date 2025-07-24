using FluentValidation.TestHelper;
using Marketplace.Api.Endpoints.Document;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Document;
using Marketplace.Core.Validators;
using Xunit;

namespace Marketplace.Test.Scenarios.Documents.UnitTests;

public class DocumentCreateValidatorTests
{
    private readonly DocumentCreateValidator _validator;

    public DocumentCreateValidatorTests()
    {
        _validator = new DocumentCreateValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        // Arrange
        var request = new DocumentCreate { Title = "", DocumentType = "PDF" };

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
        var request = new DocumentCreate { Title = longTitle, DocumentType = "PDF" };

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
        var request = new DocumentCreate
            { Title = "Test Document", Description = longDescription, DocumentType = "PDF" };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 500 characters");
    }

    [Fact]
    public void Should_Have_Error_When_DocumentType_Is_Empty()
    {
        // Arrange
        var request = new DocumentCreate { Title = "Test Document", DocumentType = "" };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.DocumentType)
            .WithErrorMessage("DocumentType is required");
    }

    [Fact]
    public void Should_Have_Error_When_DocumentType_Exceeds_Maximum_Length()
    {
        // Arrange
        var longDocumentType = new string('a', 51); // 51 characters
        var request = new DocumentCreate { Title = "Test Document", DocumentType = longDocumentType };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.DocumentType)
            .WithErrorMessage("DocumentType must not exceed 50 characters");
    }

    [Fact]
    public void Should_Have_Error_When_ProductDetailId_Is_Zero()
    {
        // Arrange
        var request = new DocumentCreate { Title = "Test Document", DocumentType = "PDF", ProductDetailId = 0 };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ProductDetailId)
            .WithErrorMessage("ProductDetailId must be greater than 0");
    }

    [Fact]
    public void Should_Have_Error_When_ProductDetailId_Is_Negative()
    {
        // Arrange
        var request = new DocumentCreate { Title = "Test Document", DocumentType = "PDF", ProductDetailId = -1 };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ProductDetailId)
            .WithErrorMessage("ProductDetailId must be greater than 0");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        // Arrange
        var request = new DocumentCreate
        {
            Title = "Test Document",
            Description = "Test description",
            Text = "Document content",
            Bytes = "Base64EncodedBytes",
            DocumentType = "PDF",
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
        var request = new DocumentCreate
        {
            Title = "Test Document",
            Description = null,
            Text = null,
            Bytes = null,
            DocumentType = "PDF",
            ProductDetailId = null
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Description_Is_Empty()
    {
        // Arrange
        var request = new DocumentCreate { Title = "Test Document", Description = "", DocumentType = "PDF" };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Not_Have_Error_When_ProductDetailId_Is_Null()
    {
        // Arrange
        var request = new DocumentCreate { Title = "Test Document", DocumentType = "PDF", ProductDetailId = null };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.ProductDetailId);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Fields_Are_At_Maximum_Length()
    {
        // Arrange
        var request = new DocumentCreate
        {
            Title = new string('a', 100), // Exactly 100 characters
            Description = new string('b', 500), // Exactly 500 characters
            DocumentType = new string('c', 50) // Exactly 50 characters
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
        result.ShouldNotHaveValidationErrorFor(x => x.DocumentType);
    }

    [Fact]
    public void Should_Validate_Multiple_Errors()
    {
        // Arrange
        var request = new DocumentCreate
        {
            Title = "", // Required
            Description = new string('a', 501), // Too long
            DocumentType = "", // Required
            ProductDetailId = 0 // Invalid
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Title);
        result.ShouldHaveValidationErrorFor(x => x.Description);
        result.ShouldHaveValidationErrorFor(x => x.DocumentType);
        result.ShouldHaveValidationErrorFor(x => x.ProductDetailId);
    }
}