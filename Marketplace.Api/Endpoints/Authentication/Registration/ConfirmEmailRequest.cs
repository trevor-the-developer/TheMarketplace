namespace Marketplace.Api.Endpoints.Authentication.Registration;

public record ConfirmEmailRequest
{
    public string? Email { get; set; }
    public string? UserId { get; set; }
    public string? Token { get; set; }
}