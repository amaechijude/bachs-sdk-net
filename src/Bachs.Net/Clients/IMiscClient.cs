using Bachs.Net.Common;
using Bachs.Net.Models.Misc;

namespace Bachs.Net.Clients;

/// <summary>
/// Client for reference endpoints (banks, currencies, rails, bank account resolution).
/// </summary>
public interface IMiscClient
{
    /// <summary>
    /// Lists supported payout and collection currencies.
    /// </summary>
    Task<IReadOnlyList<string>> GetSupportedCurrenciesAsync(
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists banks available for payout destinations.
    /// </summary>
    Task<IReadOnlyList<BankInfo>> GetBanksAsync(
        string country = "NG",
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves and verifies bank account details for a destination.
    /// </summary>
    Task<ResolveBankAccountResponse> ResolveBankAccountAsync(
        ResolveBankAccountRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default);
}
