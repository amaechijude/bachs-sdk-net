using Bachs.Net.Common;
using Bachs.Net.Models.Subscriptions;

namespace Bachs.Net.Clients;

/// <summary>
/// Client for managing recurring subscriptions.
/// </summary>
public interface ISubscriptionsClient
{
    /// <summary>
    /// Retrieves a subscription by its ID.
    /// </summary>
    Task<Subscription> GetAsync(
        string subscriptionId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists subscriptions with pagination and filtering.
    /// </summary>
    Task<BachsList<Subscription>> ListAsync(
        ListSubscriptionsOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates a subscription (e.g. change plan or toggle cancel_at_period_end).
    /// </summary>
    Task<Subscription> UpdateAsync(
        string subscriptionId,
        UpdateSubscriptionRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Cancels a subscription immediately.
    /// </summary>
    Task<Subscription> CancelAsync(
        string subscriptionId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
