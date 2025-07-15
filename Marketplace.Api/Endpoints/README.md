# API Endpoints Structure

Endpoints are organised in domain-specific modules to ensure clean separation of concerns and maintainable code
architecture. Each endpoint module implements the mediator pattern using WolverineFx for command/query processing.

## Current Status

✅ **All endpoints tested and functional** - Complete CRUD operations implemented and verified.
✅ **Authentication working** - JWT-based authentication with refresh tokens fully operational.
✅ **Swagger documentation** - All endpoints documented with proper request/response models.

## Organisation Pattern

```
Endpoints/
├── Authentication/
│   ├── LoginEndpoint.cs
│   ├── RegisterEndpoint.cs
│   ├── RefreshTokenEndpoint.cs
│   └── Handlers/
│       ├── LoginHandler.cs
│       ├── RegisterHandler.cs
│       └── RefreshTokenHandler.cs
├── Listings/
│   ├── GetListingsEndpoint.cs
│   └── Handlers/
└── Cards/
    ├── GetCardsEndpoint.cs
    └── Handlers/
```

## Mediator Pattern Implementation

### Command/Query Processing

Each endpoint processes requests through dedicated handler classes using
the [Mediator Pattern](https://en.wikipedia.org/wiki/Mediator_pattern)
via [WolverineFx](https://wolverine.netlify.app/tutorials/mediator.html).

### Handler Discovery

Wolverine provides automatic handler discovery that:

- Finds candidate message handler methods at startup
- Correlates handlers by message type (Command/Query)
- Builds connective code at runtime for message processing
- Routes messages from `IMessageBus` to appropriate handlers

## Current Endpoints

### Authentication Module

- **POST /api/auth/login** - User authentication with JWT token generation
- **POST /api/auth/register** - User registration with email confirmation
- **GET /api/auth/confirm-email** - Email confirmation and account activation
- **POST /api/auth/refresh** - JWT token refresh using refresh tokens
- **POST /api/auth/logout** - User logout and token revocation

### Listings Module

- **POST /api/listings** - Create new listing
- **PUT /api/listings/{id}** - Update existing listing
- **DELETE /api/listings/{id}** - Delete listing
- **GET /api/listings** - Get all listings
- **GET /api/listings/{id}** - Get specific listing by ID

### Cards Module

- **POST /api/cards** - Create new card
- **PUT /api/cards/{id}** - Update existing card
- **DELETE /api/cards/{id}** - Delete card
- **GET /api/cards** - Get all cards
- **GET /api/cards/{id}** - Get specific card by ID

### Products Module

- **POST /api/products** - Create new product
- **PUT /api/products/{id}** - Update existing product
- **DELETE /api/products/{id}** - Delete product
- **GET /api/products** - Get all products
- **GET /api/products/{id}** - Get specific product by ID

### Product Details Module

- **POST /api/product-details** - Create new product detail
- **PUT /api/product-details/{id}** - Update existing product detail
- **DELETE /api/product-details/{id}** - Delete product detail
- **GET /api/product-details** - Get all product details
- **GET /api/product-details/{id}** - Get specific product detail by ID

### Media Module

- **POST /api/media** - Create new media
- **PUT /api/media/{id}** - Update existing media
- **DELETE /api/media/{id}** - Delete media
- **GET /api/media** - Get all media
- **GET /api/media/{id}** - Get specific media by ID

### Documents Module

- **POST /api/documents** - Create new document
- **PUT /api/documents/{id}** - Update existing document
- **DELETE /api/documents/{id}** - Delete document
- **GET /api/documents** - Get all documents
- **GET /api/documents/{id}** - Get specific document by ID

### Tags Module

- **POST /api/tags** - Create new tag
- **PUT /api/tags/{id}** - Update existing tag
- **DELETE /api/tags/{id}** - Delete tag
- **GET /api/tags** - Get all tags
- **GET /api/tags/{id}** - Get specific tag by ID

### User Profiles Module

- **POST /api/user-profiles** - Create new user profile
- **PUT /api/user-profiles/{id}** - Update existing user profile
- **DELETE /api/user-profiles/{id}** - Delete user profile
- **GET /api/user-profiles** - Get all user profiles
- **GET /api/user-profiles/{id}** - Get specific user profile by ID

## Handler Pattern Example

```csharp
// Request/Command
public record LoginRequest(string Email, string Password);

// Handler
public class LoginHandler
{
    private readonly IAuthenticationRepository _authRepository;
    private readonly ITokenService _tokenService;
    
    public LoginHandler(IAuthenticationRepository authRepository, ITokenService tokenService)
    {
        _authRepository = authRepository;
        _tokenService = tokenService;
    }
    
    public async Task<LoginResponse> Handle(LoginRequest request)
    {
        // Authentication logic here
        var user = await _authRepository.ValidateUserAsync(request.Email, request.Password);
        var token = await _tokenService.GenerateTokenAsync(user);
        
        return new LoginResponse(token.AccessToken, token.RefreshToken);
    }
}

// Endpoint
app.MapPost("/api/auth/login", async (LoginRequest request, IMessageBus bus) =>
{
    return await bus.InvokeAsync<LoginResponse>(request);
});
```

## Benefits of This Architecture

- **Separation of Concerns**: Endpoints handle HTTP concerns, handlers handle business logic
- **Testability**: Handlers can be unit tested independently of HTTP infrastructure
- **Maintainability**: Changes to business logic don't affect endpoint definitions
- **Scalability**: Easy to add new endpoints and handlers without affecting existing code
- **Consistency**: All endpoints follow the same pattern for predictable development
