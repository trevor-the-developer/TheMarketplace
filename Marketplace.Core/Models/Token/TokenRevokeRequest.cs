namespace Marketplace.Core.Models.Token;

public record TokenRevokeRequest(string AccessToken, string RefreshToken);