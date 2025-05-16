namespace Marketplace.Api.Endpoints.Authentication.Token
{
    public record TokenRefreshRequest(string AccessToken, string RefreshToken);
}
