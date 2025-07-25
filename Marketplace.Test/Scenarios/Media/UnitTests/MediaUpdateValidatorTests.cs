using FluentValidation.TestHelper;
using Marketplace.Core.Models.Media;
using Marketplace.Core.Validators;
using Xunit;

namespace Marketplace.Test.Scenarios.Media.UnitTests;

public class MediaUpdateValidatorTests
{
    private readonly MediaUpdateValidator _validator;

    public MediaUpdateValidatorTests()
    {
        _validator = new MediaUpdateValidator();
    }

    [Fact]
    public void Should_Have_Error_When_Id_Is_Zero()
    {
        // Arrange
        var request = new MediaUpdate
        {
            Id = 0,
            Title = "Test Media",
            Description = "Test Description"
        };
        
        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Id)
            .WithErrorMessage("Id must be greater than 0");
    }
}