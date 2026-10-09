using System.Net;

namespace Bachs.Net.Common;

/// <summary>
/// Exception thrown when the Bachs API returns an error response (4xx or 5xx).
/// </summary>
public class BachsApiException : BachsException
{
    /// <summary>
    /// Gets the HTTP status code returned by the Bachs API.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Gets the machine-readable error code returned by the API (e.g., 'VALIDATION_ERROR', 'NOT_FOUND').
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Gets the human-readable explanation of the error.
    /// </summary>
    public string Detail { get; }

    /// <summary>
    /// Gets the documentation link for this error, if provided by the API.
    /// </summary>
    public string? DocUrl { get; }

    /// <summary>
    /// Gets the parsed Bachs error response object.
    /// </summary>
    public BachsError? Error { get; }

    /// <summary>
    /// Gets rate limiting information associated with the failed request, if present.
    /// </summary>
    public RateLimitInfo? RateLimit { get; }

    /// <summary>
    /// Gets the raw JSON response payload returned by the server.
    /// </summary>
    public string? RawResponse { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BachsApiException"/> class.
    /// </summary>
    public BachsApiException(
        HttpStatusCode statusCode,
        string errorCode,
        string detail,
        string? docUrl = null,
        BachsError? error = null,
        RateLimitInfo? rateLimit = null,
        string? rawResponse = null)
        : base(FormatMessage(statusCode, errorCode, detail))
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        Detail = detail;
        DocUrl = docUrl;
        Error = error;
        RateLimit = rateLimit;
        RawResponse = rawResponse;
    }

    private static string FormatMessage(HttpStatusCode statusCode, string errorCode, string detail)
    {
        return !string.IsNullOrWhiteSpace(errorCode)
            ? $"Bachs API error ({(int)statusCode} {statusCode}) [{errorCode}]: {detail}"
            : $"Bachs API error ({(int)statusCode} {statusCode}): {detail}";
    }

    /// <summary>
    /// Creates the most specific <see cref="BachsApiException"/> subtype for the given HTTP status and error details.
    /// </summary>
    public static BachsApiException Create(
        HttpStatusCode statusCode,
        BachsError? error,
        RateLimitInfo? rateLimit,
        string? rawResponse)
    {
        var errorCode = error?.ErrorCode ?? statusCode.ToString().ToUpperInvariant();
        var detail = error?.Detail ?? $"Request failed with HTTP status code {(int)statusCode} ({statusCode}).";
        var docUrl = error?.DocUrl;

        return statusCode switch
        {
            HttpStatusCode.BadRequest when errorCode == "VALIDATION_ERROR" || (error?.Errors != null && error.Errors.Count > 0) =>
                new BachsValidationException(statusCode, errorCode, detail, docUrl, error, rateLimit, rawResponse),

            HttpStatusCode.UnprocessableEntity =>
                new BachsValidationException(statusCode, errorCode, detail, docUrl, error, rateLimit, rawResponse),

            HttpStatusCode.Unauthorized =>
                new BachsAuthenticationException(statusCode, errorCode, detail, docUrl, error, rateLimit, rawResponse),

            HttpStatusCode.Forbidden =>
                new BachsForbiddenException(statusCode, errorCode, detail, docUrl, error, rateLimit, rawResponse),

            HttpStatusCode.NotFound =>
                new BachsNotFoundException(statusCode, errorCode, detail, docUrl, error, rateLimit, rawResponse),

            HttpStatusCode.Conflict =>
                new BachsConflictException(statusCode, errorCode, detail, docUrl, error, rateLimit, rawResponse),

            HttpStatusCode.TooManyRequests =>
                new BachsRateLimitException(statusCode, errorCode, detail, docUrl, error, rateLimit, rawResponse),

            _ => new BachsApiException(statusCode, errorCode, detail, docUrl, error, rateLimit, rawResponse)
        };
    }
}
