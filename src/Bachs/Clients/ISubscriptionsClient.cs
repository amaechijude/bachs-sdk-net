using Bachs.Common;
using Bachs.Exceptions;
using Bachs.Models.Subscriptions;

namespace Bachs.Clients;

/// <summary>
/// Client for managing recurring subscriptions.
/// </summary>
public interface ISubscriptionsClient
{
    /// <summary>
    /// Retrieves a subscription by its ID.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the identifier argument is null, empty, or whitespace.</exception>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<Subscription>> GetAsync(
        string subscriptionId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists subscriptions with pagination and filtering.
    /// </summary>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<BachsList<Subscription>>> ListAsync(
        ListSubscriptionsOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates a subscription (e.g. change plan or toggle cancel_at_period_end).
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the identifier argument is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the request body is <see langword="null"/>.</exception>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<Subscription>> UpdateAsync(
        string subscriptionId,
        UpdateSubscriptionRequest request,
        BachsRequestOptions requestOptions,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Cancels a subscription immediately.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the identifier argument is null, empty, or whitespace.</exception>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<Subscription>> CancelAsync(
        string subscriptionId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
