using System.Net;

namespace Bachs.Net.Common;

/// <summary>
/// Exception thrown when a request fails validation (HTTP 400 or 422).
/// Contains field-level validation errors when returned by the Bachs API.
/// </summary>
public class BachsValidationException : BachsApiException
{
    /// <summary>
    /// Gets the list of field-level validation errors, if provided by the API.
    /// </summary>
    public IReadOnlyList<BachsValidationError> ValidationErrors =>
        Error?.Errors ?? Array.Empty<BachsValidationError>();

    /// <summary>
    /// Initializes a new instance of the <see cref="BachsValidationException"/> class.
    /// </summary>
    public BachsValidationException(
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
