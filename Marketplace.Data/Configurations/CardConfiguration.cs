using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Data.Configurations;

public class CardConfiguration : IEntityTypeConfiguration<Card>
{
    public void Configure(EntityTypeBuilder<Card> builder)
    {
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Title).HasMaxLength(100).IsRequired();
        builder.Property(c => c.Description).HasMaxLength(500);
        builder.Property(c => c.IsEnabled);
        builder.Property(c => c.Colour).HasMaxLength(20);
        builder.HasOne(c => c.Listing)
            .WithMany(l => l.Cards)
            .HasForeignKey(c => c.ListingId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(c => c.Products)
            .WithOne(p => p.Card)
            .HasForeignKey(p => p.CardId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.ToTable("Cards");

        // seed data
        builder.HasData(
            new List<Card>
            {
                new()
                {
                    Id = 1,
                    Title = "Sample Card",
                    Description = "This is a sample card",
                    IsEnabled = true,
                    Colour = "Blue",
                    ListingId = 1
                },
                new()
                {
                    Id = 2,
                    Title = "Another Card",
                    Description = "This is another card",
                    IsEnabled = false,
                    Colour = "Red",
                    ListingId = 1
                },
                new()
                {
                    Id = 3,
                    Title = "Third Card",
                    Description = "This is the third card",
                    IsEnabled = true,
                    Colour = "Green",
                    ListingId = 1
                }
            }
        );
    }
}