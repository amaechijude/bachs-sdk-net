using Bachs.Net.Common;
using Bachs.Net.Models.Webhooks;
using Bachs.Net.WebHooks;

namespace Bachs.Net.Clients;

/// <summary>
/// Client for managing webhook destination endpoints, inspecting/replaying webhook deliveries,
/// and validating incoming webhook events.
/// </summary>
public interface IWebhooksClient : IWebhookEventValidator, IWebhookValidator
{
    /// <summary>
    /// Creates a new webhook destination endpoint.
    /// </summary>
    Task<WebhookEndpoint> CreateEndpointAsync(
        CreateWebhookEndpointRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a webhook endpoint by its ID.
    /// </summary>
    Task<WebhookEndpoint> GetEndpointAsync(
        string endpointId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists configured webhook endpoints.
    /// </summary>
    Task<BachsList<WebhookEndpoint>> ListEndpointsAsync(
        ListWebhookEndpointsOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates a webhook endpoint's URL or subscribed events.
    /// </summary>
    Task<WebhookEndpoint> UpdateEndpointAsync(
        string endpointId,
        UpdateWebhookEndpointRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Deletes a webhook endpoint.
    /// </summary>
    Task DeleteEndpointAsync(
        string endpointId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Rotates the signing secret for an endpoint with an optional 24-hour overlap window.
    /// </summary>
    Task<SecretRotationResult> RotateSecretAsync(
        string endpointId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists webhook events recorded for your organization.
    /// </summary>
    Task<BachsList<WebhookEventRecord>> ListEventsAsync(
        ListRequestOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a specific webhook event by its ID.
    /// </summary>
    Task<WebhookEventRecord> GetEventAsync(
        string eventId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
