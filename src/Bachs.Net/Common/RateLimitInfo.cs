using System.Net.Http.Headers;

namespace Bachs.Net.Common;

/// <summary>
/// Contains rate limiting information extracted from Bachs API response headers.
/// </summary>
public sealed record RateLimitInfo
{
    /// <summary>
    /// Gets the maximum requests allowed in the current time window (from X-RateLimit-Limit).
    /// </summary>
    public int? Limit { get; init; }

    /// <summary>
    /// Gets the number of requests remaining in the current window (from X-RateLimit-Remaining).
    /// </summary>
    public int? Remaining { get; init; }

    /// <summary>
    /// Gets the UTC reset timestamp for the current rate limit window (from X-RateLimit-Reset).
    /// </summary>
    public DateTimeOffset? Reset { get; init; }

    /// <summary>
    /// Gets the duration to wait before retrying on a 429 response (from Retry-After).
    /// </summary>
    public TimeSpan? RetryAfter { get; init; }

    /// <summary>
    /// Parses rate limit headers from HTTP response headers.
    /// </summary>
    /// <param name="headers">The response headers.</param>
    /// <returns>A populated <see cref="RateLimitInfo"/> instance.</returns>
    public static RateLimitInfo FromHeaders(HttpResponseHeaders headers)
    {
        int? limit = null;
        if (
            headers.TryGetValues("X-RateLimit-Limit", out var limitValues)
            && int.TryParse(limitValues.FirstOrDefault(), out var parsedLimit)
        )
        {
            limit = parsedLimit;
        }

        int? remaining = null;
        if (
            headers.TryGetValues("X-RateLimit-Remaining", out var remainingValues)
            && int.TryParse(remainingValues.FirstOrDefault(), out var parsedRemaining)
        )
        {
            remaining = parsedRemaining;
        }

        DateTimeOffset? reset = null;
        if (
            headers.TryGetValues("X-RateLimit-Reset", out var resetValues)
            && long.TryParse(resetValues.FirstOrDefault(), out var resetSeconds)
        )
        {
            reset = DateTimeOffset.FromUnixTimeSeconds(resetSeconds);
        }

        TimeSpan? retryAfter = null;
        if (headers.RetryAfter != null)
        {
            if (headers.RetryAfter.Delta.HasValue)
            {
                retryAfter = headers.RetryAfter.Delta.Value;
            }
            else if (headers.RetryAfter.Date.HasValue)
            {
                var delta = headers.RetryAfter.Date.Value - DateTimeOffset.UtcNow;
                retryAfter = delta > TimeSpan.Zero ? delta : TimeSpan.Zero;
            }
        }
        else if (
            headers.TryGetValues("Retry-After", out var retryAfterValues)
            && int.TryParse(retryAfterValues.FirstOrDefault(), out var seconds)
        )
        {
            retryAfter = TimeSpan.FromSeconds(seconds);
        }

        return new RateLimitInfo
        {
            Limit = limit,
            Remaining = remaining,
            Reset = reset,
            RetryAfter = retryAfter,
        };
    }
}
