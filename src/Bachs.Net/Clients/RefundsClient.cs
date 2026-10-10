using Bachs.Net.Common;
using Bachs.Net.Models.Refunds;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for issuing and tracking refunds.
/// </summary>
public sealed class RefundsClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        IRefundsClient
{
    public Task<Refund> CreateAsync(
        CreateRefundRequest request,
        RequestOptions? requestOptions = null,
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

    public Task<Refund> GetAsync(
        string refundId,
        RequestOptions? requestOptions = null,
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

    public Task<BachsList<Refund>> ListAsync(
        ListRefundsOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = QueryBuilder.BuildUrl("v1/refunds", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Refund>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }

    public Task<BachsList<Refund>> GetByPaymentIdAsync(
        string paymentId,
        RequestOptions? requestOptions = null,
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
