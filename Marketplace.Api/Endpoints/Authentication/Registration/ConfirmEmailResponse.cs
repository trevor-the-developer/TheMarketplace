using Marketplace.Core;

namespace Marketplace.Api.Endpoints.Authentication.Registration;

public record ConfirmEmailResponse
{
    public string? Email { get; set; }
    public string? UserId { get; set; }
    public string? ConfirmationCode { get; set; }
    public bool RegistrationCompleted { get; set; } = false;
    public ApiError? ApiError { get; set; }
}