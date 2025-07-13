# Test Factories & Infrastructure

Factory classes provide standardised object creation and lifecycle management for testing scenarios. The test infrastructure includes automated database setup, user authentication, and consistent test data generation.

## Current Status
✅ **All factories working** - Test data creation is consistent and reliable across all test scenarios.
✅ **Database fixture stable** - Automated SQL Server container management with proper cleanup.
✅ **Authentication mocks updated** - Recent fixes aligned mock setups with current implementations.

## Factory Pattern Benefits

1. **Standardised Object Creation**: Consistent way to create test objects
2. **Lifecycle Management**: Handle object construction and destruction
3. **Reduced Boilerplate**: Minimise repetitive test setup code
4. **Consistency**: Ensure uniform test data across all test cases

## Current Test Infrastructure

### Database Test Fixture

- **Automated Setup**: SQL Server container management
- **Migration Application**: Automatic database schema creation
- **Cleanup**: Proper disposal of test resources
- **Isolation**: Each test run uses a clean database state

### Authentication Factories

- **User Creation**: Generate test users with appropriate roles
- **Token Generation**: Create valid JWT tokens for authenticated tests
- **Role Management**: Test different authorisation scenarios
- **Mock Services**: Properly configured authentication mocks

### Data Factories

- **Entity Creation**: Generate test entities with valid relationships
- **Seed Data**: Create consistent test data for integration tests
- **Mock Objects**: Properly configured mocks for unit tests

## Usage Examples

### Database Test Setup

```csharp
[Collection("DatabaseCollection")]
public class IntegrationTestBase
{
    protected readonly DatabaseTestFixture _fixture;
    
    public IntegrationTestBase(DatabaseTestFixture fixture)
    {
        _fixture = fixture;
    }
    
    // Tests have access to properly configured database
}
```

### Authentication Test Setup

```csharp
public class AuthenticationTests : IntegrationTestBase
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsToken()
    {
        // Factory creates test user with proper setup
        var testUser = await UserFactory.CreateTestUserAsync();
        
        // Test authentication flow
        var loginRequest = new LoginRequest(testUser.Email, "TestPassword123!");
        var response = await _client.PostAsync("/api/auth/login", loginRequest);
        
        // Assertions
        response.Should().BeSuccessful();
    }
}
```

## Test Infrastructure Features

### Automated Database Management

- **Container Health Checks**: Ensure SQL Server is ready before tests
- **Migration Application**: Apply EF Core migrations automatically
- **Data Cleanup**: Clean state between test runs
- **Connection Management**: Proper connection string configuration

### Authentication Infrastructure

- **JWT Token Validation**: Proper token validation setup
- **Role-Based Testing**: Test different user roles and permissions
- **Refresh Token Testing**: Validate token refresh mechanisms
- **Authorisation Testing**: Test protected endpoints

### Test Data Management

- **Consistent Data**: Standardised test data across all tests
- **Relationship Handling**: Proper foreign key relationships
- **Cleanup Strategies**: Efficient test data cleanup
- **Isolation**: Tests don't interfere with each other

## Benefits in Testing

- **Reliability**: Consistent test environment setup
- **Maintainability**: Changes to test infrastructure affect all tests uniformly
- **Speed**: Efficient test setup and teardown
- **Isolation**: Each test runs in a clean environment
- **Reusability**: Common test scenarios can be easily reused

## Test Statistics

- **Total Tests**: Comprehensive test coverage across all modules
- **Coverage**: Unit and integration tests for all major components
- **Authentication**: Comprehensive auth flow testing with JWT helpers
- **Database**: Full integration testing with containerized SQL Server
- **Mocking**: Proper isolation for unit tests
- **Alba Integration**: Modern integration testing framework
- **Scenario-based Testing**: Organized by feature domains
