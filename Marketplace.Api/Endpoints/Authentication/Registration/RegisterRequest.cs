using Marketplace.Data.Enums;

namespace Marketplace.Api.Endpoints.Authentication.Registration
{
    public record RegisterRequest
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime DateOfBirth { get; set; }
        public Role Role { get; set; } = Role.User;
    }
}
