using Marketplace.Data.Entities;
using Marketplace.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.Role).HasConversion<string>().IsRequired();
        builder.HasOne(u => u.UserProfile)
            .WithOne(up => up.ApplicationUser)
            .HasForeignKey<UserProfile>(up => up.ApplicationUserId)
            .IsRequired(false);
        builder.Property(u => u.RefreshTokenExpiry).IsRequired();
        builder.Property(u => u.ApplicationUserId).IsRequired();
        builder.Property(u => u.DateOfBirth).IsRequired(false);
        builder.Property(u => u.RefreshToken).IsRequired(false);

        var hasher = new PasswordHasher<ApplicationUser>();

        builder.HasData(
            new ApplicationUser
            {
                Id = "a5ac5ebb-5f11-4363-a58d-4362d8ff6863",
                ApplicationUserId = 1,
                Email = "admin@localhost",
                NormalizedEmail = "ADMIN@LOCALHOST",
                NormalizedUserName = "ADMIN@LOCALHOST",
                UserName = "admin@localhost",
                FirstName = "System",
                LastName = "Administrator",
                PasswordHash = hasher.HashPassword(new ApplicationUser { UserName = "Admin" }, "P@ssw0rd!"),
                EmailConfirmed = true,
                Role = Role.Adminstrator,
                RefreshTokenExpiry = new DateTime(2025, 12, 31, 0, 0, 0, DateTimeKind.Utc)
            },
            new ApplicationUser
            {
                Id = "69a38a69-e24d-4c7f-bdf2-c7bc2222cbe7",
                ApplicationUserId = 2,
                Email = "demouser@localhost",
                NormalizedEmail = "DEMOUSER@LOCALHOST",
                NormalizedUserName = "DEMOUSER@LOCALHOST",
                UserName = "demouser@localhost",
                FirstName = "Demo",
                LastName = "User",
                PasswordHash = hasher.HashPassword(new ApplicationUser { UserName = "DemoUser1" }, "P@ssword1"),
                EmailConfirmed = true,
                Role = Role.User,
                RefreshTokenExpiry = new DateTime(2025, 12, 31, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}