using Bachs.Net.Common;
using Bachs.Net.Models.Checkout;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for managing hosted checkout sessions.
/// </summary>
public sealed class CheckoutSessionsClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        ICheckoutSessionsClient
{
    public Task<CreateCheckoutSessionResponse> CreateAsync(
        CreateCheckoutSessionRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    ) =>
        BachsClient.PostAsync<CreateCheckoutSessionRequest, CreateCheckoutSessionResponse>(
            "v1/checkout-sessions",
            request,
            requestOptions,
            cancellationToken
        );

    public Task<GetCheckoutSessionResponse> GetAsync(
        string checkoutId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    ) =>
        BachsClient.GetAsync<GetCheckoutSessionResponse>(
            $"v1/checkout-sessions/{Uri.EscapeDataString(checkoutId)}",
            requestOptions,
            cancellationToken
        );
}
