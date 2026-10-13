using System.Diagnostics.CodeAnalysis;
using Bachs.Net.Common;
using Bachs.Net.Models.Webhooks;
using Bachs.Net.WebHooks;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for managing webhook destination endpoints and events.
/// </summary>
public sealed class WebhooksClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        IWebhooksClient
{
    public Task<WebhookEndpoint> CreateEndpointAsync(
        CreateWebhookEndpointRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<CreateWebhookEndpointRequest, WebhookEndpoint>(
            "v1/webhooks/endpoints",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<WebhookEndpoint> GetEndpointAsync(
        string endpointId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpointId);
        return BachsClient.GetAsync<WebhookEndpoint>(
            $"v1/webhooks/endpoints/{Uri.EscapeDataString(endpointId)}",
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsList<WebhookEndpoint>> ListEndpointsAsync(
        ListWebhookEndpointsOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = QueryBuilder.BuildUrl("v1/webhooks/endpoints", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<WebhookEndpoint>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }

    public Task<WebhookEndpoint> UpdateEndpointAsync(
        string endpointId,
        UpdateWebhookEndpointRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpointId);
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PatchAsync<UpdateWebhookEndpointRequest, WebhookEndpoint>(
            $"v1/webhooks/endpoints/{Uri.EscapeDataString(endpointId)}",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task DeleteEndpointAsync(
        string endpointId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpointId);
        return BachsClient.DeleteAsync(
            $"v1/webhooks/endpoints/{Uri.EscapeDataString(endpointId)}",
            requestOptions,
            cancellationToken
        );
    }

    public Task<SecretRotationResult> RotateSecretAsync(
        string endpointId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(endpointId);
        return BachsClient.PostAsync<SecretRotationResult>(
            $"v1/webhooks/endpoints/{Uri.EscapeDataString(endpointId)}/rotate-secret",
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsList<WebhookEventRecord>> ListEventsAsync(
        ListRequestOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = QueryBuilder.BuildUrl("v1/webhooks/events", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<WebhookEventRecord>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }

    public Task<WebhookEventRecord> GetEventAsync(
        string eventId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(eventId);
        return BachsClient.GetAsync<WebhookEventRecord>(
            $"v1/webhooks/events/{Uri.EscapeDataString(eventId)}",
            requestOptions,
            cancellationToken
        );
    }

    private string ResolveSecret(string? secret)
    {
        if (!string.IsNullOrWhiteSpace(secret))
            return secret;

        if (!string.IsNullOrWhiteSpace(BachsClient.Options.WebhookSecret))
            return BachsClient.Options.WebhookSecret;

        throw new BachsInvalidOperationException(
            "Webhook secret must be provided either via the method parameter or configured in BachsClientOptions.WebhookSecret."
        );
    }

    /// <inheritdoc />
    public BachsWebhookEvent ValidateEvent(
        string payload,
        string signatureHeader,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    ) =>
        BachsWebhook.ValidateEvent(
            payload,
            signatureHeader,
            ResolveSecret(secret),
            toleranceSeconds,
            timestampHeader
        );

    /// <inheritdoc />
    public BachsWebhookEvent ValidateEvent(
        byte[] payloadBytes,
        string signatureHeader,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    ) =>
        BachsWebhook.ValidateEvent(
            payloadBytes,
            signatureHeader,
            ResolveSecret(secret),
            toleranceSeconds,
            timestampHeader
        );

    /// <inheritdoc />
    public bool TryValidateEvent(
        string payload,
        string signatureHeader,
        [NotNullWhen(true)] out BachsWebhookEvent? webhookEvent,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    )
    {
        string resolvedSecret;
        try
        {
            resolvedSecret = ResolveSecret(secret);
        }
        catch
        {
            webhookEvent = null;
            return false;
        }

        return BachsWebhook.TryValidateEvent(
            payload,
            signatureHeader,
            resolvedSecret,
            out webhookEvent,
            toleranceSeconds,
            timestampHeader
        );
    }

    /// <inheritdoc />
    public bool TryValidateEvent(
        byte[] payloadBytes,
        string signatureHeader,
        [NotNullWhen(true)] out BachsWebhookEvent? webhookEvent,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    )
    {
        string resolvedSecret;
        try
        {
            resolvedSecret = ResolveSecret(secret);
        }
        catch
        {
            webhookEvent = null;
            return false;
        }

        return BachsWebhook.TryValidateEvent(
            payloadBytes,
            signatureHeader,
            resolvedSecret,
            out webhookEvent,
            toleranceSeconds,
            timestampHeader
        );
    }

    /// <inheritdoc />
    public bool VerifySignature(
        string payload,
        string signatureHeader,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    ) =>
        BachsWebhook.VerifySignature(
            payload,
            signatureHeader,
            ResolveSecret(secret),
            toleranceSeconds,
            timestampHeader
        );

    /// <inheritdoc />
    public bool VerifySignature(
        byte[] payloadBytes,
        string signatureHeader,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    ) =>
        BachsWebhook.VerifySignature(
            payloadBytes,
            signatureHeader,
            ResolveSecret(secret),
            toleranceSeconds,
            timestampHeader
        );

    /// <inheritdoc />
    public BachsWebhookEvent ConstructEvent(
        string payload,
        string signatureHeader,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    ) => ValidateEvent(payload, signatureHeader, secret, toleranceSeconds, timestampHeader);

    /// <inheritdoc />
    public BachsWebhookEvent ConstructEvent(
        byte[] payloadBytes,
        string signatureHeader,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    ) => ValidateEvent(payloadBytes, signatureHeader, secret, toleranceSeconds, timestampHeader);

    /// <inheritdoc />
    public BachsTypedEvent ValidateTypedEvent(
        string payload,
        string signatureHeader,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    ) =>
        ValidateEvent(payload, signatureHeader, secret, toleranceSeconds, timestampHeader)
            .ToTypedEvent();

    /// <inheritdoc />
    public BachsTypedEvent ValidateTypedEvent(
        byte[] payloadBytes,
        string signatureHeader,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    ) =>
        ValidateEvent(payloadBytes, signatureHeader, secret, toleranceSeconds, timestampHeader)
            .ToTypedEvent();

    /// <inheritdoc />
    public bool TryValidateTypedEvent(
        string payload,
        string signatureHeader,
        [NotNullWhen(true)] out BachsTypedEvent? typedEvent,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    )
    {
        if (
            TryValidateEvent(
                payload,
                signatureHeader,
                out var rawEvent,
                secret,
                toleranceSeconds,
                timestampHeader
            )
        )
        {
            typedEvent = rawEvent.ToTypedEvent();
            return true;
        }

        typedEvent = null;
        return false;
    }

    /// <inheritdoc />
    public bool TryValidateTypedEvent(
        byte[] payloadBytes,
        string signatureHeader,
        [NotNullWhen(true)] out BachsTypedEvent? typedEvent,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    )
    {
        if (
            TryValidateEvent(
                payloadBytes,
                signatureHeader,
                out var rawEvent,
                secret,
                toleranceSeconds,
                timestampHeader
            )
        )
        {
            typedEvent = rawEvent.ToTypedEvent();
            return true;
        }

        typedEvent = null;
        return false;
    }

    /// <inheritdoc />
    public BachsTypedEvent ConstructTypedEvent(
        string payload,
        string signatureHeader,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    ) => ValidateTypedEvent(payload, signatureHeader, secret, toleranceSeconds, timestampHeader);

    /// <inheritdoc />
    public BachsTypedEvent ConstructTypedEvent(
        byte[] payloadBytes,
        string signatureHeader,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    ) =>
        ValidateTypedEvent(
            payloadBytes,
            signatureHeader,
            secret,
            toleranceSeconds,
            timestampHeader
        );
}
