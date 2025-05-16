using Marketplace.Core;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace Marketplace.Api.Endpoints.Authentication.Login
{
    public record LoginResponse
    {
        public bool? Succeeded { get; set; }
        public string? Id { get; set; }
        public string? Email { get; set; }
        public string? SecurityToken { get; set; }
        public DateTime? Expiration { get; set; }
        public string? RefreshToken { get; set; }
        public ApiError? ApiError { get; set; }
        public IEnumerable<IdentityError>? Errors { get; set; }
    }
}
