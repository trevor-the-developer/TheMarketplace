using System.Net;
using System.Threading.Tasks;
using Alba;
using Marketplace.Api.Endpoints.Authentication.Login;
using Marketplace.Core.Constants;
using Marketplace.Core.Models;
using Marketplace.Core.Models.Login;
using Marketplace.Test.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Marketplace.Test.Scenarios.Authentication.IntegrationTests.Authentication;

[Collection("scenarios")]
public class LoginTests(WebAppFixture fixture) : ScenarioContext(fixture), IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await DatabaseResetService.ResetDatabaseAsync();
    }

    public async Task DisposeAsync()
    {
        await Task.CompletedTask;
    }

    [Fact]
    public async Task Login_Success()
    {
        var response = await Host.Scenario(_ =>
        {
            // This serializes the Input object to json,
            // writes it to the HttpRequest.Body, and sets
            // the accepts & content-type header values to
            // application/json
            _.Post
                .Json(new { Email = "admin@localhost", Password = "P@ssw0rd!" }, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiSlashLogin);
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });

        var result = await response.ReadAsJsonAsync<LoginResponse>();

        Assert.True(result?.Succeeded);
        Assert.NotNull(result?.SecurityToken);
        Assert.NotNull(result.Expiration);
        Assert.NotNull(result.RefreshToken);
        Assert.Null(result.ApiError);
    }

    [Fact]
    public async Task Login_Failure()
    {
        var response = await Host.Scenario(_ =>
        {
            _.Post
                .Json(new { Email = "wrong@localhost", Password = "Wr0ngP4ss1!" }, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiSlashLogin);
            _.StatusCodeShouldBe(HttpStatusCode.Unauthorized);
        });

        var result = await response.ReadAsJsonAsync<ProblemDetails>();

        Assert.NotNull(result);
        Assert.IsType<ProblemDetails>(result);
        Assert.False(string.IsNullOrEmpty(result.Detail));
        Assert.False(string.IsNullOrEmpty(result.Title));
        Assert.False(string.IsNullOrEmpty(result.Type));
        Assert.NotNull(result.Status);
        Assert.IsType<int>(result.Status);
        Assert.True(result.Status == (int?)HttpStatusCode.Unauthorized);
        Assert.True(result.Title == "Login endpoint.");
    }
}