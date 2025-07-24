using System;
using FluentValidation.TestHelper;
using Marketplace.Api.Endpoints.Authentication.Registration;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Registration;
using Marketplace.Core.Validators;
using Marketplace.Data.Enums;
using Xunit;

namespace Marketplace.Test.Scenarios.Authentication.UnitTests.Registration;

public class RegisterRequestValidatorTests
{
    private readonly RegisterRequestValidator _validator;

    public RegisterRequestValidatorTests()
    {
        _validator = new RegisterRequestValidator();
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Is_Empty()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "password123",
            DateOfBirth = DateTime.Today.AddYears(-20),
            Role = Role.User
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("FirstName is required");
    }

    [Fact]
    public void Should_Have_Error_When_FirstName_Exceeds_Maximum_Length()
    {
        // Arrange
        var longFirstName = new string('a', 101); // 101 characters
        var request = new RegisterRequest
        {
            FirstName = longFirstName,
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "password123",
            DateOfBirth = DateTime.Today.AddYears(-20),
            Role = Role.User
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.FirstName)
            .WithErrorMessage("FirstName must not exceed 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Is_Empty()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "",
            Email = "john.doe@example.com",
            Password = "password123",
            DateOfBirth = DateTime.Today.AddYears(-20),
            Role = Role.User
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("LastName is required");
    }

    [Fact]
    public void Should_Have_Error_When_LastName_Exceeds_Maximum_Length()
    {
        // Arrange
        var longLastName = new string('a', 101); // 101 characters
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = longLastName,
            Email = "john.doe@example.com",
            Password = "password123",
            DateOfBirth = DateTime.Today.AddYears(-20),
            Role = Role.User
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.LastName)
            .WithErrorMessage("LastName must not exceed 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Empty()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "",
            Password = "password123",
            DateOfBirth = DateTime.Today.AddYears(-20),
            Role = Role.User
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email is required");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Is_Invalid_Format()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "invalid-email",
            Password = "password123",
            DateOfBirth = DateTime.Today.AddYears(-20),
            Role = Role.User
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Invalid email format");
    }

    [Fact]
    public void Should_Have_Error_When_Email_Exceeds_Maximum_Length()
    {
        // Arrange
        var longEmail = new string('a', 250) + "@test.com"; // 259 characters
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = longEmail,
            Password = "password123",
            DateOfBirth = DateTime.Today.AddYears(-20),
            Role = Role.User
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Email)
            .WithErrorMessage("Email must not exceed 254 characters");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Empty()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "",
            DateOfBirth = DateTime.Today.AddYears(-20),
            Role = Role.User
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password is required");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Is_Too_Short()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "12345",
            DateOfBirth = DateTime.Today.AddYears(-20),
            Role = Role.User
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must be at least 6 characters long");
    }

    [Fact]
    public void Should_Have_Error_When_Password_Exceeds_Maximum_Length()
    {
        // Arrange
        var longPassword = new string('a', 101); // 101 characters
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = longPassword,
            DateOfBirth = DateTime.Today.AddYears(-20),
            Role = Role.User
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.Password)
            .WithErrorMessage("Password must not exceed 100 characters");
    }

    [Fact]
    public void Should_Have_Error_When_DateOfBirth_Is_Too_Young()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "password123",
            DateOfBirth = DateTime.Today.AddYears(-12), // 12 years old
            Role = Role.User
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
            .WithErrorMessage("Must be at least 13 years old");
    }

    [Fact]
    public void Should_Have_Error_When_DateOfBirth_Is_In_Future()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "password123",
            DateOfBirth = DateTime.Today.AddDays(1), // Tomorrow
            Role = Role.User
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth)
            .WithErrorMessage("Date of birth cannot be in the future");
    }

    [Fact]
    public void Should_Not_Have_Error_When_Request_Is_Valid()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "password123",
            DateOfBirth = DateTime.Today.AddYears(-20),
            Role = Role.User
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Should_Allow_Valid_Age_At_Boundary()
    {
        // Arrange - exactly 13 years old
        var request = new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "password123",
            DateOfBirth = DateTime.Today.AddYears(-13),
            Role = Role.User
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldNotHaveValidationErrorFor(x => x.DateOfBirth);
    }

    [Fact]
    public void Should_Validate_Multiple_Errors()
    {
        // Arrange
        var request = new RegisterRequest
        {
            FirstName = "",
            LastName = "",
            Email = "invalid-email",
            Password = "123",
            DateOfBirth = DateTime.Today.AddDays(1),
            Role = Role.User
        };

        // Act & Assert
        var result = _validator.TestValidate(request);
        result.ShouldHaveValidationErrorFor(x => x.FirstName);
        result.ShouldHaveValidationErrorFor(x => x.LastName);
        result.ShouldHaveValidationErrorFor(x => x.Email);
        result.ShouldHaveValidationErrorFor(x => x.Password);
        result.ShouldHaveValidationErrorFor(x => x.DateOfBirth);
    }
}