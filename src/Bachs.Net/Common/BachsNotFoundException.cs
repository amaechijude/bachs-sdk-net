using System.Net;

namespace Bachs.Net.Common;

/// <summary>
/// Exception thrown when a requested resource was not found (HTTP 404).
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BachsNotFoundException"/> class.
/// </remarks>
public sealed class BachsNotFoundException(
    HttpStatusCode statusCode,
    string errorCode,
    string detail,
    string? docUrl = null,
    BachsError? error = null,
    BachsRateLimitInfo? rateLimit = null,
    string? rawResponse = null
) : BachsApiException(statusCode, errorCode, detail, docUrl, error, rateLimit, rawResponse) { }
