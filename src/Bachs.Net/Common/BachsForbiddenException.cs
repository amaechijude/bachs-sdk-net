using System.Net;

namespace Bachs.Net.Common;

/// <summary>
/// Exception thrown when an authenticated API key lacks the required permissions/scopes (HTTP 403).
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BachsForbiddenException"/> class.
/// </remarks>
public sealed class BachsForbiddenException(
    HttpStatusCode statusCode,
    string errorCode,
    string detail,
    string? docUrl = null,
    BachsError? error = null,
    BachsRateLimitInfo? rateLimit = null,
    string? rawResponse = null
) : BachsApiException(statusCode, errorCode, detail, docUrl, error, rateLimit, rawResponse);
