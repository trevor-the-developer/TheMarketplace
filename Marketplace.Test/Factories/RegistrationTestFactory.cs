using Marketplace.Api.Endpoints.Authentication.Registration;
using Marketplace.Data.Entities;
using Microsoft.AspNetCore.Identity;
using RegisterStepOneResponse = Marketplace.Api.Endpoints.Authentication.Registration.RegisterStepOneResponse;

namespace Marketplace.Test.Factories;

/// <summary>
/// Factory for creating registration-related test objects
/// </summary>
public static class RegistrationTestFactory
{
    /// <summary>
    /// Creates a valid RegisterRequest for testing
    /// </summary>
    public static RegisterRequest CreateValidRegisterRequest(string? email = null)
    {
        return new RegisterRequest
        {
            FirstName = "Test",
            LastName = "User",
            Email = email ?? "test@example.com",
            Password = "TestPassword123!",
            DateOfBirth = DateTime.Now.AddYears(-25)
        };
    }

    /// <summary>
    /// Creates a RegisterRequest with missing required fields
    /// </summary>
    public static RegisterRequest CreateInvalidRegisterRequest()
    {
        return new RegisterRequest
        {
            FirstName = "",
            LastName = "",
            Email = "",
            Password = "",
            DateOfBirth = default(DateTime)
        };
    }

    /// <summary>
    /// Creates a test ApplicationUser for registration scenarios
    /// </summary>
    public static ApplicationUser CreateTestUser(string? email = null, bool emailConfirmed = false)
    {
        return new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = email ?? "test@example.com",
            UserName = email ?? "test@example.com",
            FirstName = "Test",
            LastName = "User",
            EmailConfirmed = emailConfirmed,
            DateOfBirth = DateTime.Now.AddYears(-25)
        };
    }

    /// <summary>
    /// Creates a ConfirmEmailRequest for testing
    /// </summary>
    public static ConfirmEmailRequest CreateConfirmEmailRequest(string? userId = null, string? token = null, string? email = null)
    {
        return new ConfirmEmailRequest
        {
            UserId = userId ?? Guid.NewGuid().ToString(),
            Token = token ?? "test-token",
            Email = email ?? "test@example.com"
        };
    }

    /// <summary>
    /// Creates a RegisterStepTwoRequest for testing
    /// </summary>
    public static RegisterStepTwoRequest CreateRegisterStepTwoRequest(string? userId = null, string? token = null, string? email = null)
    {
        return new RegisterStepTwoRequest
        {
            UserId = userId ?? Guid.NewGuid().ToString(),
            Token = token ?? "test-token",
            Email = email ?? "test@example.com"
        };
    }

    /// <summary>
    /// Creates a successful RegisterStepOneResponse
    /// </summary>
    public static RegisterStepOneResponse CreateSuccessfulRegistrationResponse(string? userId = null)
    {
        return new RegisterStepOneResponse
        {
            UserId = userId ?? Guid.NewGuid().ToString(),
            RegistrationStepOne = true,
            ConfirmationEmailLink = "https://example.com/confirm-email",
            ApiError = null,
            Errors = Enumerable.Empty<IdentityError>()
        };
    }

    /// <summary>
    /// Creates a failed RegisterStepOneResponse with errors
    /// </summary>
    public static RegisterStepOneResponse CreateFailedRegistrationResponse(IEnumerable<IdentityError>? errors = null)
    {
        return new RegisterStepOneResponse
        {
            UserId = null,
            RegistrationStepOne = false,
            ConfirmationEmailLink = null,
            ApiError = null,
            Errors = errors ?? new List<IdentityError>
            {
                new() { Code = "PasswordTooShort", Description = "Password is too short" }
            }
        };
    }

    /// <summary>
    /// Creates a test IdentityRole
    /// </summary>
    public static IdentityRole CreateTestRole(string? name = null)
    {
        return new IdentityRole
        {
            Id = Guid.NewGuid().ToString(),
            Name = name ?? "User",
            NormalizedName = (name ?? "User").ToUpper()
        };
    }
}
