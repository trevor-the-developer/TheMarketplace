# Entity Type Configurations

Configuration classes provide a clean separation of concerns by isolating entity configuration logic from the main DbContext class. This pattern promotes maintainable, testable code that follows SOLID principles.

## Current Status
✅ **All configurations tested** - Entity mappings are validated through automated tests.
✅ **Seed data working** - Initial roles and users are properly seeded in the database.
✅ **Relationships validated** - All foreign key relationships are correctly configured.

## Benefits of Configuration Pattern

- **Separation of Concerns**: Each entity's configuration is isolated in its own class
- **Maintainability**: Changes to entity configuration don't affect the DbContext
- **Testability**: Individual configurations can be tested in isolation
- **Readability**: DbContext remains clean and focused on core responsibilities
- **Scalability**: Easy to add new entity configurations without cluttering existing code

## Configuration Structure

Each configuration class implements `IEntityTypeConfiguration<TEntity>` and follows this pattern:

```csharp
public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
{
    public void Configure(EntityTypeBuilder<IdentityRole> builder)
    {
        // Entity configuration
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Name).IsRequired().HasMaxLength(256);
        
        // Seed data
        builder.HasData(
            new IdentityRole
            {
                Id = "00917cdb-f5b0-4c84-9172-ff5b72ff8500",
                Name = "Administrator",
                NormalizedName = "ADMINISTRATOR"
            },
            new IdentityRole
            {
                Id = "e23ba8c8-b3ae-4e81-b468-c269c6e35cf2",
                Name = "User",
                NormalizedName = "USER"
            }
        );
    }
}
```

## Configuration Types

### Entity Mapping
- **Property Configuration**: Data types, lengths, required fields
- **Relationships**: Foreign keys, navigation properties
- **Indexes**: Performance optimisation for queries
- **Constraints**: Unique constraints, check constraints

### Seed Data
- **Initial Data**: Default roles, settings, lookup values
- **Test Data**: Development and testing scenarios
- **Reference Data**: Static data required for application functionality

## Usage in DbContext

```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    // Apply all configurations from assembly
    builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    
    base.OnModelCreating(builder);
}
```

## Best Practices

1. **One Configuration Per Entity**: Each entity should have its own configuration class
2. **Naming Convention**: Use `{EntityName}Configuration` naming pattern
3. **Grouping**: Place related configurations in the same namespace/folder
4. **Documentation**: Comment complex configurations for future maintainers
5. **Validation**: Include appropriate validation rules and constraints
