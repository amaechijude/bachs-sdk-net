using Bachs.Net.Common;
using Bachs.Net.Models.Common;
using Bachs.Net.Models.VirtualAccounts;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for managing fixed virtual bank accounts.
/// </summary>
public sealed class VirtualAccountsClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        IVirtualAccountsClient
{
    public Task<VirtualAccount> CreateAsync(
        CreateVirtualAccountRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<CreateVirtualAccountRequest, VirtualAccount>(
            "v1/virtual-accounts",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<VirtualAccount> GetAsync(
        BachsCurrency currency = BachsCurrency.NGN,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = BachsQueryBuilder.BuildUrl(
            "v1/virtual-accounts",
            new Dictionary<string, string> { ["currency"] = currency.ToString() }
        );

        return BachsClient.GetAsync<VirtualAccount>(path, requestOptions, cancellationToken);
    }
}
