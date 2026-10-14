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
    public Task<BachsResponse<Dispute>> GetAsync(
        string disputeId,
        BachsRequestOptions? requestOptions = null,
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

    public Task<BachsResponse<BachsList<Dispute>>> ListAsync(
        ListDisputesOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = BachsQueryBuilder.BuildUrl("v1/disputes", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Dispute>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }

    public Task<BachsResponse<Dispute>> UpdateEvidenceAsync(
        string disputeId,
        UpdateDisputeEvidenceRequest request,
        BachsRequestOptions? requestOptions = null,
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

    public Task<BachsResponse<Dispute>> SubmitAsync(
        string disputeId,
        BachsRequestOptions? requestOptions = null,
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
