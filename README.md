# TheMarketplace - Community Trading Platform

## Overview

TheMarketplace is a .NET 8 web API backend for a community-based buy/sell/trade platform. The system provides a structured approach to marketplace operations using a hierarchical model: **Listings** contain **Cards**, which contain **Products** with detailed information.

## Current Architecture

### Core Components
- **Listings**: Top-level collections that organise trading items
- **Cards**: Visual containers within listings that group related products
- **Products**: Individual items/services with detailed specifications
- **Product Details**: Extended information including media and documents
- **User Management**: JWT-based authentication with role-based access

### Technology Stack
- **.NET 8** - Modern C# runtime with minimal API architecture
- **[WolverineFx](https://github.com/JasperFx/wolverine)** - Message bus for CQRS pattern implementation
- **Entity Framework Core 8** - Data access layer with Code First approach
- **ASP.NET Core Identity** - User authentication and authorisation
- **JWT Authentication** - Secure token-based authentication with refresh tokens
- **SQL Server 2022** - Primary database (containerised)
- **AutoMapper** - Object-to-object mapping
- **Swagger/OpenAPI** - API documentation and testing
- **xUnit & Alba** - Unit and integration testing framework
- **Docker & Docker Compose** - Containerisation for development

## Current Implementation Status

### ✅ Completed Features
- **Authentication System**
  - User registration with email confirmation
  - JWT token-based login/logout
  - Token refresh and revocation with proper validation
  - Role-based authorisation
  
- **Core Data Models**
  - Complete entity relationship mapping
  - Base entity with audit fields
  - User profiles and roles
  - Media and document support
  
- **API Endpoints**
  - Complete CRUD operations for all entities (Listings, Cards, Products, ProductDetails, Media, Documents, Tags, UserProfiles)
  - Authentication endpoints (login, register, token management)
  - Comprehensive endpoint coverage with proper HTTP verbs
  - Swagger documentation with security schemes
  
- **Infrastructure**
  - Docker containerisation for SQL Server with proper volume management
  - EF Core migrations setup with .NET 8 compatibility
  - Automated database initialisation for tests
  - Comprehensive test structure with Alba integration testing
  - Scenario-based testing with proper database isolation
  - CORS configuration
  - Proper dependency injection configuration
  - ✅ **All tests passing (227 total)** - Recent fixes to registration tests and cleanup of legacy mocks
  
### 🚧 Future Enhancements
- Advanced search and filtering functionality
- File upload and media handling optimisation
- Real-time notifications
- Performance optimisations

## Getting Started

### Prerequisites
- .NET 8 SDK
- Docker Desktop
- SQL Server Management Studio (optional)

### Quick Start

1. **Start the database**
   ```bash
   docker compose up -d
   ```

2. **Run migrations**
   ```bash
   dotnet ef database update --project Marketplace.Data --startup-project Marketplace.Api
   ```

3. **Start the API**
   ```bash
   dotnet run --project Marketplace.Api
   ```

4. **Access Swagger UI**
   - Navigate to: `https://localhost:5001/swagger`
   - Database connection: `Server=127.0.0.1,1433;User=sa;Password=P@ssw0rd!`

### Development Setup

The solution follows a clean architecture pattern with four main projects:

- **Marketplace.Api** - Web API layer with endpoints and configuration
- **Marketplace.Core** - Business logic and shared utilities
- **Marketplace.Data** - Data access layer with EF Core
- **Marketplace.Test** - Comprehensive unit and integration tests

## Future Roadmap

### Phase 1: Core Marketplace Features (Q2 2025)
- **Priority: High**
- ✅ Complete CRUD operations for all entities (COMPLETED)
- File upload and media handling optimisation
- Advanced search and filtering functionality
- Email notifications system
- Input validation and error handling enhancements
- Performance optimisation and caching

### Phase 2: Enhanced User Experience (Q3 2025)
- **Priority: High**
- Advanced search with filters and sorting
- Real-time notifications (SignalR)
- Messaging system between users
- Transaction history tracking
- Wishlist/favorites functionality
- Mobile-responsive web interface

### Phase 3: Business Features (Q4 2025)
- **Priority: Medium**
- Payment integration (Stripe/PayPal)
- Rating and review system
- Escrow service for secure transactions
- Admin dashboard for platform management
- Reporting and analytics
- API rate limiting and monitoring

### Phase 4: Advanced Features (Q1 2026)
- **Priority: Medium**
- AI-powered product recommendations
- Automated content moderation
- Multi-language support
- Mobile app (React Native/Flutter)
- Advanced analytics dashboard
- Integration with external marketplaces

### Phase 5: Scale & Optimise (2026)
- **Priority: Low**
- Microservices architecture migration
- Redis caching implementation
- CDN for media delivery
- Advanced security features (2FA, audit logging)
- Machine learning for fraud detection
- White-label solution for other organisations

## Development Guidelines

### Code Organisation
- Follow SOLID principles and clean architecture
- Use the mediator pattern via WolverineFx
- Implement comprehensive unit and integration tests
- Maintain API documentation via Swagger
- Follow conventional commits for version control

### Security Considerations
- JWT tokens with refresh mechanism
- Input validation and sanitisation
- SQL injection prevention via EF Core
- CORS configuration for frontend integration
- Role-based access control

### Performance Guidelines
- Use async/await for all database operations
- Implement pagination for large data sets
- Consider caching strategies for frequently accessed data
- Optimise database queries with proper indexing

## Contributing

For detailed development instructions, navigate to individual project README.md files:
- `Marketplace.Api/README.md` - API development guidelines and technology stack
- `Marketplace.Core/README.md` - Business logic, validation, and domain services
- `Marketplace.Data/README.md` - Database, migrations, and repository patterns
- `Marketplace.Test/README.md` - Testing strategies, factories, and infrastructure

### Additional Documentation
- `Marketplace.Api/Endpoints/README.md` - Complete endpoint reference and patterns
- `Marketplace.Data/Configurations/README.md` - Entity configuration patterns
- `Marketplace.Data/Repositories/README.md` - Repository implementation details
- `Marketplace.Test/Factories/README.md` - Test factories and infrastructure setup

## Docker Development

The project includes Docker support for the SQL Server database:

```yaml
# Start database container
docker compose up -d

# Stop and remove containers
docker compose down

# View logs
docker compose logs -f
```

**Database Connection Details:**
- Server: `127.0.0.1,1433`
- Authentication: SQL Server Authentication
- Username: `sa`
- Password: `P@ssw0rd!` (change in docker-compose.yaml)

## Testing

The solution includes comprehensive testing:

```bash
# Run all tests
dotnet test

# Run with verbose output
dotnet test --verbosity normal

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test Marketplace.Test

# Run tests with Alba integration testing
dotnet test --logger "console;verbosity=detailed"
```
