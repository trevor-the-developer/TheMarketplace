namespace Marketplace.Api.Endpoints.Authentication.Registration;

public record RegisterStepTwoRequest
{
    public required string UserId { get; set; }
    public required string Email { get; set; }
    public required string Token { get; set; }
}