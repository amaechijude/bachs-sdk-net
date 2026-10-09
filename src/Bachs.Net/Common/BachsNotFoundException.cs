using System.Net;

namespace Bachs.Net.Common;

/// <summary>
/// Exception thrown when a requested resource was not found (HTTP 404).
/// </summary>
public class BachsNotFoundException : BachsApiException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BachsNotFoundException"/> class.
    /// </summary>
    public BachsNotFoundException(
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
