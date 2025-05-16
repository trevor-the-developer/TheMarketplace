using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Data.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.Property(d => d.Title).HasMaxLength(100).IsRequired();
        builder.Property(d => d.Description).HasMaxLength(500);
        builder.Property(d => d.Text);
        builder.Property(d => d.Bytes);
        builder.Property(d => d.DocumentType).HasMaxLength(50).IsRequired();
        builder.HasOne(d => d.ProductDetail)
            .WithMany(p => p.Documents)
            .HasForeignKey(d => d.ProductDetailId);
    }
}