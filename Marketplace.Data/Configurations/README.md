# Configurations
## Configurations pattern allows for better seperation of concerns and allows for Lean Code practices i.e. code that does not cause anxiety.
### Configuration
A configuration file will consist of the followiing pattern:
```
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
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
The configuration class allows configuration for an entity to be factored into a seperate class rather than cluttering the DbContext class
(and lends towards a better testing strategy).

**IEntityTypeConiguration<TEntityType>** - Allows configuration for an entity type to be factored into a separate class.
