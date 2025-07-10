using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Marketplace.Core.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue("userId");
    }

    public string? GetCurrentUserEmail()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    }

    public string GetCurrentUserName()
    {
        var email = GetCurrentUserEmail();
        var userId = GetCurrentUserId();
        
        // Return email if available, otherwise userId, otherwise "System"
        return email ?? userId ?? "System";
    }

    public bool IsAuthenticated()
    {
        return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
