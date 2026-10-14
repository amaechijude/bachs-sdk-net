using Bachs.Net.Common;
using Bachs.Net.Models.Subscriptions;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for managing subscriptions.
/// </summary>
public sealed class SubscriptionsClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        ISubscriptionsClient
{
    public Task<BachsResponse<Subscription>> GetAsync(
        string subscriptionId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subscriptionId);
        return BachsClient.GetAsync<Subscription>(
            $"v1/subscriptions/{Uri.EscapeDataString(subscriptionId)}",
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<BachsList<Subscription>>> ListAsync(
        ListSubscriptionsOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = BachsQueryBuilder.BuildUrl("v1/subscriptions", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Subscription>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }

    public Task<BachsResponse<Subscription>> UpdateAsync(
        string subscriptionId,
        UpdateSubscriptionRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subscriptionId);
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PatchAsync<UpdateSubscriptionRequest, Subscription>(
            $"v1/subscriptions/{Uri.EscapeDataString(subscriptionId)}",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<Subscription>> CancelAsync(
        string subscriptionId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(subscriptionId);
        return BachsClient.DeleteAsync<Subscription>(
            $"v1/subscriptions/{Uri.EscapeDataString(subscriptionId)}",
            requestOptions,
            cancellationToken
        );
    }
}
