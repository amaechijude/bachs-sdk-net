using Bachs.Net.Common;
using Bachs.Net.Exceptions;
using Bachs.Net.Models.Checkout;

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
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<CreateCheckoutSessionResponse>> CreateAsync(
        CreateCheckoutSessionRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a checkout session by its ID.
    /// </summary>
    /// <param name="checkoutId">The checkout session ID (starts with 'chk_').</param>
    /// <param name="requestOptions">Optional per-request options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The detailed checkout session details including resolved line items and charges.</returns>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<GetCheckoutSessionResponse>> GetAsync(
        string checkoutId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
