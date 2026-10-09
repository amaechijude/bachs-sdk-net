using Bachs.Net.Common;
using Bachs.Net.Models.Payments;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for reading and listing payments.
/// </summary>
public class PaymentsClient : BaseClient, IPaymentsClient
{
    public PaymentsClient(IBachsClient bachsClient) : base(bachsClient) { }

    public Task<Payment> GetAsync(
        string paymentId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentId);
        return BachsClient.GetAsync<Payment>(
            $"v1/payments/{Uri.EscapeDataString(paymentId)}",
            requestOptions,
            cancellationToken);
    }

    public Task<BachsList<Payment>> ListAsync(
        ListPaymentsOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        var path = QueryBuilder.BuildUrl("v1/payments", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Payment>>(
            path,
            requestOptions ?? options,
            cancellationToken);
    }
}
