using System.Net;
using Alba;
using Marketplace.Api.Endpoints.Authentication.Registration;
using Marketplace.Core.Constants;
using Marketplace.Test.Factories;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Marketplace.Test.Scenarios.Authentication.IntegrationTests.Authentication;

public class RegistrationTests(WebAppFixture fixture) : ScenarioContext(fixture)
{
    [Fact]
    public async Task RegisterStepOne_Success()
    {
        var registerRequest = RegistrationTestFactory.CreateValidRegisterRequest("newuser@example.com");
        
        var response = await Host.Scenario(_ =>
        {
            _.Post
                .Json(registerRequest, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiSlashRegister);
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var result = await response.ReadAsJsonAsync<RegisterStepOneResponse>();

        Assert.NotNull(result);
        Assert.True(result.RegistrationStepOne);
        Assert.NotNull(result.UserId);
        Assert.NotNull(result.ConfirmationEmailLink);
        Assert.Null(result.ApiError);
    }

    [Fact]
    public async Task RegisterStepOne_Duplicate_Email_Returns_Error()
    {
        // First registration
        var registerRequest = RegistrationTestFactory.CreateValidRegisterRequest("duplicate@example.com");
        
        await Host.Scenario(_ =>
        {
            _.Post
                .Json(registerRequest, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiSlashRegister);
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        // Attempt duplicate registration
        await Host.Scenario(_ =>
        {
            _.Post
                .Json(registerRequest, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiSlashRegister);
            _.StatusCodeShouldBe(HttpStatusCode.InternalServerError);
        });
    }

    [Fact]
    public async Task RegisterStepOne_Invalid_Data_Returns_BadRequest()
    {
        var invalidRequest = RegistrationTestFactory.CreateInvalidRegisterRequest();
        
        await Host.Scenario(_ =>
        {
            _.Post
                .Json(invalidRequest, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiSlashRegister);
            _.StatusCodeShouldBe(HttpStatusCode.BadRequest);
        });
    }

    [Fact]
    public async Task ConfirmEmail_Success()
    {
        // First register a user
        var registerRequest = RegistrationTestFactory.CreateValidRegisterRequest("confirm@example.com");
        
        var registerResponse = await Host.Scenario(_ =>
        {
            _.Post
                .Json(registerRequest, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiSlashRegister);
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var registrationResult = await registerResponse.ReadAsJsonAsync<RegisterStepOneResponse>();
        
        // Extract token from confirmation link (simplified for testing)
        var confirmationUrl = registrationResult?.ConfirmationEmailLink;
        Assert.NotNull(confirmationUrl);
        
        // Parse the confirmation parameters
        var uri = new Uri(confirmationUrl.StartsWith("http") ? confirmationUrl : $"https://localhost{confirmationUrl}");
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        
        var confirmRequest = new ConfirmEmailRequest
        {
            UserId = query["userId"],
            Token = query["token"],
            Email = query["email"]
        };

        var confirmResponse = await Host.Scenario(_ =>
        {
            _.Get.Url($"/api/confirm_email/?userId={confirmRequest.UserId}&token={Uri.EscapeDataString(confirmRequest.Token!)}&email={confirmRequest.Email}");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var confirmResult = await confirmResponse.ReadAsJsonAsync<ConfirmEmailResponse>();
        
        Assert.NotNull(confirmResult);
        Assert.Equal("EmailConfirmed", confirmResult.ConfirmationCode);
        Assert.Null(confirmResult.ApiError);
    }

    [Fact]
    public async Task ConfirmEmail_Invalid_Token_Returns_Error()
    {
        var confirmRequest = new ConfirmEmailRequest
        {
            UserId = "invalid-user-id",
            Token = "invalid-token",
            Email = "invalid@example.com"
        };

        await Host.Scenario(_ =>
        {
            _.Get.Url($"/api/confirm_email/?userId={confirmRequest.UserId}&token={confirmRequest.Token}&email={confirmRequest.Email}");
            _.StatusCodeShouldBe(HttpStatusCode.NotFound);
        });
    }

    [Fact]
    public async Task RegisterStepTwo_Success()
    {
        // First register a user
        var registerRequest = RegistrationTestFactory.CreateValidRegisterRequest("steptwo@example.com");
        
        var registerResponse = await Host.Scenario(_ =>
        {
            _.Post
                .Json(registerRequest, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiSlashRegister);
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var registrationResult = await registerResponse.ReadAsJsonAsync<RegisterStepOneResponse>();
        
        // Extract token from confirmation link
        var confirmationUrl = registrationResult?.ConfirmationEmailLink;
        Assert.NotNull(confirmationUrl);
        
        var uri = new Uri(confirmationUrl.StartsWith("http") ? confirmationUrl : $"https://localhost{confirmationUrl}");
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        
        var stepTwoRequest = new RegisterStepTwoRequest
        {
            UserId = query["userId"]!,
            Token = query["token"]!,
            Email = query["email"]!
        };

        var stepTwoResponse = await Host.Scenario(_ =>
        {
            _.Post
                .Json(stepTwoRequest, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiSlashRegisterStepTwo);
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var stepTwoResult = await stepTwoResponse.ReadAsJsonAsync<RegisterStepTwoResponse>();
        
        Assert.NotNull(stepTwoResult);
        Assert.True(stepTwoResult.RegistrationStepTwo);
        Assert.Equal("RegistrationComplete", stepTwoResult.ConfirmationCode);
        Assert.Null(stepTwoResult.ApiError);
    }

    [Fact]
    public async Task RegisterStepTwo_Invalid_User_Returns_NotFound()
    {
        var stepTwoRequest = RegistrationTestFactory.CreateRegisterStepTwoRequest("invalid-user-id", "invalid-token", "invalid@example.com");

        await Host.Scenario(_ =>
        {
            _.Post
                .Json(stepTwoRequest, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiSlashRegisterStepTwo);
            _.StatusCodeShouldBe(HttpStatusCode.NotFound);
        });
    }

    [Fact]
    public async Task FullRegistrationWorkflow_Success()
    {
        var registerRequest = RegistrationTestFactory.CreateValidRegisterRequest("fullworkflow@example.com");
        
        // Step 1: Initial Registration
        var registerResponse = await Host.Scenario(_ =>
        {
            _.Post
                .Json(registerRequest, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiSlashRegister);
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var registrationResult = await registerResponse.ReadAsJsonAsync<RegisterStepOneResponse>();
        Assert.NotNull(registrationResult);
        Assert.True(registrationResult.RegistrationStepOne);

        // Step 2: Email Confirmation
        var confirmationUrl = registrationResult.ConfirmationEmailLink;
        Assert.NotNull(confirmationUrl);
        
        var uri = new Uri(confirmationUrl.StartsWith("http") ? confirmationUrl : $"https://localhost{confirmationUrl}");
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

        var confirmResponse = await Host.Scenario(_ =>
        {
            _.Get.Url($"/api/confirm_email/?userId={query["userId"]}&token={Uri.EscapeDataString(query["token"]!)}&email={query["email"]}");
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var confirmResult = await confirmResponse.ReadAsJsonAsync<ConfirmEmailResponse>();
        Assert.NotNull(confirmResult);
        Assert.Equal("EmailConfirmed", confirmResult.ConfirmationCode);

        // Step 3: Registration Step Two
        var stepTwoRequest = new RegisterStepTwoRequest
        {
            UserId = query["userId"]!,
            Token = query["token"]!,
            Email = query["email"]!
        };

        var stepTwoResponse = await Host.Scenario(_ =>
        {
            _.Post
                .Json(stepTwoRequest, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiSlashRegisterStepTwo);
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var stepTwoResult = await stepTwoResponse.ReadAsJsonAsync<RegisterStepTwoResponse>();
        Assert.NotNull(stepTwoResult);
        Assert.True(stepTwoResult.RegistrationStepTwo);
        Assert.Equal("RegistrationComplete", stepTwoResult.ConfirmationCode);
    }
}
