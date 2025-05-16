using Marketplace.Core;
using Microsoft.AspNetCore.Identity;

namespace Marketplace.Api.Endpoints.Authentication.Registration;

public record RegisterStepTwoResponse
{
    public string? UserId { get; set; }
    public bool? RegistrationStepTwo { get; set; }
    public string? ConfirmationCode { get; set; }
    public ApiError? ApiError { get; set; }
    public IEnumerable<IdentityError>? Errors { get; set; }
}