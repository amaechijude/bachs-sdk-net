using System.Net;
using Bachs.Net.Common;

namespace Bachs.Net.Exceptions;

/// <summary>
/// Exception thrown when a request conflicts with current server state or idempotency lock (HTTP 409).
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BachsConflictException"/> class.
/// </remarks>
public sealed class BachsConflictException(
    HttpStatusCode statusCode,
    string errorCode,
    string detail,
    string? docUrl = null,
    BachsError? error = null,
    BachsRateLimitInfo? rateLimit = null,
    string? rawResponse = null
) : BachsApiException(statusCode, errorCode, detail, docUrl, error, rateLimit, rawResponse)
{
    /// <summary>
    /// Gets whether this conflict was caused by a concurrent idempotency request in progress.
    /// </summary>
    public bool IsIdempotencyInProgress =>
        string.Equals(ErrorCode, "IDEMPOTENCY_IN_PROGRESS", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Gets whether this conflict was caused by reusing an idempotency key with a mismatched request payload.
    /// </summary>
    public bool IsIdempotencyConflict =>
        string.Equals(ErrorCode, "IDEMPOTENCY_CONFLICT", StringComparison.OrdinalIgnoreCase);
}
