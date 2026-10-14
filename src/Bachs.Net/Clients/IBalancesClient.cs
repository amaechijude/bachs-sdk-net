using Bachs.Net.Common;
using Bachs.Net.Models.Balances;

namespace Bachs.Net.Clients;

/// <summary>
/// Client for reading balances and managing payout settings.
/// </summary>
public interface IBalancesClient
{
    /// <summary>
    /// Retrieves organization balance buckets across all currencies.
    /// </summary>
    Task<BachsResponse<Balance>> GetBalancesAsync(
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves current automatic payout balance settings.
    /// </summary>
    Task<BachsResponse<BalanceSettings>> GetSettingsAsync(
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates automatic payout balance settings.
    /// </summary>
    Task<BachsResponse<BalanceSettings>> UpdateSettingsAsync(
        BalanceSettings settings,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
