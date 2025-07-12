# Data Layer - Marketplace.Data

.NET 8 Entity Framework Core data layer with comprehensive entity modeling and automated database management.

## Data Structure Hierarchy

```
* n Listing's
	- n Card's
		- n Products
			- Product Details
				- Media
				- Documents
```

## Entity Descriptions

- **Listing** - Top-level collections that organise trading items by category (e.g., vehicles, electronics, shoes). Supports tag-based search functionality.
- **Card** - Visual containers within listings that group related products for better organisation and presentation.
- **Product** - Individual items or services available for sale/trade/swap/give away with detailed specifications.
- **Product Details** - Extended information including descriptions, specifications, and metadata used for listings and search.
- **Media & Documents** - File attachments associated with products (images, manuals, PDFs, marketing materials).

## Database Features

### Entity Framework Core 8
- **Code First Approach**: Database schema generated from entity models
- **Migrations**: Automated database schema versioning and updates
- **Identity Integration**: ASP.NET Core Identity for user management
- **Audit Fields**: Base entity with created/modified timestamps

### Database Management
- **Automated Setup**: Database creation and migration application during test runs
- **Design-Time Factory**: Proper EF Core tooling support for .NET 8
- **Connection Management**: Scoped DbContext lifetime for proper dependency injection

### Migration Commands
```bash
# Add new migration
dotnet ef migrations add MigrationName --project Marketplace.Data --startup-project Marketplace.Api

# Update database
dotnet ef database update --project Marketplace.Data --startup-project Marketplace.Api

# Remove last migration
dotnet ef migrations remove --project Marketplace.Data --startup-project Marketplace.Api
```
