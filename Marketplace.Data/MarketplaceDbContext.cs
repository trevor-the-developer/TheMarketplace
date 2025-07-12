using Marketplace.Data.Configurations;
using Marketplace.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Marketplace.Data
{
    public class MarketplaceDbContext(DbContextOptions<MarketplaceDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            // Note: PendingModelChangesWarning is only available in .NET 9.0+
            // optionsBuilder.ConfigureWarnings(warnings => 
            //     warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modalBuilder)
        {
            base.OnModelCreating(modalBuilder);
            
            // Configurations pattern: apply your custom configuration here
            // builder.ApplyConfiguration(new YourConfiguration());
            modalBuilder.ApplyConfiguration(new RoleConfiguration());
            modalBuilder.ApplyConfiguration(new UserConfiguration());
            modalBuilder.ApplyConfiguration(new UserRoleConfiguration());
            // modalBuilder.ApplyConfiguration(new UserProfileConfiguration());
            
            // entity configurations
            modalBuilder.ApplyConfiguration(new MediaConfiguration());
            modalBuilder.ApplyConfiguration(new DocumentConfiguration());
            modalBuilder.ApplyConfiguration(new ProductDetailConfiguration());
            modalBuilder.ApplyConfiguration(new ProductConfiguration());
            modalBuilder.ApplyConfiguration(new CardConfiguration());
            modalBuilder.ApplyConfiguration(new ListingConfiguration());
            modalBuilder.ApplyConfiguration(new TagConfiguration());
        }

        public DbSet<Listing> Listings { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductDetail> ProductDetails { get; set; }
        public DbSet<Media> Files { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<UserProfile> Profiles { get; set; }

    }
}
