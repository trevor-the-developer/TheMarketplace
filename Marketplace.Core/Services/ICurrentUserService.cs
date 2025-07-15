namespace Marketplace.Core.Services;

public interface ICurrentUserService
{
    string? GetCurrentUserId();
    string? GetCurrentUserEmail();
    string GetCurrentUserName();
    bool IsAuthenticated();
}