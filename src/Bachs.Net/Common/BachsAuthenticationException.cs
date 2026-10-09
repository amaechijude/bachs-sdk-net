using System.Net;

namespace Bachs.Net.Common;

/// <summary>
/// Exception thrown when authentication fails due to a missing or invalid API key (HTTP 401).
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BachsAuthenticationException"/> class.
/// </remarks>
public sealed class BachsAuthenticationException(
    HttpStatusCode statusCode,
    string errorCode,
    string detail,
    string? docUrl = null,
    BachsError? error = null,
    RateLimitInfo? rateLimit = null,
    string? rawResponse = null
) : BachsApiException(statusCode, errorCode, detail, docUrl, error, rateLimit, rawResponse);
