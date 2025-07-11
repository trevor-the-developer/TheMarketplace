using Marketplace.Core.Services;

namespace Marketplace.Test.Mocks;

public class MockCurrentUserService : ICurrentUserService
{
    private readonly string _email;
    private readonly bool _isAuthenticated;
    private readonly string _userId;

    public MockCurrentUserService(string userId = "test-user-id", string email = "test@example.com",
        bool isAuthenticated = true)
    {
        _userId = userId;
        _email = email;
        _isAuthenticated = isAuthenticated;
    }

    public string? GetCurrentUserId()
    {
        return _userId;
    }

    public string? GetCurrentUserEmail()
    {
        return _email;
    }

    public string GetCurrentUserName()
    {
        return _email ?? _userId ?? "TestUser";
    }

    public bool IsAuthenticated()
    {
        return _isAuthenticated;
    }
}