using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Data.Configurations;

public class ProductDetailConfiguration : IEntityTypeConfiguration<ProductDetail>
{
    public void Configure(EntityTypeBuilder<ProductDetail> builder)
    {
        builder.HasMany(pd => pd.Media)
            .WithOne(m => m.ProductDetail)
            .HasForeignKey(m => m.ProductDetailId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(pd => pd.Documents)
            .WithOne(d => d.ProductDetail)
            .HasForeignKey(d => d.ProductDetailId)
            .OnDelete(DeleteBehavior.Cascade);

        // seed data
        builder.HasData(
            new ProductDetail
            {
                Id = 1,
                Title = "Sample Product Detail",
                Description = "This is a sample product detail",
                ProductId = 1,
                CreatedDate = DateTime.Now,
                CreatedBy = "John Doe",
                ModifiedDate = DateTime.Now,
                ModifiedBy = "John Doe"
            },
            new ProductDetail
            {
                Id = 2,
                Title = "Another Sample Product Detail",
                Description = "This is another sample product detail",
                ProductId = 2,
                CreatedDate = DateTime.Now,
                CreatedBy = "Jane Smith",
                ModifiedDate = DateTime.Now,
                ModifiedBy = "Jane Smith"
            },
            new ProductDetail
            {
                Id = 3,
                Title = "Yet Another Sample Product Detail",
                Description = "This is yet another sample product detail",
                ProductId = 3,
                CreatedDate = DateTime.Now,
                CreatedBy = "John Doe",
                ModifiedDate = DateTime.Now,
                ModifiedBy = "John Doe"
            }
        );
    }
}