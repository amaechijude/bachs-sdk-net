using System.Net;
using System.Net.Http.Headers;

namespace Bachs.Net.Common;

/// <summary>
/// Represents a complete HTTP response from the Bachs API, including data and metadata.
/// </summary>
/// <typeparam name="T">The type of the parsed response data.</typeparam>
public sealed record BachsResponse<T>
{
    /// <summary>
    /// Gets the parsed response data.
    /// </summary>
    public T Data { get; init; } = default!;

    /// <summary>
    /// Gets the parsed API error when the response represents an unsuccessful request.
    /// </summary>
    public BachsError? Error { get; init; }

    /// <summary>
    /// Gets the HTTP status code returned by the API.
    /// </summary>
    public HttpStatusCode StatusCode { get; init; }

    /// <summary>
    /// Gets whether the HTTP status code indicates success (2xx).
    /// </summary>
    public bool IsSuccessStatusCode => (int)StatusCode >= 200 && (int)StatusCode <= 299;

    /// <summary>
    /// Gets rate limiting information parsed from the response headers, if present.
    /// </summary>
    public BachsRateLimitInfo? RateLimit { get; init; }

    /// <summary>
    /// Gets the response headers.
    /// </summary>
    public HttpResponseHeaders? Headers { get; init; }

    /// <summary>
    /// Gets the raw JSON response body.
    /// </summary>
    public string? RawJson { get; init; }
}
