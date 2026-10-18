using Bachs.Clients;
using Bachs.Common;
using Bachs.Models.Common;
using Bachs.Models.VirtualAccounts;

namespace Bachs.Clients;

/// <summary>
/// Client implementation for managing fixed virtual bank accounts.
/// </summary>
public sealed class VirtualAccountsClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        IVirtualAccountsClient
{
    public Task<BachsResponse<VirtualAccount>> CreateAsync(
        CreateVirtualAccountRequest request,
        BachsRequestOptions requestOptions,
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

    public Task<BachsResponse<VirtualAccount>> GetAsync(
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
