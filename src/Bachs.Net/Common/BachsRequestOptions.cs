namespace Bachs.Net.Common;

/// <summary>
/// Options that can be configured per request to the Bachs API.
/// </summary>
public class BachsRequestOptions
{
    /// <summary>
    /// Gets or sets the Apikey for specific operations.
    /// When set, it will override the default api for the request request
    /// </summary>
    public string? ScopedApiKey { get; set; }

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
    /// Creates a new instance of <see cref="BachsRequestOptions"/>.
    /// </summary>
    public BachsRequestOptions() { }

    /// <summary>
    /// Creates a new instance of <see cref="BachsRequestOptions"/> with the specified idempotency key.
    /// </summary>
    /// <param name="idempotencyKey">The idempotency key.</param>
    public BachsRequestOptions(string idempotencyKey)
    {
        IdempotencyKey = idempotencyKey;
    }

    /// <summary>
    /// Sets the idempotency key and returns this instance.
    /// </summary>
    public BachsRequestOptions WithIdempotencyKey(string idempotencyKey)
    {
        IdempotencyKey = idempotencyKey;
        return this;
    }

    /// <summary>
    /// Sets the connected account ID and returns this instance.
    /// </summary>
    public BachsRequestOptions WithAccountId(string accountId)
    {
        AccountId = accountId;
        return this;
    }

    /// <summary>
    /// Adds a custom header and returns this instance.
    /// </summary>
    public BachsRequestOptions WithHeader(string key, string value)
    {
        CustomHeaders ??= new Dictionary<string, string>();
        CustomHeaders[key] = value;
        return this;
    }
}
