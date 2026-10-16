using Bachs.Net.Exceptions;

namespace Bachs.Net.Common;

public enum BachsEnvironment
{
    Live,
    Sandbox,
    Custom,
}

/// <summary>
/// Configuration options for the Bachs API client.
/// </summary>
/// <remarks>
/// Resilience settings (timeouts, retries, circuit breaker) apply to clients registered
/// through dependency injection. The defaults are tuned for a typical low-to-moderate
/// traffic consumer and should work without changes for most applications.
/// </remarks>
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

    private const string LiveKeyPrefix = "sk_live_";
    private const string SandboxKeyPrefix = "sk_sandbox_";

    // ------------------------------------------------------------------
    // Connection
    // ------------------------------------------------------------------

    /// <summary>
    /// Gets or sets the Bachs secret API key (starts with <c>sk_sandbox_</c> or <c>sk_live_</c>).
    /// Required.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the target environment.
    /// <list type="bullet">
    /// <item><description><see cref="BachsEnvironment.Live"/>: production API. Requires a <c>sk_live_</c> key.</description></item>
    /// <item><description><see cref="BachsEnvironment.Sandbox"/>: sandbox API. Requires a <c>sk_sandbox_</c> key.</description></item>
    /// <item><description><see cref="BachsEnvironment.Custom"/>: a user-supplied <see cref="BaseUrl"/> (proxies, mock servers, tests). Any key prefix is accepted.</description></item>
    /// </list>
    /// When <see langword="null"/> (the default), the environment is inferred: if
    /// <see cref="BaseUrl"/> is set, otherwise from the <see cref="ApiKey"/> prefix, falling back to
    /// <see cref="BachsEnvironment.Sandbox"/> so an unrecognized key can never hit production.
    /// </summary>
    public BachsEnvironment? Environment { get; set; }

    /// <summary>
    /// Gets or sets an explicit base URL. Required when <see cref="Environment"/> is
    /// <see cref="BachsEnvironment.Custom"/>. When set, it always takes precedence over the
    /// standard live and sandbox URLs. Must be an absolute URI.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the default connected account ID, sent as the <c>X-Account-Id</c> header,
    /// to act on behalf of across requests.
    /// </summary>
    public string? AccountId { get; set; }

    /// <summary>
    /// Gets or sets the default webhook signing secret used to verify incoming webhook events.
    /// </summary>
    public string? WebhookSecret { get; set; }

    // ------------------------------------------------------------------
    // Timeouts
    // ------------------------------------------------------------------

    /// <summary>
    /// Gets or sets the maximum duration of a request including all retries and backoff delays.
    /// Defaults to 30 seconds. Must be greater than zero and at least <see cref="AttemptTimeout"/>.
    /// </summary>
    public TimeSpan TotalRequestTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the maximum duration of each individual HTTP attempt.
    /// Defaults to 10 seconds. Must be greater than zero and no greater than
    /// <see cref="TotalRequestTimeout"/>.
    /// </summary>
    public TimeSpan AttemptTimeout { get; set; } = TimeSpan.FromSeconds(10);

    // ------------------------------------------------------------------
    // Retries
    // ------------------------------------------------------------------

    /// <summary>
    /// Gets or sets the maximum number of retries after the initial attempt for transient failures
    /// (network errors, timeouts, 408, 429, and 5xx responses). Defaults to 3.
    /// Set to 0 to disable retries entirely.
    /// </summary>
    /// <remarks>
    /// With the defaults, the backoff waits total roughly 3.5 seconds (0.5s, 1s, 2s, before jitter),
    /// so the full retry sequence fits comfortably inside <see cref="TotalRequestTimeout"/>.
    /// </remarks>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Gets or sets the initial delay before the first retry. Subsequent delays grow exponentially
    /// (doubling) with random jitter applied. Defaults to 500 milliseconds. Must not be negative.
    /// </summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// Gets or sets the upper bound for any single retry delay, including delays requested by the
    /// server through a <c>Retry-After</c> header on 429 or 503 responses. Defaults to 5 seconds which is bachs default.
    /// If the server asks for a longer wait, the request fails instead of waiting beyond this cap.
    /// Must be at least <see cref="RetryDelay"/>.
    /// </summary>
    public TimeSpan MaxRetryDelay { get; set; } = TimeSpan.FromSeconds(5);

    // ------------------------------------------------------------------
    // Circuit breaker
    // ------------------------------------------------------------------

    /// <summary>
    /// Gets or sets whether the circuit breaker is enabled. Defaults to <see langword="true"/>.
    /// When open, requests fail fast instead of waiting on an unhealthy service.
    /// </summary>
    public bool EnableCircuitBreaker { get; set; } = true;

    /// <summary>
    /// Gets or sets the failure ratio, within the sampling window, at which the circuit opens.
    /// Defaults to 0.5 (50%). Must be greater than 0 and no greater than 1.
    /// </summary>
    public double CircuitBreakerFailureRatio { get; set; } = 0.5;

    /// <summary>
    /// Gets or sets the minimum number of requests within the sampling window before the circuit
    /// can open. Prevents a handful of failures from tripping the breaker. Defaults to 20.
    /// Must be greater than zero.
    /// </summary>
    public int CircuitBreakerMinimumThroughput { get; set; } = 20;

    /// <summary>
    /// Gets or sets the duration of the failure sampling window. Defaults to 30 seconds.
    /// Must be greater than zero.
    /// </summary>
    public TimeSpan CircuitBreakerSamplingDuration { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets how long the circuit stays open before a single probe request is allowed through.
    /// Defaults to 15 seconds. Must be greater than zero.
    /// </summary>
    public TimeSpan CircuitBreakerBreakDuration { get; set; } = TimeSpan.FromSeconds(15);
    public bool EnableRetries => MaxRetries > 0;

    // ------------------------------------------------------------------
    // Resolution and validation
    // ------------------------------------------------------------------

    /// <summary>
    /// Resolves the effective environment from <see cref="Environment"/>, <see cref="BaseUrl"/>,
    /// and the <see cref="ApiKey"/> prefix.
    /// </summary>
    /// <returns>The resolved <see cref="BachsEnvironment"/>.</returns>
    public BachsEnvironment GetEffectiveEnvironment()
    {
        if (Environment is { } explicitEnvironment)
            return explicitEnvironment;

        if (!string.IsNullOrWhiteSpace(BaseUrl))
            return BachsEnvironment.Custom;

        if (ApiKey.StartsWith(LiveKeyPrefix, StringComparison.OrdinalIgnoreCase))
            return BachsEnvironment.Live;

        // Sandbox key or unrecognized key: default to sandbox for safety.
        return BachsEnvironment.Sandbox;
    }

    /// <summary>
    /// Resolves the effective base URL. An explicit <see cref="BaseUrl"/> always wins; otherwise the
    /// URL is chosen from <see cref="GetEffectiveEnvironment"/>.
    /// </summary>
    /// <returns>The resolved base <see cref="Uri"/>, always ending in a trailing slash.</returns>
    /// <exception cref="BachsInvalidOperationException">
    /// Thrown if the environment is <see cref="BachsEnvironment.Custom"/> and no <see cref="BaseUrl"/> is set.
    /// </exception>
    public Uri GetEffectiveBaseUrl()
    {
        if (!string.IsNullOrWhiteSpace(BaseUrl))
            return new Uri(BaseUrl.TrimEnd('/') + "/");

        return GetEffectiveEnvironment() switch
        {
            BachsEnvironment.Live => new Uri(DefaultProductionBaseUrl + "/"),
            BachsEnvironment.Sandbox => new Uri(DefaultSandboxBaseUrl + "/"),
            _ => throw new BachsInvalidOperationException(
                "BaseUrl is required when Environment is Custom."
            ),
        };
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

        ValidateEnvironment();
        ValidateTimeouts();
        ValidateRetries();

        if (EnableCircuitBreaker)
            ValidateCircuitBreaker();
    }

    private void ValidateEnvironment()
    {
        var environment = GetEffectiveEnvironment();

        if (environment == BachsEnvironment.Custom)
        {
            if (string.IsNullOrWhiteSpace(BaseUrl))
                throw new BachsInvalidOperationException(
                    "BaseUrl is required when Environment is Custom."
                );
        }

        if (!string.IsNullOrWhiteSpace(BaseUrl) && !Uri.TryCreate(BaseUrl, UriKind.Absolute, out _))
        {
            throw new BachsInvalidOperationException("BaseUrl must be an absolute URI.");
        }

        // Guard against pointing a key at the wrong environment.
        if (
            environment == BachsEnvironment.Live
            && !ApiKey.StartsWith(LiveKeyPrefix, StringComparison.OrdinalIgnoreCase)
        )
        {
            throw new BachsInvalidOperationException(
                "Environment is Live but the API key does not start with 'sk_live_'."
            );
        }

        if (
            environment == BachsEnvironment.Sandbox
            && !ApiKey.StartsWith(SandboxKeyPrefix, StringComparison.OrdinalIgnoreCase)
        )
        {
            throw new BachsInvalidOperationException(
                "Environment is Sandbox but the API key does not start with 'sk_sandbox_'."
            );
        }
    }

    private void ValidateTimeouts()
    {
        if (TotalRequestTimeout <= TimeSpan.Zero)
            throw new BachsInvalidOperationException(
                "TotalRequestTimeout must be greater than zero."
            );

        if (AttemptTimeout <= TimeSpan.Zero)
            throw new BachsInvalidOperationException("AttemptTimeout must be greater than zero.");

        if (AttemptTimeout > TotalRequestTimeout)
            throw new BachsInvalidOperationException(
                "AttemptTimeout cannot be greater than TotalRequestTimeout."
            );
    }

    private void ValidateRetries()
    {
        if (MaxRetries < 0)
            throw new BachsInvalidOperationException("MaxRetries cannot be negative.");

        if (RetryDelay < TimeSpan.Zero)
            throw new BachsInvalidOperationException("RetryDelay cannot be negative.");

        if (MaxRetryDelay < RetryDelay)
            throw new BachsInvalidOperationException(
                "MaxRetryDelay cannot be less than RetryDelay."
            );
    }

    private void ValidateCircuitBreaker()
    {
        if (CircuitBreakerFailureRatio is <= 0 or > 1)
            throw new BachsInvalidOperationException(
                "CircuitBreakerFailureRatio must be greater than zero and no greater than one."
            );

        if (CircuitBreakerMinimumThroughput <= 0)
            throw new BachsInvalidOperationException(
                "CircuitBreakerMinimumThroughput must be greater than zero."
            );

        if (CircuitBreakerSamplingDuration <= TimeSpan.Zero)
            throw new BachsInvalidOperationException(
                "CircuitBreakerSamplingDuration must be greater than zero."
            );

        if (CircuitBreakerBreakDuration <= TimeSpan.Zero)
            throw new BachsInvalidOperationException(
                "CircuitBreakerBreakDuration must be greater than zero."
            );
    }
}
