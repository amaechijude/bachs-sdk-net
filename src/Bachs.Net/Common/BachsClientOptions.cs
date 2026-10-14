namespace Bachs.Net.Common;

/// <summary>
/// Configuration options for the Bachs API client.
/// </summary>
public sealed class BachsClientOptions
{
    /// <summary>
    /// Default production base URL.
    /// </summary>
    public const string DefaultProductionBaseUrl = "https://api.bachs.io";

    /// <summary>
    /// Default sandbox base URL.
    /// </summary>
    public const string DefaultSandboxBaseUrl = "https://sandbox-api.bachs.io";

    /// <summary>
    /// Gets or sets the Bachs secret API key (starts with sk_sandbox_ or sk_live_).
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets an explicit base URL to override standard sandbox or production URLs.
    /// Useful for testing, proxies, or mock servers.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the default connected account ID (X-Account-Id header)
    /// to act on behalf of across requests.
    /// </summary>
    public string? AccountId { get; set; }

    /// <summary>
    /// Gets or sets the default webhook signing secret used for verifying incoming webhook events.
    /// </summary>
    public string? WebhookSecret { get; set; }

    /// <summary>
    /// Gets or sets the HTTP request timeout. Defaults to 30 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the maximum number of retries for transient failures and rate limits.
    /// Defaults to 0.
    /// <para>
    /// Note: If you are registering this client via Dependency Injection and using a standard resilience
    /// pipeline (like Polly or <c>AddStandardResilienceHandler</c>), leave this as 0 to prevent nested
    /// exponential retries. For standalone usage without DI, you can set this to a positive integer
    /// (e.g., 2) to enable built-in transient error handling.
    /// </para>
    /// </summary>
    public int MaxRetries { get; set; } = 0;

    /// <summary>
    /// Gets or sets the maximum delay to wait when retrying rate-limited requests.
    /// Defaults to 30 seconds.
    /// <para>
    /// Note: This setting only applies if <see cref="MaxRetries"/> is greater than 0 (i.e., when using the built-in retry mechanism).
    /// If you are using standard DI resilience handlers, delay configuration should be handled by your external policies.
    /// </para>
    /// </summary>
    public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets whether to throw typed exceptions (<see cref="BachsApiException"/>) on HTTP error status codes.
    /// Defaults to false, so API failures are returned as unsuccessful <see cref="BachsResponse{T}"/> results.
    /// </summary>
    public bool ThrowOnApiErrors { get; set; }

    /// <summary>
    /// Resolves the effective base URL based on <see cref="BaseUrl"/>, <see cref="Environment"/>,
    /// and the <see cref="ApiKey"/> prefix.
    /// </summary>
    /// <returns>The resolved base <see cref="Uri"/>.</returns>
    public Uri GetEffectiveBaseUrl()
    {
        if (!string.IsNullOrWhiteSpace(BaseUrl))
            return new Uri(BaseUrl.TrimEnd('/') + "/");

        // Infer from API Key prefix
        if (!string.IsNullOrWhiteSpace(ApiKey))
        {
            if (ApiKey.StartsWith("sk_live_", StringComparison.OrdinalIgnoreCase))
                return new Uri(DefaultProductionBaseUrl + "/");

            if (ApiKey.StartsWith("sk_sandbox_", StringComparison.OrdinalIgnoreCase))
                return new Uri(DefaultSandboxBaseUrl + "/");
        }

        // Default to sandbox for safety
        return new Uri(DefaultSandboxBaseUrl + "/");
    }

    /// <summary>
    /// Validates the configuration options.
    /// </summary>
    /// <exception cref="BachsInvalidOperationException">Thrown if required settings are missing or invalid.</exception>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ApiKey))
            throw new BachsInvalidOperationException(
                "A Bachs API key is required. Provide a secret key starting with 'sk_sandbox_' or 'sk_live_'."
            );

        if (MaxRetries < 0)
            throw new BachsInvalidOperationException("MaxRetries cannot be negative.");
    }
}
