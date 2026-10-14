using Bachs.Net.Common;
using Bachs.Net.Models.Misc;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for reference data and bank account resolution.
/// </summary>
public sealed class MiscClient(IBachsClient bachsClient) : BaseClient(bachsClient), IMiscClient
{
    public Task<IReadOnlyList<string>> GetSupportedCurrenciesAsync(
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        return BachsClient.GetAsync<IReadOnlyList<string>>(
            "v1/currencies/supported",
            requestOptions,
            cancellationToken
        );
    }

    public Task<IReadOnlyList<BankInfo>> GetBanksAsync(
        string country = "NG",
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = BachsQueryBuilder.BuildUrl(
            "v1/reference/banks",
            new Dictionary<string, string> { ["country"] = country }
        );

        return BachsClient.GetAsync<IReadOnlyList<BankInfo>>(
            path,
            requestOptions,
            cancellationToken
        );
    }

    public Task<ResolveBankAccountResponse> ResolveBankAccountAsync(
        ResolveBankAccountRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<ResolveBankAccountRequest, ResolveBankAccountResponse>(
            "v1/misc/bank-accounts/resolve",
            request,
            requestOptions,
            cancellationToken
        );
    }
}
