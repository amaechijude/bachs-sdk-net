using Bachs.Net.Common;
using Bachs.Net.Models.Checkout;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for managing hosted checkout sessions.
/// </summary>
public class CheckoutSessionsClient : BaseClient, ICheckoutSessionsClient
{
    public CheckoutSessionsClient(IBachsClient bachsClient) : base(bachsClient) { }

    public Task<CheckoutSession> CreateAsync(
        CreateCheckoutSessionRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<CreateCheckoutSessionRequest, CheckoutSession>(
            "v1/checkout-sessions",
            request,
            requestOptions,
            cancellationToken);
    }

    public Task<CheckoutSession> GetAsync(
        string checkoutId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(checkoutId);
        return BachsClient.GetAsync<CheckoutSession>(
            $"v1/checkout-sessions/{Uri.EscapeDataString(checkoutId)}",
            requestOptions,
            cancellationToken);
    }
}
