using Microsoft.AspNetCore.Http;

namespace Marketplace.Core
{
    public record ApiError(string? HttpStatusCode, int? StatusCode, string? ErrorMessage, string? StackTrace)
    {
        public List<string>? ValidationErrors { get; init; }
        
        // Helper methods for common scenarios
        public static ApiError Unauthorized(string message) => new(
            HttpStatusCode: StatusCodes.Status401Unauthorized.ToString(),
            StatusCode: StatusCodes.Status401Unauthorized,
            ErrorMessage: message,
            StackTrace: null
        );
        
        public static ApiError BadRequest(string message, List<string>? validationErrors = null) => new(
            HttpStatusCode: StatusCodes.Status400BadRequest.ToString(),
            StatusCode: StatusCodes.Status400BadRequest,
            ErrorMessage: message,
            StackTrace: null
        ) { ValidationErrors = validationErrors };
        
        public static ApiError NotFound(string message) => new(
            HttpStatusCode: StatusCodes.Status404NotFound.ToString(),
            StatusCode: StatusCodes.Status404NotFound,
            ErrorMessage: message,
            StackTrace: null
        );
        
        public static ApiError InternalServerError(string message, string? stackTrace = null) => new(
            HttpStatusCode: StatusCodes.Status500InternalServerError.ToString(),
            StatusCode: StatusCodes.Status500InternalServerError,
            ErrorMessage: message,
            StackTrace: stackTrace
        );
    }
}
