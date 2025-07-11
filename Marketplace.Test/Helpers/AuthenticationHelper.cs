using System.Net;
using Alba;
using Marketplace.Api.Endpoints.Authentication.Login;
using Marketplace.Core.Constants;
using Newtonsoft.Json;

namespace Marketplace.Test.Helpers;

public static class AuthenticationHelper
{
    public static async Task<string> GetAdminTokenAsync(IAlbaHost host)
    {
        var loginResponse = await host.Scenario(_ =>
        {
            _.Post
                .Json(new { Email = "admin@localhost", Password = "P@ssw0rd!" }, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiSlashLogin);
        });

        var loginResult = await loginResponse.ReadAsJsonAsync<LoginResponse>();

        if (loginResult?.SecurityToken == null) throw new InvalidOperationException("Failed to get admin token");

        return loginResult.SecurityToken;
    }

    public static async Task<LoginResponse> GetLoginResponse(IAlbaHost host)
    {
        var loginResponse = await host.Scenario(_ =>
        {
            _.Post
                .Json(new { Email = "admin@localhost", Password = "P@ssw0rd!" }, JsonStyle.MinimalApi)
                .ToUrl(ApiConstants.ApiSlashLogin);
            _.StatusCodeShouldBe(HttpStatusCode.OK);
        });


        var jsonString = await loginResponse.ReadAsTextAsync();
        var loginResult = JsonConvert.DeserializeObject<LoginResponse>(jsonString);

        if (loginResult == null) throw new InvalidOperationException("Failed to get login response");

        return loginResult;
    }
}