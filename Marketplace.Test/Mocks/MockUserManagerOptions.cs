using Microsoft.AspNetCore.Identity;

namespace Marketplace.Test.Mocks;

/// <summary>
/// Options for configuring MockUserManager behavior
/// </summary>
public class MockUserManagerOptions
{
    /// <summary>
    /// Whether FindByEmailAsync should return null (user doesn't exist)
    /// </summary>
    public bool UserNotFound { get; init; } = false;

    /// <summary>
    /// Whether CreateAsync should fail
    /// </summary>
    public bool CreateAsyncFailed { get; init; } = false;

    /// <summary>
    /// Whether CheckPasswordAsync should return false (invalid password)
    /// </summary>
    public bool InvalidPassword { get; init; } = false;

    /// <summary>
    /// Whether UpdateAsync should fail
    /// </summary>
    public bool UpdateAsyncFailed { get; init; } = false;

    /// <summary>
    /// Whether DeleteAsync should fail
    /// </summary>
    public bool DeleteAsyncFailed { get; init; } = false;

    /// <summary>
    /// Whether AddToRoleAsync should fail
    /// </summary>
    public bool AddToRoleAsyncFailed { get; init; } = false;

    /// <summary>
    /// Whether GenerateEmailConfirmationTokenAsync should fail
    /// </summary>
    public bool GenerateEmailConfirmationTokenAsyncFailed { get; init; } = false;

    /// <summary>
    /// Whether ConfirmEmailAsync should fail
    /// </summary>
    public bool ConfirmEmailAsyncFailed { get; init; } = false;

    /// <summary>
    /// Custom errors to return when operations fail
    /// </summary>
    public IEnumerable<IdentityError>? CustomErrors { get; init; }

    /// <summary>
    /// Token to return from GenerateEmailConfirmationTokenAsync
    /// </summary>
    public string? EmailConfirmationToken { get; init; } = "test-confirmation-token";
}
