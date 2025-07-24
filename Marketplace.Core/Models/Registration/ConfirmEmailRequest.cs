namespace Marketplace.Core.Models.Registration;

public record ConfirmEmailRequest
{
    public string? Email { get; set; }
    public string? UserId { get; set; }
    public string? Token { get; set; }
}