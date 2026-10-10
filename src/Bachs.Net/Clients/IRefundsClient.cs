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
    Task<Refund> CreateAsync(
        CreateRefundRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a refund by its ID.
    /// </summary>
    Task<Refund> GetAsync(
        string refundId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists refunds with pagination and filtering.
    /// </summary>
    Task<BachsList<Refund>> ListAsync(
        ListRefundsOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves refunds associated with a specific charge/payment ID.
    /// </summary>
    Task<BachsList<Refund>> GetByPaymentIdAsync(
        string paymentId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
