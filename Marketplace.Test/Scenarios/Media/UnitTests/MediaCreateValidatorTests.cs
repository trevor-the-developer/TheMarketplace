using FluentValidation.TestHelper;
using Marketplace.Api.Endpoints.Media;
using Xunit;

namespace Marketplace.Test.Scenarios.Media.UnitTests
{
    public class MediaCreateValidatorTests
    {
        private readonly MediaCreateValidator _validator;

        public MediaCreateValidatorTests()
        {
            _validator = new MediaCreateValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Empty()
        {
            // Arrange
            var request = new MediaCreate { Title = "" };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Title)
                .WithErrorMessage("Title is required");
        }

        [Fact]
        public void Should_Have_Error_When_ProductDetailId_Is_Zero()
        {
            // Arrange
            var request = new MediaCreate { Title = "Test Media", ProductDetailId = 0 };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.ProductDetailId)
                .WithErrorMessage("ProductDetailId must be greater than 0");
        }

        [Fact]
        public void Should_Have_Error_When_ProductDetailId_Is_Negative()
        {
            // Arrange
            var request = new MediaCreate { Title = "Test Media", ProductDetailId = -1 };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.ProductDetailId)
                .WithErrorMessage("ProductDetailId must be greater than 0");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Request_Is_Valid()
        {
            // Arrange
            var request = new MediaCreate 
            { 
                Title = "Test Media",
                Description = "Test description",
                FilePath = "test.mp4",
                DirectoryPath = "media",
                MediaType = "video",
                ProductDetailId = 1
            };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_ProductDetailId_Is_Null()
        {
            // Arrange
            var request = new MediaCreate { Title = "Test Media", ProductDetailId = null };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveValidationErrorFor(x => x.ProductDetailId);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Optional_Fields_Are_Null()
        {
            // Arrange
            var request = new MediaCreate 
            { 
                Title = "Test Media",
                Description = null,
                FilePath = null,
                DirectoryPath = null,
                MediaType = null,
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
            var request = new MediaCreate 
            { 
                Title = "Test Media",
                Description = "",
                FilePath = "",
                DirectoryPath = "",
                MediaType = ""
            };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
            result.ShouldNotHaveValidationErrorFor(x => x.FilePath);
            result.ShouldNotHaveValidationErrorFor(x => x.DirectoryPath);
            result.ShouldNotHaveValidationErrorFor(x => x.MediaType);
        }

        [Fact]
        public void Should_Not_Have_Error_When_ProductDetailId_Is_Valid()
        {
            // Arrange
            var request = new MediaCreate { Title = "Test Media", ProductDetailId = 123 };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveValidationErrorFor(x => x.ProductDetailId);
        }

        [Fact]
        public void Should_Validate_Multiple_Errors()
        {
            // Arrange
            var request = new MediaCreate 
            { 
                Title = "", // Required
                ProductDetailId = 0 // Invalid
            };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Title);
            result.ShouldHaveValidationErrorFor(x => x.ProductDetailId);
        }
    }
}
