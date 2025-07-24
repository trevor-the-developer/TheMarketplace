using System.Collections.Generic;

namespace Marketplace.Core;

/// <summary>
///     Represents the result of an operation that can succeed or fail
/// </summary>
public class Result<T>
{
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

    public bool IsSuccess { get; private set; }
    public T? Data { get; private set; }
    public ApiError? Error { get; private set; }

    // Success factory methods
    public static Result<T> Success(T data)
    {
        return new Result<T>(data);
    }

    // Failure factory methods
    public static Result<T> Failure(ApiError error)
    {
        return new Result<T>(error);
    }

    public static Result<T> Failure(string message)
    {
        return new Result<T>(ApiError.BadRequest(message));
    }

    public static Result<T> Failure(string message, int statusCode)
    {
        return new Result<T>(new ApiError(
            statusCode.ToString(),
            statusCode,
            message,
            null
        ));
    }

    public static Result<T> Failure(string message, int statusCode, string? details)
    {
        return new Result<T>(new ApiError(
            statusCode.ToString(),
            statusCode,
            message,
            details
        ));
    }

    public static Result<T> ValidationFailure(string message, List<string> validationErrors)
    {
        return new Result<T>(ApiError.BadRequest(message, validationErrors));
    }

    public static Result<T> NotFound(string message)
    {
        return new Result<T>(ApiError.NotFound(message));
    }

    public static Result<T> Unauthorized(string message)
    {
        return new Result<T>(ApiError.Unauthorized(message));
    }

    // Implicit conversions for convenience
    public static implicit operator Result<T>(T data)
    {
        return Success(data);
    }

    public static implicit operator Result<T>(ApiError error)
    {
        return Failure(error);
    }
}

/// <summary>
///     Result for operations that don't return data
/// </summary>
public class Result
{
    private Result(bool isSuccess, ApiError? error = null)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; private set; }
    public ApiError? Error { get; private set; }

    public static Result Success()
    {
        return new Result(true);
    }

    public static Result Failure(ApiError error)
    {
        return new Result(false, error);
    }

    public static Result Failure(string message)
    {
        return new Result(false, ApiError.BadRequest(message));
    }
}