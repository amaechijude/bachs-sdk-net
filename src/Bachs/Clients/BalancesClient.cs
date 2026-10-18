using Bachs.Clients;
using Bachs.Common;
using Bachs.Models.Balances;

namespace Bachs.Clients;

/// <summary>
/// Client implementation for reading balances and managing payout settings.
/// </summary>
public sealed class BalancesClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        IBalancesClient
{
    public Task<BachsResponse<Balance>> GetBalancesAsync(
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        return BachsClient.GetAsync<Balance>("v1/balances", requestOptions, cancellationToken);
    }

    public Task<BachsResponse<BalanceSettings>> GetSettingsAsync(
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

    public Task<BachsResponse<BalanceSettings>> UpdateSettingsAsync(
        BalanceSettings settings,
        BachsRequestOptions requestOptions,
        CancellationToken cancellationToken = default
    ) =>
        BachsClient.PostAsync<BalanceSettings, BalanceSettings>(
            "v1/balance_settings",
            settings,
            requestOptions,
            cancellationToken
        );
}
