using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Data.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "00917cdb-f5b0-4c84-9172-ff5b72ff8500",
                    UserId = "a5ac5ebb-5f11-4363-a58d-4362d8ff6863",
                },
                new IdentityUserRole<string>
                {
                    RoleId = "e23ba8c8-b3ae-4e81-b468-c269c6e35cf2",
                    UserId = "69a38a69-e24d-4c7f-bdf2-c7bc2222cbe7",
                }
            );
        }
    }
}
