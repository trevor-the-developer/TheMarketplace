using FluentValidation.TestHelper;
using Marketplace.Core.Models.Media;
using Marketplace.Core.Validators;
using Xunit;

namespace Marketplace.Test.Scenarios.Media.UnitTests;

public class MediaCreateWithFileValidatorTests
{
    private readonly MediaCreateWithFileValidator _validator;

    public MediaCreateWithFileValidatorTests()
    {
        _validator = new MediaCreateWithFileValidator();
    }

    #region File-Specific Validation Tests

    [Fact]
    public void Should_Have_Error_When_FileStream_Is_Null()
    {
        // Arrange
        var request = new MediaCreateWithFile
        {
            Title = "Test Media",
            FileStream = null,
            FileName = "test.jpg",
            ContentType = "image/jpeg"
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.FileStream)
            .WithErrorMessage("File is required for upload");
    }

    [Fact]
    public void Should_Have_Error_When_FileName_Is_Empty()
    {
        // Arrange
        using var fileStream = new MemoryStream();
        var request = new MediaCreateWithFile
        {
            Title = "Test Media",
            FileStream = fileStream,
            FileName = "",
            ContentType = "image/jpeg"
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.FileName)
            .WithErrorMessage("File name is required");
    }

    [Fact]
    public void Should_Have_Error_When_FileName_Is_Null()
    {
        // Arrange
        using var fileStream = new MemoryStream();
        var request = new MediaCreateWithFile
        {
            Title = "Test Media",
            FileStream = fileStream,
            FileName = null,
            ContentType = "image/jpeg"
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.FileName)
            .WithErrorMessage("File name is required");
    }

    [Fact]
    public void Should_Have_Error_When_ContentType_Is_Empty()
    {
        // Arrange
        using var fileStream = new MemoryStream();
        var request = new MediaCreateWithFile
        {
            Title = "Test Media",
            FileStream = fileStream,
            FileName = "test.jpg",
            ContentType = ""
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ContentType)
            .WithErrorMessage("Content type is required");
    }

    [Fact]
    public void Should_Have_Error_When_ContentType_Is_Null()
    {
        // Arrange
        using var fileStream = new MemoryStream();
        var request = new MediaCreateWithFile
        {
            Title = "Test Media",
            FileStream = fileStream,
            FileName = "test.jpg",
            ContentType = null
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ContentType)
            .WithErrorMessage("Content type is required");
    }

    #endregion

    #region Inherited MediaCreate Validation Tests

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        // Arrange
        using var fileStream = new MemoryStream();
        var request = new MediaCreateWithFile
        {
            Title = "",
            FileStream = fileStream,
            FileName = "test.jpg",
            ContentType = "image/jpeg"
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required");
    }

    [Fact]
    public void Should_Have_Error_When_ProductDetailId_Is_Zero()
    {
        // Arrange
        using var fileStream = new MemoryStream();
        var request = new MediaCreateWithFile
        {
            Title = "Test Media",
            FileStream = fileStream,
            FileName = "test.jpg",
            ContentType = "image/jpeg",
            ProductDetailId = 0
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ProductDetailId)
            .WithErrorMessage("ProductDetailId must be greater than 0");
    }

    [Fact]
    public void Should_Have_Error_When_ProductDetailId_Is_Negative()
    {
        // Arrange
        using var fileStream = new MemoryStream();
        var request = new MediaCreateWithFile
        {
            Title = "Test Media",
            FileStream = fileStream,
            FileName = "test.jpg",
            ContentType = "image/jpeg",
            ProductDetailId = -1
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.ProductDetailId)
            .WithErrorMessage("ProductDetailId must be greater than 0");
    }

    #endregion

    #region Valid Request Tests

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid_With_ProductDetailId()
    {
        // Arrange
        using var fileStream = new MemoryStream();
        var request = new MediaCreateWithFile
        {
            Title = "Test Media",
            Description = "Test description",
            MediaType = "Image",
            ProductDetailId = 1,
            FileStream = fileStream,
            FileName = "test.jpg",
            ContentType = "image/jpeg"
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid_Without_ProductDetailId()
    {
        // Arrange
        using var fileStream = new MemoryStream();
        var request = new MediaCreateWithFile
        {
            Title = "Test Media",
            Description = "Test description",
            MediaType = "Image",
            ProductDetailId = null,
            FileStream = fileStream,
            FileName = "test.jpg",
            ContentType = "image/jpeg"
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Optional_Fields_Are_Null()
    {
        // Arrange
        using var fileStream = new MemoryStream();
        var request = new MediaCreateWithFile
        {
            Title = "Test Media",
            Description = null,
            MediaType = null,
            ProductDetailId = null,
            FileStream = fileStream,
            FileName = "test.jpg",
            ContentType = "image/jpeg"
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Various_File_Types()
    {
        // Arrange
        using var fileStream = new MemoryStream();
        var testCases = new[]
        {
            ("image.jpg", "image/jpeg"),
            ("image.png", "image/png"),
            ("document.pdf", "application/pdf"),
            ("video.mp4", "video/mp4"),
            ("file.txt", "text/plain")
        };

        foreach (var (fileName, contentType) in testCases)
        {
            var request = new MediaCreateWithFile
            {
                Title = "Test Media",
                FileStream = fileStream,
                FileName = fileName,
                ContentType = contentType
            };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveAnyValidationErrors();
        }
    }

    #endregion

    #region Multiple Validation Errors Tests

    [Fact]
    public void Should_Validate_Multiple_Errors_File_Fields_Only()
    {
        // Arrange
        var request = new MediaCreateWithFile
        {
            Title = "Test Media",
            FileStream = null, // Required
            FileName = "", // Required
            ContentType = null // Required
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.FileStream);
        result.ShouldHaveValidationErrorFor(x => x.FileName);
        result.ShouldHaveValidationErrorFor(x => x.ContentType);
    }

    [Fact]
    public void Should_Validate_Multiple_Errors_All_Fields()
    {
        // Arrange
        var request = new MediaCreateWithFile
        {
            Title = "", // Required
            ProductDetailId = 0, // Invalid
            FileStream = null, // Required
            FileName = "", // Required
            ContentType = "" // Required
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Title);
        result.ShouldHaveValidationErrorFor(x => x.ProductDetailId);
        result.ShouldHaveValidationErrorFor(x => x.FileStream);
        result.ShouldHaveValidationErrorFor(x => x.FileName);
        result.ShouldHaveValidationErrorFor(x => x.ContentType);
    }

    [Fact]
    public void Should_Validate_File_Missing_But_Metadata_Present()
    {
        // Arrange - Metadata provided but no actual file stream
        var request = new MediaCreateWithFile
        {
            Title = "Test Media",
            FileStream = null, // Missing!
            FileName = "test.jpg",
            ContentType = "image/jpeg",
            ProductDetailId = 1
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.FileStream)
            .WithErrorMessage("File is required for upload");
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
        result.ShouldNotHaveValidationErrorFor(x => x.FileName);
        result.ShouldNotHaveValidationErrorFor(x => x.ContentType);
    }

    #endregion
}
