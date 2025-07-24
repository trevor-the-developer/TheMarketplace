namespace Marketplace.Core.Models.Token;

public record TokenRefreshRequest(string AccessToken, string RefreshToken);