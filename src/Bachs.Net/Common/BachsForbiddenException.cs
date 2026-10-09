using System.Net;

namespace Bachs.Net.Common;

/// <summary>
/// Exception thrown when an authenticated API key lacks the required permissions/scopes (HTTP 403).
/// </summary>
public class BachsForbiddenException : BachsApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BachsForbiddenException"/> class.
    /// </summary>
    public BachsForbiddenException(
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
