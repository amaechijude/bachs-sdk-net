using System.Net;

namespace Bachs.Net.Common;

/// <summary>
/// Exception thrown when the API request exceeds rate limits (HTTP 429 Too Many Requests).
/// </summary>
public class BachsRateLimitException : BachsApiException
{
    /// <summary>
    /// Gets the duration to wait before retrying the request, parsed from the 'Retry-After' header.
    /// </summary>
    public TimeSpan? RetryAfter => RateLimit?.RetryAfter;

    /// <summary>
    /// Initializes a new instance of the <see cref="BachsRateLimitException"/> class.
    /// </summary>
    public BachsRateLimitException(
        HttpStatusCode statusCode,
        string errorCode,
        string detail,
        string? docUrl = null,
        BachsError? error = null,
        RateLimitInfo? rateLimit = null,
        string? rawResponse = null)
        : base(statusCode, errorCode, detail, docUrl, error, rateLimit, rawResponse)
    {
    }
}
