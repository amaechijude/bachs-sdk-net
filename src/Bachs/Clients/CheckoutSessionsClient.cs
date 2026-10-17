using Bachs.Common;
using Bachs.Models.Checkout;

namespace Bachs.Clients;

/// <summary>
/// Client implementation for managing hosted checkout sessions.
/// </summary>
public sealed class CheckoutSessionsClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        ICheckoutSessionsClient
{
    public Task<BachsResponse<CreateCheckoutSessionResponse>> CreateAsync(
        CreateCheckoutSessionRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    ) =>
        BachsClient.PostAsync<CreateCheckoutSessionRequest, CreateCheckoutSessionResponse>(
            "v1/checkout-sessions",
            request,
            requestOptions,
            cancellationToken
        );

    public Task<BachsResponse<GetCheckoutSessionResponse>> GetAsync(
        string checkoutId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    ) =>
        BachsClient.GetAsync<GetCheckoutSessionResponse>(
            $"v1/checkout-sessions/{Uri.EscapeDataString(checkoutId)}",
            requestOptions,
            cancellationToken
        );
}
