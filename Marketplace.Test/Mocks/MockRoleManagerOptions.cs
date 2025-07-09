using Microsoft.AspNetCore.Identity;

namespace Marketplace.Test.Mocks;

/// <summary>
/// Options for configuring MockRoleManager behavior
/// </summary>
public class MockRoleManagerOptions
{
    /// <summary>
    /// Whether CreateAsync should fail
    /// </summary>
    public bool CreateAsyncFailed { get; init; } = false;

    /// <summary>
    /// Whether UpdateAsync should fail
    /// </summary>
    public bool UpdateAsyncFailed { get; init; } = false;

    /// <summary>
    /// Whether DeleteAsync should fail
    /// </summary>
    public bool DeleteAsyncFailed { get; init; } = false;

    /// <summary>
    /// Whether RoleExistsAsync should return false
    /// </summary>
    public bool RoleDoesNotExist { get; init; } = false;

    /// <summary>
    /// Custom errors to return when operations fail
    /// </summary>
    public IEnumerable<IdentityError>? CustomErrors { get; init; }
}
