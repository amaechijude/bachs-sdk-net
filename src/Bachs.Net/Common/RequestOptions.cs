namespace Bachs.Net.Common;

/// <summary>
/// Options that can be configured per request to the Bachs API.
/// </summary>
public class RequestOptions
{
    /// <summary>
    /// Gets or sets an idempotency key sent via the 'Idempotency-Key' header on POST and PATCH requests.
    /// Retrying the same request with the same idempotency key returns the cached successful response.
    /// </summary>
    public string? IdempotencyKey { get; set; }

    /// <summary>
    /// Gets or sets the connected account ID sent via the 'X-Account-Id' header.
    /// Use this when acting on behalf of a connected account.
    /// </summary>
    public string? AccountId { get; set; }

    /// <summary>
    /// Gets or sets custom HTTP headers to include with the request.
    /// </summary>
    public IDictionary<string, string>? CustomHeaders { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="RequestOptions"/>.
    /// </summary>
    public RequestOptions() { }

    /// <summary>
    /// Creates a new instance of <see cref="RequestOptions"/> with the specified idempotency key.
    /// </summary>
    /// <param name="idempotencyKey">The idempotency key.</param>
    public RequestOptions(string idempotencyKey)
    {
        IdempotencyKey = idempotencyKey;
    }

    /// <summary>
    /// Sets the idempotency key and returns this instance.
    /// </summary>
    public RequestOptions WithIdempotencyKey(string idempotencyKey)
    {
        IdempotencyKey = idempotencyKey;
        return this;
    }

    /// <summary>
    /// Sets the connected account ID and returns this instance.
    /// </summary>
    public RequestOptions WithAccountId(string accountId)
    {
        AccountId = accountId;
        return this;
    }

    /// <summary>
    /// Adds a custom header and returns this instance.
    /// </summary>
    public RequestOptions WithHeader(string key, string value)
    {
        CustomHeaders ??= new Dictionary<string, string>();
        CustomHeaders[key] = value;
        return this;
    }
}
