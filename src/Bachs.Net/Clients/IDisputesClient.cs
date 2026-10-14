using Bachs.Net.Common;
using Bachs.Net.Models.Disputes;

namespace Bachs.Net.Clients;

/// <summary>
/// Client for reading and managing customer charge disputes.
/// </summary>
public interface IDisputesClient
{
    /// <summary>
    /// Retrieves a dispute by its ID.
    /// </summary>
    Task<Dispute> GetAsync(
        string disputeId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists disputes with pagination and filtering.
    /// </summary>
    Task<BachsList<Dispute>> ListAsync(
        ListDisputesOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates evidence drafts for a dispute.
    /// </summary>
    Task<Dispute> UpdateEvidenceAsync(
        string disputeId,
        UpdateDisputeEvidenceRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Submits evidence for a dispute to the payment network for review.
    /// </summary>
    Task<Dispute> SubmitAsync(
        string disputeId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
