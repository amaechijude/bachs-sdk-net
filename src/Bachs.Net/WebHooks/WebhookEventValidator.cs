using System.Diagnostics.CodeAnalysis;
using Bachs.Net.Common;

namespace Bachs.Net.WebHooks;

/// <summary>
/// Standalone implementation of <see cref="IWebhookEventValidator"/> and <see cref="IWebhookValidator"/>
/// for validating incoming Bachs webhook events.
/// </summary>
public sealed class WebhookEventValidator : IWebhookEventValidator, IWebhookValidator
{
    private readonly string? _defaultSecret;

    /// <summary>
    /// Initializes a new instance of <see cref="WebhookEventValidator"/> with an optional default signing secret.
    /// </summary>
    /// <param name="defaultSecret">The default webhook signing secret.</param>
    public WebhookEventValidator(string? defaultSecret = null)
    {
        _defaultSecret = defaultSecret;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="WebhookEventValidator"/> from <see cref="BachsClientOptions"/>.
    /// </summary>
    /// <param name="options">Client options containing the webhook secret.</param>
    public WebhookEventValidator(BachsClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        _defaultSecret = options.WebhookSecret;
    }

    private string ResolveSecret(string? secret)
    {
        if (!string.IsNullOrWhiteSpace(secret))
            return secret;

        if (!string.IsNullOrWhiteSpace(_defaultSecret))
            return _defaultSecret;

        throw new BachsInvalidOperationException(
            "Webhook secret must be provided either via the method parameter or configured default secret."
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
