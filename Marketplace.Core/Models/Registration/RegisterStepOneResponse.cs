using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Marketplace.Core.Models.Registration;

public record RegisterStepOneResponse
{
    public string? UserId { get; set; }
    public bool? RegistrationStepOne { get; set; }
    public string? ConfirmationEmailLink { get; set; }
    public ApiError? ApiError { get; set; }
    public IEnumerable<IdentityError>? Errors { get; set; }
}