using Bachs.Net.Common;
using Bachs.Net.Models.Refunds;

namespace Bachs.Net.Clients;

/// <summary>
/// Client for issuing and tracking refunds.
/// </summary>
public interface IRefundsClient
{
    /// <summary>
    /// Issues a full or partial refund against a payment.
    /// </summary>
    Task<BachsResponse<Refund>> CreateAsync(
        CreateRefundRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a refund by its ID.
    /// </summary>
    Task<BachsResponse<Refund>> GetAsync(
        string refundId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists refunds with pagination and filtering.
    /// </summary>
    Task<BachsResponse<BachsList<Refund>>> ListAsync(
        ListRefundsOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves refunds associated with a specific charge/payment ID.
    /// </summary>
    Task<BachsResponse<BachsList<Refund>>> GetByPaymentIdAsync(
        string paymentId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
