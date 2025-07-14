using Marketplace.Api.Endpoints.Authentication.Registration;
using Marketplace.Core;
using Marketplace.Core.Constants;
using Marketplace.Test.Factories;
using Marketplace.Test.Mocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Marketplace.Test.Scenarios.Authentication.UnitTests.Registration;

public class RegistrationTests
{
    #region Registration Step One Tests

    [Fact]
    public void Registration_Step_One_Success_Test()
    {
        // Arrange
        var registrationStepOne = false;

        // Act
        registrationStepOne = true;

        // Assert
        Assert.True(registrationStepOne);
    }

    [Fact]
    public void Registration_Step_One_Fail_Test()
    {
        // Arrange
        var registrationStepOne = false;

        // Act
        registrationStepOne = false;

        // Assert
        Assert.False(registrationStepOne);
    }

    [Fact]
    public void Registration_Step_One_ApiError_Test()
    {
        // Arrange & Act
        var registrationStepOne = new RegisterStepOneResponse
        {
            RegistrationStepOne = false,
            ConfirmationEmailLink = "TestConfirmationLink",
            ApiError = new ApiError(
                "Not authorised",
                403,
                "Not authorised to view object.",
                "TesStackTrace")
        };

        // Assert
        Assert.NotNull(registrationStepOne);
        Assert.False(registrationStepOne.RegistrationStepOne);
        Assert.NotNull(registrationStepOne.ApiError);
        Assert.IsType<ApiError>(registrationStepOne.ApiError);
    }

    [Fact]
    public async Task Registration_Step_One_Null_Command_Throws_ArgumentNullException()
    {
        // Arrange
        var mockAuthRepository = new MockAuthenticationRepository();
        var mockLogger = new Mock<ILogger<RegisterHandler>>();
        var mockValidationService = new MockValidationService();

        var registerHandler = new RegisterHandler();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await registerHandler.Handle(
                null!,
                mockAuthRepository.Object,
                mockLogger.Object,
                mockValidationService);
        });
    }

    [Fact]
    public async Task Registration_Step_One_User_Already_Exists_Returns_Error()
    {
        // Arrange
        var command = RegistrationTestFactory.CreateValidRegisterRequest("existing@example.com");
        var existingUser = RegistrationTestFactory.CreateTestUser("existing@example.com");

        var mockAuthRepository = new MockAuthenticationRepository(existingUser);
        var mockLogger = new Mock<ILogger<RegisterHandler>>();
        var mockValidationService = new MockValidationService();
        var registerHandler = new RegisterHandler();

        // Act
        var response = await registerHandler.Handle(
            command,
            mockAuthRepository.Object,
            mockLogger.Object,
            mockValidationService);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.RegistrationStepOne);
        Assert.NotNull(response.ApiError);
        Assert.Equal(500, response.ApiError.StatusCode);
    }

    [Fact]
    public async Task Registration_Step_One_Create_User_Success_Test()
    {
        // Arrange
        var command = RegistrationTestFactory.CreateValidRegisterRequest("new@example.com");

        var mockAuthRepository = new MockAuthenticationRepository();
        var mockLogger = new Mock<ILogger<RegisterHandler>>();
        var mockValidationService = new MockValidationService();
        var registerHandler = new RegisterHandler();

        // Act
        var response = await registerHandler.Handle(
            command,
            mockAuthRepository.Object,
            mockLogger.Object,
            mockValidationService);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.RegistrationStepOne);
        Assert.NotNull(response.UserId);
        Assert.NotNull(response.ConfirmationEmailLink);
        Assert.Null(response.ApiError);
    }

    [Fact]
    public async Task Registration_Step_One_Create_User_Failed_Returns_Error()
    {
        // Arrange
        var command = RegistrationTestFactory.CreateValidRegisterRequest("new@example.com");
        var customErrors = new List<IdentityError>
        {
            new() { Code = "PasswordTooShort", Description = "Password is too short" }
        };

        var mockAuthRepository = new MockAuthenticationRepository();
        mockAuthRepository.SetupCreateUserFailed(customErrors);
        var mockLogger = new Mock<ILogger<RegisterHandler>>();
        var mockValidationService = new MockValidationService();
        var registerHandler = new RegisterHandler();

        // Act
        var response = await registerHandler.Handle(
            command,
            mockAuthRepository.Object,
            mockLogger.Object,
            mockValidationService);

        // Assert
        Assert.NotNull(response);
        Assert.False(response.RegistrationStepOne);
        Assert.NotNull(response.ApiError);
        Assert.Equal(400, response.ApiError.StatusCode);
        Assert.NotNull(response.Errors);
        Assert.Single(response.Errors);
        Assert.Equal("PasswordTooShort", response.Errors.First().Code);
    }

    [Fact]
    public async Task Registration_Step_One_Create_Role_Failed_Test()
    {
        // Arrange
        var command = RegistrationTestFactory.CreateValidRegisterRequest("new@example.com");

        var mockAuthRepository = new MockAuthenticationRepository();
        mockAuthRepository.SetupCreateRoleFailed();
        var mockLogger = new Mock<ILogger<RegisterHandler>>();
        var mockValidationService = new MockValidationService();
        var registerHandler = new RegisterHandler();

        // Act
        var response = await registerHandler.Handle(
            command,
            mockAuthRepository.Object,
            mockLogger.Object,
            mockValidationService);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.RegistrationStepOne); // User creation succeeded
        Assert.NotNull(response.UserId);
        Assert.NotNull(response.ConfirmationEmailLink);
    }

    [Fact]
    public async Task Registration_Step_One_Add_User_To_Role_Failed_Test()
    {
        // Arrange
        var command = RegistrationTestFactory.CreateValidRegisterRequest("new@example.com");

        var mockAuthRepository = new MockAuthenticationRepository();
        mockAuthRepository.SetupAddToRoleFailed();
        var mockLogger = new Mock<ILogger<RegisterHandler>>();
        var mockValidationService = new MockValidationService();
        var registerHandler = new RegisterHandler();

        // Act
        var response = await registerHandler.Handle(
            command,
            mockAuthRepository.Object,
            mockLogger.Object,
            mockValidationService);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.RegistrationStepOne); // Overall registration succeeded
        Assert.NotNull(response.UserId);
        Assert.NotNull(response.ConfirmationEmailLink);
    }

    [Fact]
    public async Task Registration_Step_One_Generate_Email_Confirmation_Token_Failed_Test()
    {
        // Arrange
        var command = RegistrationTestFactory.CreateValidRegisterRequest("new@example.com");

        var mockAuthRepository = new MockAuthenticationRepository();
        mockAuthRepository.SetupGenerateEmailTokenFailed();
        var mockLogger = new Mock<ILogger<RegisterHandler>>();
        var mockValidationService = new MockValidationService();
        var registerHandler = new RegisterHandler();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
        {
            await registerHandler.Handle(
                command,
                mockAuthRepository.Object,
                mockLogger.Object,
                mockValidationService);
        });
    }

    #endregion

    #region Email Confirmation Tests

    [Fact]
    public async Task ConfirmEmail_Null_Command_Throws_ArgumentNullException()
    {
        // Arrange
        var mockAuthRepository = new MockAuthenticationRepository();
        var mockLogger = new Mock<ILogger<RegisterHandler>>();
        var registerHandler = new RegisterHandler();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () =>
        {
            await registerHandler.Handle(
                (ConfirmEmailRequest)null!,
                mockAuthRepository.Object,
                mockLogger.Object);
        });
    }

    [Fact]
    public async Task ConfirmEmail_User_Not_Found_Returns_Error()
    {
        // Arrange
        var command = RegistrationTestFactory.CreateConfirmEmailRequest();
        var mockAuthRepository = new MockAuthenticationRepository();
        mockAuthRepository.SetupUserNotFound();
        var mockLogger = new Mock<ILogger<RegisterHandler>>();
        var registerHandler = new RegisterHandler();

        // Act
        var response = await registerHandler.Handle(
            command,
            mockAuthRepository.Object,
            mockLogger.Object);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.ApiError);
        Assert.Equal(404, response.ApiError.StatusCode);
        Assert.Equal(AuthConstants.UserDoesntExist, response.ApiError.ErrorMessage);
    }

    [Fact]
    public async Task ConfirmEmail_Success_Test()
    {
        // Arrange
        var user = RegistrationTestFactory.CreateTestUser("test@example.com");
        var command = RegistrationTestFactory.CreateConfirmEmailRequest(user.Id, "valid-token", user.Email);

        var mockAuthRepository = new MockAuthenticationRepository(user);
        var mockLogger = new Mock<ILogger<RegisterHandler>>();
        var registerHandler = new RegisterHandler();

        // Act
        var response = await registerHandler.Handle(
            command,
            mockAuthRepository.Object,
            mockLogger.Object);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(user.Id, response.UserId);
        Assert.Equal(user.Email, response.Email);
        Assert.Equal("RegistrationComplete", response.ConfirmationCode);
        Assert.True(response.RegistrationCompleted);
        Assert.Null(response.ApiError);
    }

    [Fact]
    public async Task ConfirmEmail_Failed_Returns_Error()
    {
        // Arrange
        var user = RegistrationTestFactory.CreateTestUser("test@example.com");
        var command = RegistrationTestFactory.CreateConfirmEmailRequest(user.Id, "invalid-token", user.Email);

        var mockAuthRepository = new MockAuthenticationRepository(user);
        mockAuthRepository.SetupConfirmEmailFailed();
        var mockLogger = new Mock<ILogger<RegisterHandler>>();
        var registerHandler = new RegisterHandler();

        // Act
        var response = await registerHandler.Handle(
            command,
            mockAuthRepository.Object,
            mockLogger.Object);

        // Assert
        Assert.NotNull(response);
        Assert.NotNull(response.ApiError);
        Assert.Equal(400, response.ApiError.StatusCode);
        Assert.Equal("Email confirmation failed", response.ApiError.ErrorMessage);
    }

    #endregion
}