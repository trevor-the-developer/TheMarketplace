using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Data.Configurations;

public class MediaConfiguration : IEntityTypeConfiguration<Media>
{
    public void Configure(EntityTypeBuilder<Media> builder)
    {
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Title).HasMaxLength(100).IsRequired();
        builder.Property(m => m.Description).HasMaxLength(500);
        builder.Property(m => m.FilePath).HasMaxLength(500).IsRequired();
        builder.Property(m => m.DirectoryPath).HasMaxLength(500).IsRequired();
        builder.Property(m => m.MediaType).HasMaxLength(100);
        builder.HasOne(m => m.ProductDetail)
            .WithMany(pd => pd.Media)
            .HasForeignKey(m => m.ProductDetailId)
            .OnDelete(DeleteBehavior.Restrict);

        // seed data
        builder.HasData(
            new Media
            {
                Id = 1,
                Title = "Sample Media",
                Description = "This is a sample media file",
                FilePath = "sample.mp4",
                DirectoryPath = "media",
                MediaType = "video",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            },
            new Media
            {
                Id = 2,
                Title = "Sample Media",
                Description = "This is a sample media file",
                FilePath = "sample.mp4",
                DirectoryPath = "media",
                MediaType = "video",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            },
            new Media
            {
                Id = 3,
                Title = "Sample Media",
                Description = "This is a sample media file",
                FilePath = "sample.mp4",
                DirectoryPath = "media",
                MediaType = "video",
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            }
        );
    }
}