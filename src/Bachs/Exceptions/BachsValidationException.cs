using System.Net;
using Bachs.Common;

namespace Bachs.Exceptions;

/// <summary>
/// Exception thrown when a request fails validation (HTTP 400 or 422).
/// Contains field-level validation errors when returned by the Bachs API.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="BachsValidationException"/> class.
/// </remarks>
public sealed class BachsValidationException(
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
    /// Gets the list of field-level validation errors, if provided by the API.
    /// </summary>
    public IReadOnlyList<BachsValidationError> ValidationErrors =>
        Error?.Errors ?? Array.Empty<BachsValidationError>();
}
