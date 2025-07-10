# Repository Pattern Implementation

Repository classes provide data access abstraction and align with API endpoints and business operations. The repository pattern works in conjunction with the WolverineFx mediator pattern to provide clean separation between data access and business logic.

## Architecture Principles

- **Single Responsibility**: Each repository handles operations for a specific domain entity
- **Mediator Integration**: Repositories work seamlessly with WolverineFx message handlers
- **Generic Base**: Common operations are handled by the base repository class
- **Testability**: Repositories can be easily mocked for unit testing

## Repository Classes

### Core Repositories
- **GenericRepository** - Base class providing common CRUD operations for all entities
- **ListingRepository** - Manages listing-related data operations and queries
- **CardRepository** - Handles card-specific data operations and filtering
- **ProductRepository** - Manages product data operations and search functionality
- **ProductDetailRepository** - Handles detailed product information and metadata
- **DocumentRepository** - Manages document attachments and file operations
- **MediaRepository** - Handles media files and image operations

### Authentication & Authorisation
- **AuthenticationRepository** - Manages user authentication, registration, and token operations

## Usage Pattern

```csharp
// Repository is injected into handlers via DI
public class GetListingHandler
{
    private readonly IListingRepository _listingRepository;
    
    public GetListingHandler(IListingRepository listingRepository)
    {
        _listingRepository = listingRepository;
    }
    
    public async Task<ListingDto> Handle(GetListingQuery query)
    {
        return await _listingRepository.GetByIdAsync(query.Id);
    }
}
```

## Benefits

- **Separation of Concerns**: Data access logic is isolated from business logic
- **Consistency**: Common operations are standardised across all entities
- **Maintainability**: Changes to data access patterns can be made in one place
- **Testing**: Easy to mock repositories for unit testing handlers
