using Microsoft.AspNetCore.Http;

namespace Marketplace.Core
{
    /// <summary>
    /// Represents the result of an operation that can succeed or fail
    /// </summary>
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public ApiError? Error { get; private set; }

        private Result(T data)
        {
            IsSuccess = true;
            Data = data;
            Error = null;
        }

        private Result(ApiError error)
        {
            IsSuccess = false;
            Data = default;
            Error = error;
        }

        // Success factory methods
        public static Result<T> Success(T data) => new(data);

        // Failure factory methods
        public static Result<T> Failure(ApiError error) => new(error);
        
        public static Result<T> Failure(string message) => 
            new(ApiError.BadRequest(message));

        public static Result<T> Failure(string message, int statusCode) => 
            new(new ApiError(
                HttpStatusCode: statusCode.ToString(),
                StatusCode: statusCode,
                ErrorMessage: message,
                StackTrace: null
            ));

        public static Result<T> Failure(string message, int statusCode, string? details) => 
            new(new ApiError(
                HttpStatusCode: statusCode.ToString(),
                StatusCode: statusCode,
                ErrorMessage: message,
                StackTrace: details
            ));

        public static Result<T> ValidationFailure(string message, List<string> validationErrors) => 
            new(ApiError.BadRequest(message, validationErrors));

        public static Result<T> NotFound(string message) => 
            new(ApiError.NotFound(message));

        public static Result<T> Unauthorized(string message) => 
            new(ApiError.Unauthorized(message));

        // Implicit conversions for convenience
        public static implicit operator Result<T>(T data) => Success(data);
        public static implicit operator Result<T>(ApiError error) => Failure(error);

    }

    /// <summary>
    /// Result for operations that don't return data
    /// </summary>
    public class Result
    {
        public bool IsSuccess { get; private set; }
        public ApiError? Error { get; private set; }

        private Result(bool isSuccess, ApiError? error = null)
        {
            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true);
        public static Result Failure(ApiError error) => new(false, error);
        public static Result Failure(string message) => new(false, ApiError.BadRequest(message));

    }
}
