using System.ComponentModel.DataAnnotations;
using Marketplace.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Data.Entities
{
    /// <summary>
    /// Represents an application user (after sign-in success).
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public int ApplicationUserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Role Role { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }

        public new bool? EmailConfirmed { get; set; } = false;
        // navigation properties
        public UserProfile? UserProfile { get; set; }
    }
}
