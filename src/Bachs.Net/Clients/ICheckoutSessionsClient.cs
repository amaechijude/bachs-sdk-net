using Bachs.Net.Common;
using Bachs.Net.Models.Checkout;
using Bachs.Net.Models.Checkout.Request;

namespace Bachs.Net.Clients;

/// <summary>
/// Client for managing hosted checkout sessions.
/// </summary>
public interface ICheckoutSessionsClient
{
    /// <summary>
    /// Creates a new checkout session.
    /// </summary>
    /// <param name="request">The checkout session creation parameters.</param>
    /// <param name="requestOptions">Optional per-request options (e.g. idempotency key).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created checkout session response containing the hosted checkout URL.</returns>
    Task<CreateCheckoutSessionResponse> CreateAsync(
        CreateCheckoutSessionRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a checkout session by its ID.
    /// </summary>
    /// <param name="checkoutId">The checkout session ID (starts with 'chk_').</param>
    /// <param name="requestOptions">Optional per-request options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The detailed checkout session details including resolved line items and charges.</returns>
    Task<GetCheckoutSessionResponse> GetAsync(
        string checkoutId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
