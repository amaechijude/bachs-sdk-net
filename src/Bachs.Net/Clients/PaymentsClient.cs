using Bachs.Net.Common;
using Bachs.Net.Models.Payments;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for reading and listing payments.
/// </summary>
public sealed class PaymentsClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        IPaymentsClient
{
    public Task<Payment> GetAsync(
        string paymentId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentId);
        return BachsClient.GetAsync<Payment>(
            $"v1/payments/{Uri.EscapeDataString(paymentId)}",
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsList<Payment>> ListAsync(
        ListPaymentsOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = BachsQueryBuilder.BuildUrl("v1/payments", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Payment>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }
}
