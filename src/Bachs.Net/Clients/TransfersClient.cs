using Bachs.Net.Common;
using Bachs.Net.Models.Transfers;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for managing transfers between platform and connected accounts.
/// </summary>
public sealed class TransfersClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        ITransfersClient
{
    public Task<Transfer> CreateAsync(
        CreateTransferRequest request,
        RequestOptions? requestOptions = null,
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

    public Task<Transfer> GetAsync(
        string transferId,
        RequestOptions? requestOptions = null,
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

    public Task<BachsList<Transfer>> ListAsync(
        ListTransfersOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = QueryBuilder.BuildUrl("v1/transfers", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Transfer>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }
}
