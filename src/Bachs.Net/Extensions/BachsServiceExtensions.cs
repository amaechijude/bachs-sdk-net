using Bachs.Net.Clients;
using Bachs.Net.Common;
using Bachs.Net.WebHooks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Bachs.Net.Extensions;

/// <summary>
/// Provides extension methods for registering the Bachs client and configuration in an <see cref="IServiceCollection"/>.
/// </summary>
public static class BachsServiceExtensions
{
    /// <summary>
    /// Registers the Bachs API client and sub-clients in the dependency injection container.
    /// </summary>
    /// <remarks>
    /// We recommend using the returned <see cref="IHttpClientBuilder"/> to configure standard
    /// resilience handlers (e.g., Polly or <c>AddStandardResilienceHandler</c>). If you do so, ensure
    /// <see cref="BachsClientOptions.MaxRetries"/> remains 0 (the default) to avoid nested retries.
    /// </remarks>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the client to.</param>
    /// <param name="configureOptions">Delegate to configure <see cref="BachsClientOptions"/>.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> for configuring the underlying HTTP client and resilience pipelines.</returns>
    public static IHttpClientBuilder AddBachsClient(
        this IServiceCollection services,
        Action<BachsClientOptions> configureOptions
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure(configureOptions);
        return RegisterClientCore(services);
    }

    /// <summary>
    /// Registers the Bachs API client and sub-clients with options and an HTTP client builder configurator.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the client to.</param>
    /// <param name="configureOptions">Delegate to configure <see cref="BachsClientOptions"/>.</param>
    /// <param name="configureHttpClient">Delegate to configure the <see cref="IHttpClientBuilder"/> (e.g. resilience handlers).</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> for further configuration.</returns>
    public static IHttpClientBuilder AddBachsClient(
        this IServiceCollection services,
        Action<BachsClientOptions> configureOptions,
        Action<IHttpClientBuilder> configureHttpClient
    )
    {
        ArgumentNullException.ThrowIfNull(configureHttpClient);
        var builder = services.AddBachsClient(configureOptions);
        configureHttpClient(builder);
        return builder;
    }

    /// <summary>
    /// Registers the Bachs API client with a secret API key.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the client to.</param>
    /// <param name="apiKey">The Bachs secret key (sk_sandbox_... or sk_live_...).</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> for configuring the underlying HTTP client and resilience pipelines.</returns>
    public static IHttpClientBuilder AddBachsClient(this IServiceCollection services, string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        return services.AddBachsClient(options => options.ApiKey = apiKey);
    }

    /// <summary>
    /// Registers the Bachs API client with a secret API key and an HTTP client builder configurator.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the client to.</param>
    /// <param name="apiKey">The Bachs secret key (sk_sandbox_... or sk_live_...).</param>
    /// <param name="configureHttpClient">Delegate to configure the <see cref="IHttpClientBuilder"/> (e.g. resilience handlers).</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> for further configuration.</returns>
    public static IHttpClientBuilder AddBachsClient(
        this IServiceCollection services,
        string apiKey,
        Action<IHttpClientBuilder> configureHttpClient
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        ArgumentNullException.ThrowIfNull(configureHttpClient);
        var builder = services.AddBachsClient(apiKey);
        configureHttpClient(builder);
        return builder;
    }

    private static IHttpClientBuilder RegisterClientCore(IServiceCollection services)
    {
        var builder = services.AddHttpClient<IBachsClient, BachsClient>(
            (serviceProvider, httpClient) =>
            {
                var options = serviceProvider
                    .GetRequiredService<IOptions<BachsClientOptions>>()
                    .Value;
                httpClient.BaseAddress = options.GetEffectiveBaseUrl();
                httpClient.Timeout = options.Timeout;
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
        services.TryAddTransient<IWebhookEventValidator>(sp =>
            sp.GetRequiredService<IBachsClient>().Webhooks
        );
        services.TryAddTransient<IWebhookValidator>(sp =>
            sp.GetRequiredService<IBachsClient>().Webhooks
        );
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().Misc);

        return builder;
    }
}
