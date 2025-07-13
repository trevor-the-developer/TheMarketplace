# Marketplace Core Library

Business logic and shared utilities for TheMarketplace application, implementing clean architecture principles and domain-driven design patterns.

## Current Status
✅ **Core library stable** - All business logic components are tested and functional.
✅ **Clean architecture** - Proper separation of concerns with domain entities and services.
✅ **Validation framework** - Comprehensive input validation using FluentValidation.
✅ **Security components** - JWT token management and authentication services.

## Architecture Overview

### Domain-Driven Design
The core library implements DDD principles with:
- **Entities**: Domain objects with identity and lifecycle
- **Value Objects**: Immutable objects representing concepts
- **Domain Services**: Business logic that doesn't naturally fit in entities
- **Repositories**: Abstractions for data access

### Clean Architecture Layers
```
Marketplace.Core/
├── Entities/           # Domain entities and aggregates
├── Interfaces/         # Repository and service contracts
├── Services/           # Business logic services
├── DTOs/              # Data transfer objects
├── Validators/         # Input validation rules
└── Utilities/          # Shared helper classes
```

## Core Components

### Authentication & Security
- **Token Management**: JWT token generation and validation
- **Password Handling**: Secure password hashing and verification
- **Claims Management**: User claims and role-based authorisation
- **Token Refresh**: Secure token refresh mechanism

### Business Entities
- **User Management**: Application users with profiles and roles
- **Marketplace Entities**: Listings, Cards, Products, and related objects
- **Content Management**: Media files, documents, and attachments
- **Tagging System**: Flexible tagging for search and categorisation

### Validation Framework
- **FluentValidation Integration**: Comprehensive input validation
- **Custom Validators**: Business-specific validation rules
- **Error Handling**: Consistent error messaging and responses
- **Request Validation**: All API requests validated before processing

### Shared Services
- **Logging**: Structured logging with Microsoft.Extensions.Logging
- **Configuration**: Flexible configuration management
- **Utilities**: Common helper methods and extensions

## Technology Stack

### Core Framework
- **.NET 8** - Latest LTS version with modern C# features
- **Microsoft.Extensions.Identity.Core 8.0.8** - Identity framework integration

### Security & Authentication
- **Microsoft.IdentityModel.Tokens 8.2.1** - Token validation
- **System.IdentityModel.Tokens.Jwt 8.2.1** - JWT token handling

### Validation & Logging
- **FluentValidation 12.0.0** - Input validation framework
- **Microsoft.Extensions.Logging 8.0.0** - Logging abstractions
- **Microsoft.Extensions.Logging.Console 8.0.0** - Console logging provider

### HTTP & Web
- **Microsoft.AspNetCore.Http.Abstractions 2.2.0** - HTTP context abstractions

## Key Features

### Token Service
```csharp
public interface ITokenService
{
    Task<TokenResponse> GenerateTokenAsync(ApplicationUser user);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
    Task RevokeTokenAsync(string refreshToken);
    ClaimsPrincipal? ValidateToken(string token);
}
```

### Validation Pattern
```csharp
public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Valid email address is required");
            
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters");
    }
}
```

### Repository Pattern
```csharp
public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

## Business Logic

### Authentication Flow
1. **Registration**: User registration with email confirmation
2. **Login**: Credential validation and token generation
3. **Token Refresh**: Secure token renewal mechanism
4. **Logout**: Token revocation and cleanup

### Content Management
1. **Hierarchical Structure**: Listings → Cards → Products → Details
2. **Media Handling**: File uploads and media management
3. **Document Management**: PDF and document attachment handling
4. **Search & Tagging**: Flexible tagging and search capabilities

### User Management
1. **Profile Management**: User profiles with customisable information
2. **Role-Based Access**: Administrator and user roles
3. **Security**: Secure password handling and token management

## Validation Rules

### Authentication Validation
- **Email Validation**: RFC-compliant email format validation
- **Password Strength**: Minimum length and complexity requirements
- **Token Validation**: JWT token format and expiration validation

### Business Entity Validation
- **Required Fields**: Mandatory field validation
- **Length Constraints**: Appropriate field length limits
- **Business Rules**: Domain-specific validation logic
- **Relationship Validation**: Foreign key and relationship constraints

### Input Sanitisation
- **XSS Prevention**: HTML encoding and sanitisation
- **SQL Injection Prevention**: Parameterised queries via EF Core
- **Path Traversal Prevention**: File path validation

## Dependency Injection

### Service Registration
The core library provides extension methods for service registration:

```csharp
public static IServiceCollection AddCoreServices(this IServiceCollection services)
{
    services.AddScoped<ITokenService, TokenService>();
    services.AddScoped<IPasswordService, PasswordService>();
    services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    return services;
}
```

### Interface Abstractions
- Clean separation between interfaces and implementations
- Testable design with dependency injection
- Flexible service replacement for different environments

## Error Handling

### Consistent Error Responses
- Standardised error format across all operations
- Detailed validation error messages
- Proper HTTP status codes
- Secure error information (no sensitive data leakage)

### Exception Management
- Custom exception types for business scenarios
- Proper exception handling and logging
- User-friendly error messages
- Developer-friendly debugging information

## Testing Integration

### Testability Features
- Interface-based design for easy mocking
- Dependency injection for test isolation
- Validation rule testing support
- Service behavior testing capabilities

### Mock Support
- All services can be mocked for unit testing
- Repository pattern enables data layer mocking
- Token service mocking for authentication tests
- Validation testing with FluentValidation test extensions

## Future Enhancements

### Performance Optimisations
- Caching strategies for frequently accessed data
- Async/await optimisation
- Memory usage optimisation
- Query performance improvements

### Feature Additions
- Advanced search capabilities
- Real-time notifications
- Audit logging framework
- Event sourcing capabilities

### Security Enhancements
- Two-factor authentication support
- Advanced authorisation policies
- Security audit logging
- Rate limiting capabilities
