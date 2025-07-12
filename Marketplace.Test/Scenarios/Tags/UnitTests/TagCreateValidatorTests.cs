using FluentValidation.TestHelper;
using Marketplace.Api.Endpoints.Tag;
using Xunit;

namespace Marketplace.Test.Scenarios.Tags.UnitTests
{
    public class TagCreateValidatorTests
    {
        private readonly TagCreateValidator _validator;

        public TagCreateValidatorTests()
        {
            _validator = new TagCreateValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            // Arrange
            var request = new TagCreate { Name = "", Description = "Test description" };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Name is required");
        }

        [Fact]
        public void Should_Have_Error_When_Name_Exceeds_Maximum_Length()
        {
            // Arrange
            var longName = new string('a', 101); // 101 characters
            var request = new TagCreate { Name = longName, Description = "Test description" };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Name)
                .WithErrorMessage("Name must not exceed 100 characters");
        }

        [Fact]
        public void Should_Have_Error_When_Description_Exceeds_Maximum_Length()
        {
            // Arrange
            var longDescription = new string('a', 501); // 501 characters
            var request = new TagCreate { Name = "Test tag", Description = longDescription };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description must not exceed 500 characters");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Request_Is_Valid()
        {
            // Arrange
            var request = new TagCreate 
            { 
                Name = "Test Tag", 
                Description = "Test description",
                IsEnabled = true
            };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Empty()
        {
            // Arrange
            var request = new TagCreate { Name = "Test Tag", Description = "" };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Null()
        {
            // Arrange
            var request = new TagCreate { Name = "Test Tag", Description = null };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Name_Is_At_Maximum_Length()
        {
            // Arrange
            var maxName = new string('a', 100); // Exactly 100 characters
            var request = new TagCreate { Name = maxName, Description = "Test description" };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveValidationErrorFor(x => x.Name);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_At_Maximum_Length()
        {
            // Arrange
            var maxDescription = new string('a', 500); // Exactly 500 characters
            var request = new TagCreate { Name = "Test Tag", Description = maxDescription };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_IsEnabled_Is_Null()
        {
            // Arrange
            var request = new TagCreate { Name = "Test Tag", IsEnabled = null };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveValidationErrorFor(x => x.IsEnabled);
        }

        [Fact]
        public void Should_Validate_Multiple_Errors()
        {
            // Arrange
            var request = new TagCreate 
            { 
                Name = "", // Too short
                Description = new string('a', 501) // Too long
            };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Name);
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }
    }
}
