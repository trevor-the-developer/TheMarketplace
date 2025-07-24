using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Marketplace.Core.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Marketplace.Core.Helpers;

public static class TokenHelper
{
    private const string Authorization = "Authorization";
    private const string Bearer = "Bearer";
    private const string Expiration = "Expiration";
    private const string SystemTime = "System time";

    /// <summary>
    ///     Logs JWT Authentication attempts to the console.
    /// </summary>
    /// <param name="headers">HttpRequest & response headers.</param>
    /// <param name="eventType">The type of event that occurred.</param>
    /// <returns>The completed <see cref="Task" /></returns>
    public static Task LogAttempt(IHeaderDictionary headers, string eventType)
    {
        using var loggerFactory = LoggerFactory.Create(b => b.SetMinimumLevel(LogLevel.Trace).AddConsole());

        var logger = loggerFactory.CreateLogger(nameof(TokenHelper));

        var authorizationHeader = headers[Authorization].FirstOrDefault();

        if (authorizationHeader is null)
        {
            logger.LogInformation($"{eventType}. {AuthConstants.JwtNotPresent}");
        }
        else
        {
            var jwtString = authorizationHeader[$"{Bearer} ".Length..];

            if (!jwtString.Contains(".") || jwtString.Split(".").Length != 3)
            {
                logger.LogWarning("Invalid JWT format");
                return Task.CompletedTask;
            }

            var jwt = new JwtSecurityToken(jwtString);

            logger.LogInformation($"{eventType}. {Expiration}: {jwt.ValidTo.ToLongTimeString()}. " +
                                  $"{SystemTime}: {DateTime.UtcNow.ToLongTimeString()}");
        }

        return Task.CompletedTask;
    }
}