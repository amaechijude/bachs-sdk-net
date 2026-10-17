using System.Net;
using System.Net.Http.Headers;
using Bachs.Exceptions;

namespace Bachs.Common;

/// <summary>
/// Represents a complete HTTP response from the Bachs API, including data and metadata.
/// </summary>
/// <typeparam name="T">The type of the parsed response data.</typeparam>
public sealed record BachsResponse<T>
{
    /// <summary>
    /// Gets the parsed response data.
    /// </summary>
    public T Data { get; private init; } = default!;

    /// <summary>
    /// Gets the parsed API error when the response represents an unsuccessful request.
    /// </summary>
    public BachsError? Error { get; private init; }

    /// <summary>
    /// Gets the HTTP status code returned by the API.
    /// </summary>
    public HttpStatusCode StatusCode { get; private init; }

    /// <summary>
    /// Gets whether the HTTP status code indicates success (2xx).
    /// </summary>
    public bool IsSuccessStatusCode => (int)StatusCode is >= 200 and <= 299;

    private BachsResponse(T data, HttpStatusCode statusCode)
    {
        Data = data;
        StatusCode = statusCode;
    }

    private BachsResponse(BachsError? error, HttpStatusCode statusCode)
    {
        Error = error;
        StatusCode = statusCode;
    }

    public static BachsResponse<T> Success<TResponse>(T response, HttpStatusCode httpStatusCode) =>
        new(response, httpStatusCode);

    public static BachsResponse<T> Failure(BachsError? errorBody, HttpStatusCode statusCode) =>
        new(errorBody, statusCode);
}
