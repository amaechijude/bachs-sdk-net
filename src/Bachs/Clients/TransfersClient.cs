using Bachs.Clients;
using Bachs.Common;
using Bachs.Models.Transfers;

namespace Bachs.Clients;

/// <summary>
/// Client implementation for managing transfers between platform and connected accounts.
/// </summary>
public sealed class TransfersClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        ITransfersClient
{
    public Task<BachsResponse<Transfer>> CreateAsync(
        CreateTransferRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<CreateTransferRequest, Transfer>(
            "v1/transfers",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<Transfer>> GetAsync(
        string transferId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(transferId);
        return BachsClient.GetAsync<Transfer>(
            $"v1/transfers/{Uri.EscapeDataString(transferId)}",
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<BachsList<Transfer>>> ListAsync(
        ListTransfersOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = BachsQueryBuilder.BuildUrl("v1/transfers", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Transfer>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }
}
