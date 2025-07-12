using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Data.Configurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).HasMaxLength(100).IsRequired();
        builder.Property(t => t.Description).HasMaxLength(500);
        builder.Property(t => t.IsEnabled);
        
        // Self-referencing relationship for parent tags
        builder.HasMany(t => t.Tags)
            .WithOne()
            .HasForeignKey("TagId")
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.ToTable("Tags");
        
        // seed data
        builder.HasData(
            new Tag()
            {
                Id = 1,
                Name = "Technology",
                Description = "Technology-related items",
                IsEnabled = true,
                CreatedDate = DateTime.Now,
                CreatedBy = "System",
                ModifiedDate = DateTime.Now,
                ModifiedBy = "System"
            },
            new Tag()
            {
                Id = 2,
                Name = "Gaming",
                Description = "Gaming and entertainment items",
                IsEnabled = true,
                CreatedDate = DateTime.Now,
                CreatedBy = "System",
                ModifiedDate = DateTime.Now,
                ModifiedBy = "System"
            },
            new Tag()
            {
                Id = 3,
                Name = "Education",
                Description = "Educational content and materials",
                IsEnabled = true,
                CreatedDate = DateTime.Now,
                CreatedBy = "System",
                ModifiedDate = DateTime.Now,
                ModifiedBy = "System"
            }
        );
    }
}
