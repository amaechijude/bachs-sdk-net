using Bachs.Net.Clients;
using Bachs.Net.Common;
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
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">Delegate to configure <see cref="BachsClientOptions"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddBachsClient(
        this IServiceCollection services,
        Action<BachsClientOptions> configureOptions
    )
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure(configureOptions);
        RegisterClientCore(services);
        return services;
    }

    /// <summary>
    /// Registers the Bachs API client with a secret API key.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="apiKey">The Bachs secret key (sk_sandbox_... or sk_live_...).</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddBachsClient(this IServiceCollection services, string apiKey)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(apiKey);
        return services.AddBachsClient(options => options.ApiKey = apiKey);
    }

    private static void RegisterClientCore(IServiceCollection services)
    {
        services.AddHttpClient<IBachsClient, BachsClient>(
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
        services.TryAddTransient(sp => sp.GetRequiredService<IBachsClient>().Misc);
    }
}
