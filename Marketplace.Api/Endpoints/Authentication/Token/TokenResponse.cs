﻿using Marketplace.Core;

namespace Marketplace.Api.Endpoints.Authentication.Token;

public record TokenResponse
{
    public bool? Succeeded { get; set; }
    public string? JwtToken { get; set; }
    public DateTime? Expiration { get; set; }
    public string? RefreshToken { get; set; }
    public ApiError? ApiError { get; set; }
}