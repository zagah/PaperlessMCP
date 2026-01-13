using System.Net;

namespace PaperlessMCP.Client;

/// <summary>
/// Represents the result of an API operation that can either succeed or fail with details.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
public readonly record struct ApiResult<T>
{
    public bool IsSuccess { get; }
    public T? Value { get; }
    public ApiError? Error { get; }

    private ApiResult(bool isSuccess, T? value, ApiError? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static ApiResult<T> Success(T value) => new(true, value, null);

    public static ApiResult<T> Failure(HttpStatusCode statusCode, string message, string? responseBody = null) =>
        new(false, default, new ApiError(statusCode, message, responseBody));

    public static ApiResult<T> Failure(ApiError error) => new(false, default, error);

    public static implicit operator bool(ApiResult<T> result) => result.IsSuccess;
}

/// <summary>
/// Details about an API error.
/// </summary>
public record ApiError(HttpStatusCode StatusCode, string Message, string? ResponseBody)
{
    public override string ToString() =>
        $"HTTP {(int)StatusCode} {StatusCode}: {Message}" +
        (ResponseBody != null ? $" - {ResponseBody}" : "");
}
