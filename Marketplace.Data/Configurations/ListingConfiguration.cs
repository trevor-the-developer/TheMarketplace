using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Data.Configurations;

public class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
    public void Configure(EntityTypeBuilder<Listing> builder)
    {
        builder.HasMany(l => l.Cards)
            .WithOne(c => c.Listing)
            .HasForeignKey(c => c.ListingId)
            .OnDelete(DeleteBehavior.Cascade);

        // seed data: sample data
        builder.HasData(
            new List<Listing>
            {
                new()
                {
                    Id = 1,
                    Title = "Sample Listing",
                    Cards = new List<Card>(),
                    CreatedDate = DateTime.Now,
                    CreatedBy = "John Doe",
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = "John Doe"
                }
            }
        );
    }
}