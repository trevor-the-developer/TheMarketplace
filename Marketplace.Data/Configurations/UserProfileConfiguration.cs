using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Data.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasData(
            new UserProfile
            {
                Id = 1,
                ApplicationUserId = "a5ac5ebb-5f11-4363-a58d-4362d8ff6863",
                DisplayName = "System Administrator",
                Bio = "System administrator account for marketplace",
                SocialMedia = "https://example.com/admin",
                CreatedDate = DateTime.Now,
                CreatedBy = "System",
                ModifiedDate = DateTime.Now,
                ModifiedBy = "System"
            },
            new UserProfile
            {
                Id = 2,
                ApplicationUserId = "69a38a69-e24d-4c7f-bdf2-c7bc2222cbe7",
                DisplayName = "Demo User",
                Bio = "Demo user account for testing",
                SocialMedia = "https://example.com/demo",
                CreatedDate = DateTime.Now,
                CreatedBy = "System",
                ModifiedDate = DateTime.Now,
                ModifiedBy = "System"
            }
        );
    }
}