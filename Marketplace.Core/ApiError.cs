using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace Marketplace.Core;

public record ApiError(string? HttpStatusCode, int? StatusCode, string? ErrorMessage, string? StackTrace)
{
    public List<string>? ValidationErrors { get; init; }

    // Helper methods for common scenarios
    public static ApiError Unauthorized(string message)
    {
        return new ApiError(
            StatusCodes.Status401Unauthorized.ToString(),
            StatusCodes.Status401Unauthorized,
            message,
            null
        );
    }

    public static ApiError BadRequest(string message, List<string>? validationErrors = null)
    {
        return new ApiError(
            StatusCodes.Status400BadRequest.ToString(),
            StatusCodes.Status400BadRequest,
            message,
            null
        ) { ValidationErrors = validationErrors };
    }

    public static ApiError NotFound(string message)
    {
        return new ApiError(
            StatusCodes.Status404NotFound.ToString(),
            StatusCodes.Status404NotFound,
            message,
            null
        );
    }

    public static ApiError InternalServerError(string message, string? stackTrace = null)
    {
        return new ApiError(
            StatusCodes.Status500InternalServerError.ToString(),
            StatusCodes.Status500InternalServerError,
            message,
            stackTrace
        );
    }
}