# API Endpoints Structure

Endpoints are organised in domain-specific modules to ensure clean separation of concerns and maintainable code architecture. Each endpoint module implements the mediator pattern using WolverineFx for command/query processing.

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
Each endpoint processes requests through dedicated handler classes using the [Mediator Pattern](https://en.wikipedia.org/wiki/Mediator_pattern) via [WolverineFx](https://wolverine.netlify.app/tutorials/mediator.html).

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
- **POST /api/auth/refresh** - JWT token refresh using refresh tokens
- **POST /api/auth/revoke** - User logout and token revocation

### Listings Module
- **POST /api/listing/create** - Create new listing
- **PUT /api/listing/update/{id}** - Update existing listing
- **DELETE /api/listing/delete/{id}** - Delete listing
- **POST /api/listing/get** - Retrieve listings
- **POST /api/listing/get/{id}** - Get specific listing by ID
- **POST /api/listing/get/all** - Get all listings

### Cards Module
- **POST /api/card/create** - Create new card
- **PUT /api/card/update/{id}** - Update existing card
- **DELETE /api/card/delete/{id}** - Delete card
- **POST /api/card/get** - Retrieve cards
- **POST /api/card/get/{id}** - Get specific card by ID
- **POST /api/card/get/all** - Get all cards

### Products Module
- **POST /api/product/create** - Create new product
- **PUT /api/product/update/{id}** - Update existing product
- **DELETE /api/product/delete/{id}** - Delete product
- **POST /api/product/get** - Retrieve products
- **POST /api/product/get/{id}** - Get specific product by ID
- **POST /api/product/get/all** - Get all products

### Product Details Module
- **POST /api/productdetail/create** - Create new product detail
- **PUT /api/productdetail/update/{id}** - Update existing product detail
- **DELETE /api/productdetail/delete/{id}** - Delete product detail
- **POST /api/productdetail/get** - Retrieve product details
- **POST /api/productdetail/get/{id}** - Get specific product detail by ID
- **POST /api/productdetail/get/all** - Get all product details

### Media Module
- **POST /api/media/create** - Create new media
- **PUT /api/media/update/{id}** - Update existing media
- **DELETE /api/media/delete/{id}** - Delete media
- **POST /api/media/get** - Retrieve media
- **POST /api/media/get/{id}** - Get specific media by ID
- **POST /api/media/get/all** - Get all media

### Documents Module
- **POST /api/document/create** - Create new document
- **PUT /api/document/update/{id}** - Update existing document
- **DELETE /api/document/delete/{id}** - Delete document
- **POST /api/document/get** - Retrieve documents
- **POST /api/document/get/{id}** - Get specific document by ID
- **POST /api/document/get/all** - Get all documents

### Tags Module
- **POST /api/tag/create** - Create new tag
- **PUT /api/tag/update/{id}** - Update existing tag
- **DELETE /api/tag/delete/{id}** - Delete tag
- **POST /api/tag/get** - Retrieve tags
- **POST /api/tag/get/{id}** - Get specific tag by ID
- **POST /api/tag/get/all** - Get all tags

### User Profiles Module
- **POST /api/userprofile/create** - Create new user profile
- **PUT /api/userprofile/update/{id}** - Update existing user profile
- **DELETE /api/userprofile/delete/{id}** - Delete user profile
- **POST /api/userprofile/get** - Retrieve user profiles
- **POST /api/userprofile/get/{id}** - Get specific user profile by ID
- **POST /api/userprofile/get/all** - Get all user profiles

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
