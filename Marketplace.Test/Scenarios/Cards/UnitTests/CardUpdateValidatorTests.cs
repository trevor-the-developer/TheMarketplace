using FluentValidation.TestHelper;
using Marketplace.Api.Endpoints.Card;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Card;
using Marketplace.Core.Validators;
using Xunit;

namespace Marketplace.Test.Scenarios.Cards.UnitTests;

public class CardUpdateValidatorTests
{
    private readonly CardUpdateValidator _validator;

    public CardUpdateValidatorTests()
    {
        _validator = new CardUpdateValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Zero()
    {
        // Arrange
        var request = new CardUpdate
        {
            Id = 0,
            Title = "Test Card",
            Description = "Test Description"
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Id must be greater than 0");
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Negative()
    {
        // Arrange
        var request = new CardUpdate
        {
            Id = -1,
            Title = "Test Card",
            Description = "Test Description"
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Id must be greater than 0");
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        // Arrange
        var request = new CardUpdate
        {
            Id = 1,
            Title = "",
            Description = "Test Description"
        };

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
        var request = new CardUpdate
        {
            Id = 1,
            Title = longTitle,
            Description = "Test Description"
        };

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
        var request = new CardUpdate
        {
            Id = 1,
            Title = "Test Card",
            Description = longDescription
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Description)
            .WithErrorMessage("Description must not exceed 500 characters");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Description_Is_Empty()
    {
        // Arrange
        var request = new CardUpdate
        {
            Id = 1,
            Title = "Test Card",
            Description = ""
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        // Arrange
        var request = new CardUpdate
        {
            Id = 1,
            Title = "Test Card",
            Description = "Test Description"
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Allow_Maximum_Length_Title()
    {
        // Arrange
        var maxTitle = new string('a', 100); // 100 characters
        var request = new CardUpdate
        {
            Id = 1,
            Title = maxTitle,
            Description = "Test Description"
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Title);
    }

    [Fact]
    public void Should_Allow_Maximum_Length_Description()
    {
        // Arrange
        var maxDescription = new string('a', 500); // 500 characters
        var request = new CardUpdate
        {
            Id = 1,
            Title = "Test Card",
            Description = maxDescription
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Validate_Multiple_Errors()
    {
        // Arrange
        var request = new CardUpdate
        {
            Id = 0,
            Title = "",
            Description = new string('a', 501)
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Id);
        result.ShouldHaveValidationErrorFor(x => x.Title);
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }
}