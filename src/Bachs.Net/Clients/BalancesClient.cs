using Bachs.Net.Common;
using Bachs.Net.Models.Balances;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for reading balances and managing payout settings.
/// </summary>
public sealed class BalancesClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        IBalancesClient
{
    public Task<Balance> GetBalancesAsync(
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        return BachsClient.GetAsync<Balance>("v1/balances", requestOptions, cancellationToken);
    }

    public Task<BalanceSettings> GetSettingsAsync(
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        return BachsClient.GetAsync<BalanceSettings>(
            "v1/balance_settings",
            requestOptions,
            cancellationToken
        );
    }

    public Task<BalanceSettings> UpdateSettingsAsync(
        BalanceSettings settings,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(settings);
        return BachsClient.PostAsync<BalanceSettings, BalanceSettings>(
            "v1/balance_settings",
            settings,
            requestOptions,
            cancellationToken
        );
    }
}
