using Bachs.Net.Common;
using Bachs.Net.Models.Disputes;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for managing disputes and chargebacks.
/// </summary>
public sealed class DisputesClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        IDisputesClient
{
    public Task<Dispute> GetAsync(
        string disputeId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(disputeId);
        return BachsClient.GetAsync<Dispute>(
            $"v1/disputes/{Uri.EscapeDataString(disputeId)}",
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsList<Dispute>> ListAsync(
        ListDisputesOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = QueryBuilder.BuildUrl("v1/disputes", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Dispute>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }

    public Task<Dispute> UpdateEvidenceAsync(
        string disputeId,
        UpdateDisputeEvidenceRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(disputeId);
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PatchAsync<UpdateDisputeEvidenceRequest, Dispute>(
            $"v1/disputes/{Uri.EscapeDataString(disputeId)}/evidence",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<Dispute> SubmitAsync(
        string disputeId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(disputeId);
        return BachsClient.PostAsync<Dispute>(
            $"v1/disputes/{Uri.EscapeDataString(disputeId)}/submit",
            requestOptions,
            cancellationToken
        );
    }
}
