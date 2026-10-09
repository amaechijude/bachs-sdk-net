using Bachs.Net.Common;
using Bachs.Net.Models.Webhooks;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for managing webhook destination endpoints and events.
/// </summary>
public class WebhooksClient : BaseClient, IWebhooksClient
{
    public WebhooksClient(IBachsClient bachsClient) : base(bachsClient) { }

    public Task<WebhookEndpoint> CreateEndpointAsync(
        CreateWebhookEndpointRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<CreateWebhookEndpointRequest, WebhookEndpoint>(
            "v1/webhooks/endpoints",
            request,
            requestOptions,
            cancellationToken);
    }

    public Task<WebhookEndpoint> GetEndpointAsync(
        string endpointId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpointId);
        return BachsClient.GetAsync<WebhookEndpoint>(
            $"v1/webhooks/endpoints/{Uri.EscapeDataString(endpointId)}",
            requestOptions,
            cancellationToken);
    }

    public Task<BachsList<WebhookEndpoint>> ListEndpointsAsync(
        ListWebhookEndpointsOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        var path = QueryBuilder.BuildUrl("v1/webhooks/endpoints", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<WebhookEndpoint>>(
            path,
            requestOptions ?? options,
            cancellationToken);
    }

    public Task<WebhookEndpoint> UpdateEndpointAsync(
        string endpointId,
        UpdateWebhookEndpointRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpointId);
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PatchAsync<UpdateWebhookEndpointRequest, WebhookEndpoint>(
            $"v1/webhooks/endpoints/{Uri.EscapeDataString(endpointId)}",
            request,
            requestOptions,
            cancellationToken);
    }

    public Task DeleteEndpointAsync(
        string endpointId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpointId);
        return BachsClient.DeleteAsync(
            $"v1/webhooks/endpoints/{Uri.EscapeDataString(endpointId)}",
            requestOptions,
            cancellationToken);
    }

    public Task<SecretRotationResult> RotateSecretAsync(
        string endpointId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpointId);
        return BachsClient.PostAsync<SecretRotationResult>(
            $"v1/webhooks/endpoints/{Uri.EscapeDataString(endpointId)}/rotate-secret",
            requestOptions,
            cancellationToken);
    }

    public Task<BachsList<WebhookEventRecord>> ListEventsAsync(
        ListRequestOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        var path = QueryBuilder.BuildUrl("v1/webhooks/events", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<WebhookEventRecord>>(
            path,
            requestOptions ?? options,
            cancellationToken);
    }

    public Task<WebhookEventRecord> GetEventAsync(
        string eventId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventId);
        return BachsClient.GetAsync<WebhookEventRecord>(
            $"v1/webhooks/events/{Uri.EscapeDataString(eventId)}",
            requestOptions,
            cancellationToken);
    }
}
