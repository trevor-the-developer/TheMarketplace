using FluentValidation.TestHelper;
using Marketplace.Api.Endpoints.Listing;
using Xunit;

namespace Marketplace.Test.Scenarios.Listings.UnitTests;

public class ListingCreateValidatorTests
{
    private readonly ListingCreateValidator _validator;

    public ListingCreateValidatorTests()
    {
        _validator = new ListingCreateValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Empty()
    {
        // Arrange
        var request = new ListingCreate { Title = "", Description = "Test description" };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required");
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Null()
    {
        // Arrange
        var request = new ListingCreate { Title = null!, Description = "Test description" };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required");
    }

    [Fact]
    public void Should_Have_Error_When_Title_Is_Whitespace()
    {
        // Arrange
        var request = new ListingCreate { Title = "   ", Description = "Test description" };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Title)
            .WithErrorMessage("Title is required");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        // Arrange
        var request = new ListingCreate { Title = "Test Listing", Description = "Test description" };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Not_Have_Error_When_Description_Is_Empty()
    {
        // Arrange
        var request = new ListingCreate { Title = "Test Listing", Description = "" };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Description_Is_Null()
    {
        // Arrange
        var request = new ListingCreate { Title = "Test Listing", Description = null! };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.Description);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Only_Title_Is_Provided()
    {
        // Arrange
        var request = new ListingCreate { Title = "Test Listing" };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }
}