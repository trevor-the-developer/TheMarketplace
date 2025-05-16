using Marketplace.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Marketplace.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Title).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.ProductType).HasMaxLength(50);
        builder.Property(p => p.Category).HasMaxLength(50);
        builder.Property(p => p.IsEnabled);
        builder.Property(p => p.IsDeleted);
        builder.HasOne(p => p.Card)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CardId);
        builder.HasOne(p => p.ProductDetail)
            .WithOne(pd => pd.Product)
            .HasForeignKey<ProductDetail>(pd => pd.ProductId);
        
        // seed data
        builder.HasData(
            new Product()
            {
                Id = 1,
                Title = "Sample Product",
                Description = "This is a sample product",
                ProductType = "Sample Type",
                Category = "Sample Category",
                IsEnabled = true,
                IsDeleted = false,
                CardId = 1,
                ProductDetailId = 21,
                CreatedDate = DateTime.Now,
                CreatedBy = "Sample User",
                ModifiedDate = DateTime.Now,
                ModifiedBy = "Sample User"
            },
            new Product()
            {
                Id = 2,
                Title = "Another Sample Product",
                Description = "This is another sample product",
                ProductType = "Another Sample Type",
                Category = "Another Sample Category",
                IsEnabled = true,
                IsDeleted = false,
                CardId = 1,
                ProductDetailId = 22,
                CreatedDate = DateTime.Now,
                CreatedBy = "Another Sample User",
                ModifiedDate = DateTime.Now,
                ModifiedBy = "Another Sample User"
            },
            new Product()
            {
                Id = 3,
                Title = "Third Sample Product",
                Description = "This is the third sample product",
                ProductType = "Third Sample Type",
                Category = "Third Sample Category",
                IsEnabled = true,
                IsDeleted = false,
                CardId = 1,
                ProductDetailId = 23,
                CreatedDate = DateTime.Now,
                CreatedBy = "Third Sample User",
                ModifiedDate = DateTime.Now,
                ModifiedBy = "Third Sample User"
            }
        );
    }
}