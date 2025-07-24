namespace Marketplace.Core.Models.Registration;

public record ConfirmEmailResponse
{
    public string? Email { get; set; }
    public string? UserId { get; set; }
    public string? ConfirmationCode { get; set; }
    public bool RegistrationCompleted { get; set; } = false;
    public ApiError? ApiError { get; set; }
}