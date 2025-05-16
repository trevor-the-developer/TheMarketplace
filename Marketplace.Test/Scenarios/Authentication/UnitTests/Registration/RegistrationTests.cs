using Marketplace.Api.Endpoints.Authentication.Registration;
using Marketplace.Core;
using Marketplace.Data;
using Marketplace.Data.Entities;
using Marketplace.Test.Data;
using Marketplace.Test.Mocks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using RegisterStepOneResponse = Marketplace.Test.Data.RegisterStepOneResponse;

namespace Marketplace.Test.Scenarios.Authentication.UnitTests.Registration;

public class RegistrationTests(WebAppFixture fixture) : ScenarioContext(fixture)
{
    [Fact]
    public void Registration_Step_One_Success_Test()
    {
        var registrationStepOne = false;

        registrationStepOne = true;

        Assert.True(registrationStepOne);
    }

    [Fact]
    public void Registration_Step_One_Fail_Test()
    {
        var registrationStepOne = false;

        registrationStepOne = false;

        Assert.False(registrationStepOne);
    }

    [Fact]
    public void Registration_Step_One_ApiError_Test()
    {
        var registrationStepOne = new RegisterStepOneResponse();
        registrationStepOne.RegistrationStepOne = false;
        registrationStepOne.ConfirmationEmailLink = "TestConfirmationLink";
        registrationStepOne.ApiError = new ApiError(
            "Not authorised", 
            403, 
            "Not authorised to view object.", 
            "TesStackTrace");

        Assert.NotNull(registrationStepOne);
        Assert.False(registrationStepOne.RegistrationStepOne);
        Assert.NotNull(registrationStepOne.ApiError);
        Assert.IsType<ApiError>(registrationStepOne.ApiError);
    }

    [Fact]
    public async Task Registration_Step_One_Null_RoleManager_Test()
    {
        var command = new RegisterRequest
        {
            FirstName = "TestFirstname",
            LastName = "TestLastname",
            Email = "TestEmail",
            Password = "TestPassword",
        };
        
        TestData.TestUserOne = "testuserone@testing.local";
        command.Email = TestData.TestUserOne;
        var mockUserManager = new MockUserManager(TestData.TestApplicationUser);
        var mockRoleManager = new MockRoleManager(TestData.TestAppUserRole);
        var mockLogger  = new Mock<ILogger<RegisterHandler>>();
        var mockDbContext = new MockDbContext();
        var mockUrlHelper = new Mock<IUrlHelper>();
        
        var mockUsers = new Mock<DbSet<ApplicationUser>>();
        mockDbContext.Setup(x => x.Users).Returns(mockUsers.Object);

        var registerHandler = new RegisterHandler();
        var response = await registerHandler.Handle(
            command,
            mockUserManager.Object,
            mockRoleManager.Object,
            mockLogger.Object,
            mockDbContext.Object,
            mockUrlHelper.Object
            );
        
        // TODO: setup test and assert
        // user id, reg step one, api error
    }
    
    // TODO: add additional test cases:
    // - deps
    // - FindByEmailAsync
    // - CreateAsync
    // - CreateRoleAsync
    //  - AddUserToRoleAsync
    //  - GenerateEmailConfirmationTokenAsync
}