using System;
using Marketplace.Data.Enums;

namespace Marketplace.Core.Models.Registration;

public record RegisterRequest
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Role Role { get; set; } = Role.User;
}