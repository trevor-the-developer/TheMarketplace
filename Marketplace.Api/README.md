# Marketplace API Project

.NET 8 Minimal API project demonstrating a lean coding pattern with emphasis on readable code, semantic naming conventions, and comprehensive testing. The project leverages WolverineFx as a mediator library (a better, easier-to-work-with version of .NET MediatR).

## Current Status
✅ **All API tests passing** - Authentication, endpoints, and integration tests are fully functional and verified.
✅ **Registration system fixed** - Recent updates resolved compilation issues and aligned with current architecture.
✅ **Legacy code removed** - Cleaned up unused enhanced mocks for better maintainability.

## Features

- **Authentication & Authorisation**: JWT-based authentication with refresh tokens, role-based access control
- **Clean Architecture**: Minimal API endpoints with proper separation of concerns
- **CQRS Pattern**: Implemented using WolverineFx mediator for command and query separation
- **Comprehensive Testing**: Extensive unit and integration tests with Alba framework
- **API Documentation**: Swagger/OpenAPI documentation for all endpoints

## Docker Configuration

This project uses a Docker SQL Server 2022 image as the backing store for Entity Framework operations. The database container is configured with:

- **Host**: `127.0.0.1,1433`
- **User**: `sa`
- **Password**: Defined in `docker-compose.yaml` (change to your own secure password)
- **Database**: `Marketplace` (automatically created)

**Important**: Ensure any local instances of SQL Server or SQL Express are shut down before starting the container.

### Container Services:
- **SQL Server 2022**: Primary database with named volumes for data persistence
- **Automated Setup**: Database and migrations are applied automatically during test runs

### Quick Start:
```bash
# Start database container
docker compose up -d

# Apply migrations
dotnet ef database update --project ../Marketplace.Data --startup-project .

# Run the API
dotnet run
```

## Wolverine as a Mediator

This project uses WolverineFx as a mediator tool for implementing the CQRS pattern. The configuration:

- **Message Bus**: Handles command and query routing
- **Handler Discovery**: Automatic discovery of message handlers
- **Runtime Processing**: Builds connective code at runtime for message processing
- **Persistence**: Disabled for inbox and outbox (in-memory processing)

### Benefits:
- Clean separation between API endpoints and business logic
- Automatic handler discovery and registration
- Better testability with isolated handlers
- **Simplified command/query processing pipeline
- **Complete CRUD Operations**: Full Create, Read, Update, Delete operations for all entities

### Handler Pattern:
Each endpoint uses a corresponding handler class that processes incoming commands/queries. For example:
- `LoginRequest` → `LoginHandler`
- `RegisterRequest` → `RegisterHandler`
- `RefreshTokenRequest` → `RefreshTokenHandler`

## Technology Stack

### Core Framework
- **.NET 8** - Latest LTS version with minimal APIs
- **ASP.NET Core 8.0.8** - Web framework with OpenAPI support
- **Entity Framework Core 8.0.8** - ORM with SQL Server provider

### Messaging & Patterns
- **WolverineFx 3.6.0** - Advanced mediator and message bus
- **Weasel.SqlServer 7.12.4** - Database schema management
- **AutoMapper 12.0.1** - Object-to-object mapping

### Authentication & Security
- **Microsoft.AspNetCore.Authentication.JwtBearer 8.0.8** - JWT authentication
- **Microsoft.IdentityModel.Tokens 8.2.1** - Token validation
- **System.IdentityModel.Tokens.Jwt 8.2.1** - JWT token handling

### Validation & Documentation
- **FluentValidation 12.0.0** - Input validation
- **MiniValidation 0.9.0** - Lightweight validation
- **Swashbuckle.AspNetCore 7.2.0** - OpenAPI/Swagger documentation

### Testing Integration
- **Alba 8.0.0** - Integration testing framework
- **xUnit** - Unit testing framework
- **Moq 4.20.72** - Mocking framework
