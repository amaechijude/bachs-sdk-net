using System.Diagnostics.CodeAnalysis;

namespace Bachs.Net.WebHooks;

/// <summary>
/// Exposes an interface to validate incoming Bachs webhook events, verify signatures,
/// and construct strongly-typed <see cref="BachsWebhookEvent"/> objects.
/// </summary>
public interface IWebhookEventValidator
{
    /// <summary>
    /// Validates an incoming webhook event payload and signature, returning the parsed <see cref="BachsWebhookEvent"/>.
    /// </summary>
    /// <param name="payload">The raw, unmodified request body string.</param>
    /// <param name="signatureHeader">The signature header value (from 'X-Bachs-Signature-V2' or 'X-Bachs-Signature').</param>
    /// <param name="secret">Optional webhook signing secret. If not specified, the configured default secret will be used.</param>
    /// <param name="toleranceSeconds">Maximum allowed drift in seconds between delivery timestamp and current time. Defaults to 300 (5 minutes).</param>
    /// <param name="timestampHeader">Optional 'X-Bachs-Timestamp' header when using V1 signatures.</param>
    /// <returns>The verified and parsed <see cref="BachsWebhookEvent"/>.</returns>
    /// <exception cref="Common.BachsException">Thrown if signature verification fails or payload is invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown if secret is not provided and no default secret is configured.</exception>
    BachsWebhookEvent ValidateEvent(
        string payload,
        string signatureHeader,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    );

    /// <summary>
    /// Validates an incoming webhook event payload bytes and signature, returning the parsed <see cref="BachsWebhookEvent"/>.
    /// </summary>
    /// <param name="payloadBytes">The raw request body bytes.</param>
    /// <param name="signatureHeader">The signature header value (from 'X-Bachs-Signature-V2' or 'X-Bachs-Signature').</param>
    /// <param name="secret">Optional webhook signing secret. If not specified, the configured default secret will be used.</param>
    /// <param name="toleranceSeconds">Maximum allowed drift in seconds between delivery timestamp and current time. Defaults to 300 (5 minutes).</param>
    /// <param name="timestampHeader">Optional 'X-Bachs-Timestamp' header when using V1 signatures.</param>
    /// <returns>The verified and parsed <see cref="BachsWebhookEvent"/>.</returns>
    /// <exception cref="Common.BachsException">Thrown if signature verification fails or payload is invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown if secret is not provided and no default secret is configured.</exception>
    BachsWebhookEvent ValidateEvent(
        byte[] payloadBytes,
        string signatureHeader,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    );

    /// <summary>
    /// Attempts to validate the incoming webhook payload and signature without throwing exceptions on validation failures.
    /// </summary>
    /// <param name="payload">The raw, unmodified request body string.</param>
    /// <param name="signatureHeader">The signature header value.</param>
    /// <param name="webhookEvent">When this method returns, contains the parsed event if valid; otherwise, null.</param>
    /// <param name="secret">Optional webhook signing secret. If not specified, the configured default secret will be used.</param>
    /// <param name="toleranceSeconds">Maximum allowed drift in seconds.</param>
    /// <param name="timestampHeader">Optional 'X-Bachs-Timestamp' header when using V1 signatures.</param>
    /// <returns>True if signature and payload are valid; otherwise false.</returns>
    bool TryValidateEvent(
        string payload,
        string signatureHeader,
        [NotNullWhen(true)] out BachsWebhookEvent? webhookEvent,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    );

    /// <summary>
    /// Attempts to validate incoming webhook payload bytes and signature without throwing exceptions on validation failures.
    /// </summary>
    /// <param name="payloadBytes">The raw request body bytes.</param>
    /// <param name="signatureHeader">The signature header value.</param>
    /// <param name="webhookEvent">When this method returns, contains the parsed event if valid; otherwise, null.</param>
    /// <param name="secret">Optional webhook signing secret. If not specified, the configured default secret will be used.</param>
    /// <param name="toleranceSeconds">Maximum allowed drift in seconds.</param>
    /// <param name="timestampHeader">Optional 'X-Bachs-Timestamp' header when using V1 signatures.</param>
    /// <returns>True if signature and payload are valid; otherwise false.</returns>
    bool TryValidateEvent(
        byte[] payloadBytes,
        string signatureHeader,
        [NotNullWhen(true)] out BachsWebhookEvent? webhookEvent,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    );

    /// <summary>
    /// Verifies the Bachs webhook signature header against a payload string.
    /// </summary>
    /// <param name="payload">The raw, unmodified request body string.</param>
    /// <param name="signatureHeader">The signature header value.</param>
    /// <param name="secret">Optional webhook signing secret. If not specified, the configured default secret will be used.</param>
    /// <param name="toleranceSeconds">Maximum allowed drift in seconds.</param>
    /// <param name="timestampHeader">Optional 'X-Bachs-Timestamp' header when using V1 signatures.</param>
    /// <returns>True if the signature is valid; otherwise false.</returns>
    bool VerifySignature(
        string payload,
        string signatureHeader,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    );

    /// <summary>
    /// Verifies the Bachs webhook signature header against raw payload bytes.
    /// </summary>
    /// <param name="payloadBytes">The raw request body bytes.</param>
    /// <param name="signatureHeader">The signature header value.</param>
    /// <param name="secret">Optional webhook signing secret. If not specified, the configured default secret will be used.</param>
    /// <param name="toleranceSeconds">Maximum allowed drift in seconds.</param>
    /// <param name="timestampHeader">Optional 'X-Bachs-Timestamp' header when using V1 signatures.</param>
    /// <returns>True if the signature is valid; otherwise false.</returns>
    bool VerifySignature(
        byte[] payloadBytes,
        string signatureHeader,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    );

    /// <summary>
    /// Verifies the webhook signature and deserializes the payload into a <see cref="BachsWebhookEvent"/>.
    /// </summary>
    BachsWebhookEvent ConstructEvent(
        string payload,
        string signatureHeader,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    );

    /// <summary>
    /// Verifies the webhook signature and deserializes the payload bytes into a <see cref="BachsWebhookEvent"/>.
    /// </summary>
    BachsWebhookEvent ConstructEvent(
        byte[] payloadBytes,
        string signatureHeader,
        string? secret = null,
        int toleranceSeconds = BachsWebhook.DefaultToleranceSeconds,
        string? timestampHeader = null
    );
}

/// <summary>
/// Alias interface for <see cref="IWebhookEventValidator"/>.
/// </summary>
public interface IWebhookValidator : IWebhookEventValidator { }
