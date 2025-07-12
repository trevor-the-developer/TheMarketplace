using FluentValidation.TestHelper;
using Marketplace.Api.Endpoints.Card;
using Xunit;

namespace Marketplace.Test.Scenarios.Cards.UnitTests
{
    public class CardCreateValidatorTests
    {
        private readonly CardCreateValidator _validator;

        public CardCreateValidatorTests()
        {
            _validator = new CardCreateValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Title_Is_Empty()
        {
            // Arrange
            var request = new CardCreate { Title = "", Description = "Test description", ListingId = 1 };

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
            var request = new CardCreate { Title = longTitle, Description = "Test description", ListingId = 1 };

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
            var request = new CardCreate { Title = "Test title", Description = longDescription, ListingId = 1 };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Description)
                .WithErrorMessage("Description must not exceed 500 characters");
        }

        [Fact]
        public void Should_Have_Error_When_ListingId_Is_Zero()
        {
            // Arrange
            var request = new CardCreate { Title = "Test title", Description = "Test description", ListingId = 0 };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.ListingId)
                .WithErrorMessage("ListingId must be greater than 0");
        }

        [Fact]
        public void Should_Have_Error_When_ListingId_Is_Negative()
        {
            // Arrange
            var request = new CardCreate { Title = "Test title", Description = "Test description", ListingId = -1 };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.ListingId)
                .WithErrorMessage("ListingId must be greater than 0");
        }

        [Fact]
        public void Should_Not_Have_Error_When_Request_Is_Valid()
        {
            // Arrange
            var request = new CardCreate { Title = "Test title", Description = "Test description", ListingId = 1 };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_Empty()
        {
            // Arrange
            var request = new CardCreate { Title = "Test title", Description = "", ListingId = 1 };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Description_Is_At_Maximum_Length()
        {
            // Arrange
            var maxDescription = new string('a', 500); // Exactly 500 characters
            var request = new CardCreate { Title = "Test title", Description = maxDescription, ListingId = 1 };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Title_Is_At_Maximum_Length()
        {
            // Arrange
            var maxTitle = new string('a', 100); // Exactly 100 characters
            var request = new CardCreate { Title = maxTitle, Description = "Test description", ListingId = 1 };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldNotHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Validate_Multiple_Errors()
        {
            // Arrange
            var request = new CardCreate { Title = "", Description = new string('a', 501), ListingId = 0 };

            // Act & Assert
            var result = _validator.TestValidate(request);
            result.ShouldHaveValidationErrorFor(x => x.Title);
            result.ShouldHaveValidationErrorFor(x => x.Description);
            result.ShouldHaveValidationErrorFor(x => x.ListingId);
        }
    }
}
