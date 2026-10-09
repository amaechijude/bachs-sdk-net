using System.Net;

namespace Bachs.Net.Common;

/// <summary>
/// Exception thrown when authentication fails due to a missing or invalid API key (HTTP 401).
/// </summary>
public class BachsAuthenticationException : BachsApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BachsAuthenticationException"/> class.
    /// </summary>
    public BachsAuthenticationException(
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
