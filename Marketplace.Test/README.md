# Marketplace Test Suite

Comprehensive testing suite for TheMarketplace solution using modern .NET 8 testing frameworks and patterns.

## Current Status
✅ **All 227 tests passing** - Complete test coverage across all modules with consistent results.
✅ **Registration tests fixed** - Recent updates resolved compilation issues and aligned with current architecture.
✅ **Legacy mocks removed** - Cleaned up unused enhanced mocks for better maintainability.
✅ **Integration testing** - Alba framework provides robust end-to-end testing capabilities.

## Test Architecture

### Test Organisation
```
Marketplace.Test/
├── Scenarios/
│   ├── Authentication/
│   │   ├── IntegrationTests/
│   │   └── UnitTests/
│   ├── Cards/
│   ├── Documents/
│   ├── Listings/
│   ├── Media/
│   ├── ProductDetails/
│   ├── Products/
│   ├── Tags/
│   └── UserProfiles/
├── Factories/
└── Infrastructure/
```

### Testing Patterns

#### Unit Tests
- **Handler Testing**: Isolated testing of business logic handlers
- **Validator Testing**: Comprehensive validation rule testing
- **Model Testing**: Entity and DTO validation
- **Repository Testing**: Data access layer testing with mocks

#### Integration Tests
- **End-to-End Testing**: Full API workflow testing with Alba
- **Database Integration**: Real database operations with containerised SQL Server
- **Authentication Flow**: Complete JWT authentication and authorisation testing
- **Scenario-based Testing**: Real-world usage scenarios

## Test Statistics

### Coverage by Module
- **Authentication**: 13 test classes covering login, registration, and token management
- **Cards**: 4 test classes covering CRUD operations and validation
- **Documents**: 3 test classes covering document handling and validation
- **Listings**: 4 test classes covering listing management and validation
- **Media**: 3 test classes covering media file operations
- **ProductDetails**: 2 test classes covering detailed product information
- **Products**: 3 test classes covering product management
- **Tags**: 3 test classes covering tag operations
- **UserProfiles**: 2 test classes covering user profile management

### Test Types
- **Unit Tests**: ~80% of test suite - Fast, isolated testing of individual components
- **Integration Tests**: ~20% of test suite - End-to-end scenarios with database and API
- **Validation Tests**: Comprehensive input validation across all endpoints
- **Handler Tests**: Business logic testing with proper mocking

## Technology Stack

### Testing Frameworks
- **xUnit 2.9.2** - Primary testing framework with modern assertion patterns
- **Alba 8.0.0** - Integration testing framework for ASP.NET Core
- **Moq 4.20.72** - Mocking framework for unit test isolation
- **Microsoft.NET.Test.Sdk 17.12.0** - .NET test SDK

### Database Testing
- **Microsoft.EntityFrameworkCore.InMemory 8.0.8** - In-memory database for unit tests
- **SQL Server Container** - Containerised database for integration tests
- **Database Test Fixture** - Automated database setup and cleanup

### Code Coverage
- **coverlet.collector 6.0.2** - Code coverage collection
- **Visual Studio integration** - Built-in test runner support

## Infrastructure Features

### Database Test Fixture
- **Automated Container Management**: SQL Server container lifecycle management
- **Migration Application**: Automatic database schema setup
- **Data Cleanup**: Clean state between test runs
- **Connection Management**: Proper connection string configuration

### Authentication Infrastructure
- **JWT Token Testing**: Complete token generation and validation
- **Role-Based Testing**: Different user roles and permissions
- **Refresh Token Testing**: Token refresh mechanism validation
- **Mock Authentication**: Consistent authentication setup across tests

### Test Data Management
- **Factory Pattern**: Standardised test data creation
- **Relationship Handling**: Proper foreign key relationships in test data
- **Cleanup Strategies**: Efficient test data cleanup
- **Isolation**: Tests don't interfere with each other

## Running Tests

### All Tests
```bash
# Run all tests
dotnet test

# Run with verbose output
dotnet test --verbosity normal

# Run with detailed logging
dotnet test --logger "console;verbosity=detailed"
```

### Specific Test Categories
```bash
# Run only unit tests
dotnet test --filter "Category=Unit"

# Run only integration tests
dotnet test --filter "Category=Integration"

# Run authentication tests only
dotnet test --filter "FullyQualifiedName~Authentication"
```

### Code Coverage
```bash
# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate coverage report
dotnet test --collect:"XPlat Code Coverage" --results-directory ./TestResults
```

## Test Development Guidelines

### Writing Unit Tests
1. **Arrange-Act-Assert Pattern**: Clear test structure
2. **Single Responsibility**: One assertion per test when possible
3. **Descriptive Names**: Test names describe the scenario
4. **Mock Dependencies**: Isolate the system under test
5. **Test Data Builders**: Use factory patterns for consistent test data

### Writing Integration Tests
1. **Database Isolation**: Each test gets a clean database state
2. **Realistic Scenarios**: Test real-world usage patterns
3. **End-to-End Flows**: Test complete request/response cycles
4. **Authentication Context**: Test with proper authentication
5. **Error Scenarios**: Test both success and failure cases

### Best Practices
- **Fast Tests**: Unit tests should run quickly
- **Reliable Tests**: Tests should be deterministic and repeatable
- **Independent Tests**: Tests should not depend on each other
- **Clear Assertions**: Use meaningful assertion messages
- **Cleanup**: Proper test cleanup to prevent side effects

## Recent Improvements

### Registration Tests Fix
- Fixed compilation issues in registration test handlers
- Aligned mock setups with current `RegisterHandler` implementation
- Replaced legacy `EnhancedMockUserManager` and `EnhancedMockRoleManager`
- Updated method signatures to match current architecture

### Legacy Code Cleanup
- Removed unused `EnhancedMockUserManager.cs`
- Removed unused `EnhancedMockRoleManager.cs`
- Removed unused `MockUserManagerOptions.cs`
- Removed unused `MockRoleManagerOptions.cs`
- Standardised mock usage across all tests

### Architecture Alignment
- Consistent use of `MockAuthenticationRepository`
- Proper dependency injection setup in tests
- Aligned test patterns with current business logic
- Improved test maintainability and reliability

## Future Enhancements

### Test Coverage Expansion
- Add performance testing with load scenarios
- Implement mutation testing for quality validation
- Add contract testing for API stability
- Expand integration test scenarios

### Infrastructure Improvements
- Parallel test execution optimisation
- Test data seeding improvements
- Enhanced reporting and analytics
- Automated test environment management
