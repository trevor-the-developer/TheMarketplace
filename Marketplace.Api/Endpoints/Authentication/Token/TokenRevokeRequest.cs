namespace Marketplace.Api.Endpoints.Authentication.Token;

public record TokenRevokeRequest(string AccessToken, string RefreshToken);