# TheMarketplace - Community Trading Platform

## Overview

TheMarketplace is a .NET 8 web API backend for a community-based buy/sell/trade platform. The system provides a
structured approach to marketplace operations using a hierarchical model: **Listings** contain **Cards**, which contain
**Products** with detailed information.

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
- **FluentValidation** - Input validation framework
- **MailKit** - Email service implementation
- **MailHog** - Email testing service (containerised)
- **Swagger/OpenAPI** - API documentation and testing
- **xUnit & Alba** - Unit and integration testing framework
- **Docker & Docker Compose** - Containerisation for development

## Current Implementation Status

### ✅ Completed Features

- **Authentication System**
    - **Enhanced User Registration**: Streamlined single-step registration with email confirmation
    - **Email Confirmation**: Robust token-based email verification with MailHog integration
    - **Configuration Management**: Frontend URL configuration with fallback to constants
    - **Age Validation**: Enforced minimum age requirement (13+ years)
    - **JWT Authentication**: Secure token-based authentication with refresh tokens
    - **Token Management**: Proper token refresh and revocation with validation
    - **Role-Based Authorization**: Complete role-based access control
    - **Email Service**: MailKit implementation for reliable email delivery

- **Core Data Models**
    - Complete entity relationship mapping
    - Base entity with audit fields
    - User profiles and roles
    - Media and document support

- **API Endpoints**
    - Complete CRUD operations for all entities (Listings, Cards, Products, ProductDetails, Media, Documents, Tags,
      UserProfiles)
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
    - ✅ **All tests passing (229 total)** - Recent fixes to registration tests and cleanup of legacy mocks

### ✅ Recent Improvements (January 2025)

- **Registration Flow Enhancement**: Fixed and streamlined single-step registration process
- **Email Confirmation**: Resolved email confirmation links to use correct frontend URLs
- **Configuration Management**: Added `FrontendSettings.BaseUrl` configuration in appsettings
- **Constants Enhancement**: Added `ApiConstants.DefaultFrontendBaseUrl` for consistent fallback
- **Test Suite Fixes**: Updated all registration tests to work with new configuration pattern
- **MailHog Integration**: Improved email testing workflow with proper URL routing
- **Frontend Integration**: Email confirmation links now correctly point to frontend (http://localhost:3000)

### 🙧 Future Enhancements

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

5. **Start the Frontend** (Optional - for full registration testing)
   ```bash
   cd TheMarketplace.Frontend
   npm install
   npm run dev
   ```
    - Frontend will be available at: `http://localhost:3000`

## Testing the Registration Flow

### End-to-End Registration Testing

The registration system includes email confirmation with MailHog integration for testing. Here's how to test the
complete flow:

#### Prerequisites

- Backend API running on port 5212
- Frontend running on port 3000 (optional, can test via API only)
- MailHog running in Docker container (port 8025)

#### Method 1: Full Frontend Testing (Recommended)

1. **Start all services**:
   ```bash
   # Start database and MailHog
   docker compose up -d
   
   # Start backend API
   dotnet run --project Marketplace.Api
   
   # Start frontend (in separate terminal)
   cd TheMarketplace.Frontend
   npm run dev
   ```

2. **Register a new user**:
    - Navigate to `http://localhost:3000/register`
    - Fill out the registration form:
        - First Name: `John`
        - Last Name: `Doe`
        - Email: `john.doe@example.com`
        - Password: `SecurePass123!`
        - Date of Birth: Any date making the user 13+ years old
    - Click "Register"

3. **Check registration response**:
    - Should see success message
    - User account created but not yet confirmed
    - Confirmation email sent to MailHog

4. **View confirmation email in MailHog**:
    - Open `http://localhost:8025` in your browser
    - Click on the latest email to `john.doe@example.com`
    - Subject should be "Confirm your registration"
    - Email contains a clickable confirmation link

5. **Complete registration**:
    - Click the confirmation link in the email
    - Link format: `http://localhost:3000/api/auth/confirm-email?userId=...&token=...&email=...`
    - Should see JSON response: `{"registrationCompleted": true, "confirmationCode": "RegistrationComplete"}`
    - User account is now fully activated

6. **Test login**:
    - Navigate to `http://localhost:3000/login`
    - Use the registered credentials
    - Should successfully log in and receive JWT token

#### Method 2: API-Only Testing (Backend Focus)

1. **Register via API**:
   ```bash
   curl -X POST http://localhost:5212/api/auth/register \
     -H "Content-Type: application/json" \
     -d '{
       "firstName": "Jane",
       "lastName": "Smith",
       "email": "jane.smith@example.com",
       "password": "SecurePass123!",
       "dateOfBirth": "1990-01-01"
     }'
   ```

2. **Check MailHog for confirmation email**:
    - Visit `http://localhost:8025`
    - Find email to `jane.smith@example.com`
    - Copy the confirmation URL from the email

3. **Confirm email via API**:
   ```bash
   # Extract userId, token, and email from the confirmation URL
   curl "http://localhost:3000/api/auth/confirm-email?userId=<USER_ID>&token=<TOKEN>&email=jane.smith%40example.com"
   ```

4. **Test login**:
   ```bash
   curl -X POST http://localhost:5212/api/auth/login \
     -H "Content-Type: application/json" \
     -d '{
       "email": "jane.smith@example.com",
       "password": "SecurePass123!"
     }'
   ```

#### Method 3: Swagger UI Testing

1. **Access Swagger UI**:
    - Navigate to `http://localhost:5212/swagger`

2. **Test registration**:
    - Find `POST /api/auth/register` endpoint
    - Click "Try it out"
    - Fill in the request body with user details
    - Execute the request

3. **Check MailHog**:
    - Visit `http://localhost:8025` to view the confirmation email

4. **Test email confirmation**:
    - Find `GET /api/auth/confirm-email` endpoint in Swagger
    - Enter the `userId`, `token`, and `email` parameters from the email
    - Execute the request

### Expected Behavior

- **Registration Success**: Returns user ID and confirmation link
- **Email Delivery**: Confirmation email appears in MailHog within seconds
- **Email Content**: Contains clickable link pointing to frontend URL
- **Confirmation Success**: Returns `{"registrationCompleted": true}`
- **Login Success**: User can authenticate with confirmed credentials

### Troubleshooting

- **Email not appearing**: Check MailHog container is running (`docker ps`)
- **Confirmation link fails**: Ensure frontend is running on port 3000
- **Database errors**: Run migrations (`dotnet ef database update`)
- **Port conflicts**: Check no other services are using ports 5212, 3000, or 8025

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
