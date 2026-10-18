using System.Net;
using Bachs.Clients;
using Bachs.Common;
using Bachs.WebHooks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.Extensions.Options;
using Polly;

namespace Bachs.Extensions;

/// <summary>
/// Provides extension methods for registering the Bachs client and configuration in an <see cref="IServiceCollection"/>.
/// </summary>
public static class BachsServiceExtensions
{
    /// <summary>
    /// Registers the Bachs API client and sub-clients in the dependency injection container.
    /// </summary>
    /// <remarks>
    /// The registered client uses a standard HTTP resilience pipeline configured from
    /// <see cref="BachsClientOptions"/>. Its retries replace the standalone client's retry loop.
    /// Options are validated with <see cref="BachsClientOptions.Validate"/> when first resolved.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the client to.</param>
    /// <param name="configureOptions">Delegate to configure <see cref="BachsClientOptions"/>.</param>
    /// <returns>The service collection for further service registration.</returns>
    public static IServiceCollection AddBachsClient(
        this IServiceCollection services,
        Action<BachsClientOptions> configureOptions
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure(configureOptions);
        services.PostConfigure<BachsClientOptions>(options => options.Validate());
        return RegisterClientCore(services);
    }

    /// <summary>
    /// Registers the Bachs API client with a secret API key.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the client to.</param>
    /// <param name="apiKey">The Bachs secret key (sk_sandbox_... or sk_live_...).</param>
    /// <param name="webhookSecret">The Bachs webhook secret (whsec_...).</param>
    /// <returns>An <see cref="IServiceCollection"/> for configuring the underlying HTTP client and resilience pipelines.</returns>
    public static IServiceCollection AddBachsClient(
        this IServiceCollection services,
        string apiKey,
        string webhookSecret
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        return services.AddBachsClient(options =>
        {
            options.ApiKey = apiKey;
            options.WebhookSecret = webhookSecret;
        });
    }

    private static IServiceCollection RegisterClientCore(IServiceCollection services)
    {
        services
            .AddHttpClient<IBachsClient, BachsClient>(
                (sp, httpClient) =>
                {
                    var bachs = sp.GetRequiredService<IOptions<BachsClientOptions>>().Value;
                    httpClient.BaseAddress = bachs.GetEffectiveBaseUrl();

                    // Timeouts are owned by the resilience pipeline.
                    httpClient.Timeout = Timeout.InfiniteTimeSpan;
                }
            )
            .AddStandardResilienceHandler()
            .Configure(
                (options, sp) =>
                {
                    var bachs = sp.GetRequiredService<IOptions<BachsClientOptions>>().Value;

                    // Timeouts
                    options.TotalRequestTimeout.Timeout = bachs.TotalRequestTimeout;
                    options.AttemptTimeout.Timeout = bachs.AttemptTimeout;

                    // Retries. Polly requires MaxRetryAttempts >= 1, so "no retries" is
                    // expressed through ShouldHandle below rather than by setting 0.
                    options.Retry.MaxRetryAttempts = Math.Max(1, bachs.MaxRetries);
                    options.Retry.BackoffType = DelayBackoffType.Exponential;
                    options.Retry.Delay = bachs.RetryDelay;
                    options.Retry.MaxDelay = bachs.MaxRetryDelay;
                    options.Retry.UseJitter = true;
                    options.Retry.ShouldRetryAfterHeader = true;
                    options.Retry.ShouldHandle = args =>
                        ValueTask.FromResult(ShouldRetry(bachs, args.Outcome));

                    // Circuit breaker
                    options.CircuitBreaker.FailureRatio = bachs.CircuitBreakerFailureRatio;
                    options.CircuitBreaker.MinimumThroughput =
                        bachs.CircuitBreakerMinimumThroughput;
                    options.CircuitBreaker.SamplingDuration = bachs.CircuitBreakerSamplingDuration;
                    options.CircuitBreaker.BreakDuration = bachs.CircuitBreakerBreakDuration;
                    options.CircuitBreaker.ShouldHandle = args =>
                        ValueTask.FromResult(
                            bachs.EnableCircuitBreaker
                                && HttpClientResiliencePredicates.IsTransient(args.Outcome)
                        );
                }
            );

        // Register typed sub-clients for granular constructor injection
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().CheckoutSessions);
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().Products);
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().Customers);
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().Subscriptions);
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().Payments);
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().Refunds);
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().Payouts);
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().PayoutDestinations);
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().Balances);
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().Conversions);
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().Transfers);
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().Disputes);
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().Accounts);
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().VirtualAccounts);
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().Webhooks);
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().Misc);
        services.TryAddTransient<IWebhookEventValidator>(sp =>
            sp.GetRequiredService<IBachsClient>().Webhooks
        );

        return services;
    }

    /// <summary>
    /// Decides whether an outcome is a transient failure worth retrying.
    /// </summary>
    private static bool ShouldRetry(BachsClientOptions bachs, Outcome<HttpResponseMessage> outcome)
    {
        if (bachs.MaxRetries <= 0)
            return false;

        if (outcome.Exception is HttpRequestException or Polly.Timeout.TimeoutRejectedException)
            return true;

        if (
            outcome.Result is not { StatusCode: var status } response
            || status
                is not (
                    HttpStatusCode.TooManyRequests
                    or HttpStatusCode.RequestTimeout
                    or HttpStatusCode.InternalServerError
                    or HttpStatusCode.BadGateway
                    or HttpStatusCode.ServiceUnavailable
                    or HttpStatusCode.GatewayTimeout
                )
        )
        {
            return false;
        }

        // If the server asks us to wait longer than we are willing to, fail now
        // instead of burning the request budget.
        return GetRetryAfter(response) is not { } retryAfter || retryAfter <= bachs.MaxRetryDelay;
    }

    /// <summary>
    /// Reads the <c>Retry-After</c> header, which Bachs sends as a number of seconds on a 429.
    /// .NET parses that form into <see cref="System.Net.Http.Headers.RetryConditionHeaderValue.Delta"/>.
    /// </summary>
    private static TimeSpan? GetRetryAfter(HttpResponseMessage response) =>
        response.Headers.RetryAfter?.Delta is { } delta && delta >= TimeSpan.Zero ? delta : null;
}
