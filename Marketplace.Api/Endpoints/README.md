# API Endpoints Structure

Endpoints are organised in domain-specific modules to ensure clean separation of concerns and maintainable code architecture. Each endpoint module implements the mediator pattern using WolverineFx for command/query processing.

## Organization Pattern

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
- **POST /api/auth/logout** - User logout and token revocation

### Listings Module
- **GET /api/listings** - Retrieve all listings with pagination
- **GET /api/listings/{id}** - Get specific listing by ID
- **GET /api/listings/user/{userId}** - Get user-specific listings

### Cards Module
- **GET /api/cards** - Retrieve all cards with filtering
- **GET /api/cards/{id}** - Get specific card by ID

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
