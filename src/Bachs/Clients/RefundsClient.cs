using Bachs.Clients;
using Bachs.Common;
using Bachs.Models.Refunds;

namespace Bachs.Clients;

/// <summary>
/// Client implementation for issuing and tracking refunds.
/// </summary>
public sealed class RefundsClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        IRefundsClient
{
    public Task<BachsResponse<Refund>> CreateAsync(
        CreateRefundRequest request,
        BachsRequestOptions requestOptions,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<CreateRefundRequest, Refund>(
            "v1/refunds",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<Refund>> GetAsync(
        string refundId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refundId);
        return BachsClient.GetAsync<Refund>(
            $"v1/refunds/{Uri.EscapeDataString(refundId)}",
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<BachsList<Refund>>> ListAsync(
        ListRefundsOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = BachsQueryBuilder.BuildUrl("v1/refunds", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Refund>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }

    public Task<BachsResponse<BachsList<Refund>>> GetByPaymentIdAsync(
        string paymentId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentId);
        return BachsClient.GetAsync<BachsList<Refund>>(
            $"v1/refunds/by-charge/{Uri.EscapeDataString(paymentId)}",
            requestOptions,
            cancellationToken
        );
    }
}
