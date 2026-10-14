using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using Bachs.Net.Common;
using Bachs.Net.Serialization;

namespace Bachs.Net.WebHooks;

/// <summary>
/// Helper for verifying Bachs webhook signatures and parsing events.
/// </summary>
public static class BachsWebhook
{
    public const int DefaultToleranceSeconds = 300;

    /// <summary>
    /// Verifies the Bachs webhook signature header (supports both X-Bachs-Signature-V2 and X-Bachs-Signature).
    /// </summary>
    /// <param name="payload">The raw, unmodified request body string.</param>
    /// <param name="signatureHeader">The signature header value (from 'X-Bachs-Signature-V2' or 'X-Bachs-Signature').</param>
    /// <param name="secret">Your webhook signing secret.</param>
    /// <param name="toleranceSeconds">Maximum allowed drift in seconds between delivery timestamp and current time. Defaults to 300 (5 minutes).</param>
    /// <param name="timestampHeader">Optional 'X-Bachs-Timestamp' header when using V1 signatures.</param>
    /// <returns>True if signature and timestamp are valid; otherwise false.</returns>
    public static bool VerifySignature(
        string payload,
        string signatureHeader,
        string secret,
        int toleranceSeconds = DefaultToleranceSeconds,
        string? timestampHeader = null
    )
    {
        ArgumentNullException.ThrowIfNull(payload);
        ArgumentNullException.ThrowIfNull(signatureHeader);
        ArgumentNullException.ThrowIfNull(secret);

        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        return VerifySignature(
            payloadBytes,
            signatureHeader,
            secret,
            toleranceSeconds,
            timestampHeader
        );
    }

    /// <summary>
    /// Verifies the Bachs webhook signature using raw payload bytes.
    /// </summary>
    public static bool VerifySignature(
        byte[] payloadBytes,
        string signatureHeader,
        string secret,
        int toleranceSeconds = DefaultToleranceSeconds,
        string? timestampHeader = null
    )
    {
        ArgumentNullException.ThrowIfNull(payloadBytes);
        ArgumentNullException.ThrowIfNull(signatureHeader);
        ArgumentNullException.ThrowIfNull(secret);

        // Check if header is V2 format (contains "t=" and "v1=")
        if (signatureHeader.Contains("t=") && signatureHeader.Contains("v1="))
        {
            return VerifyV2Signature(payloadBytes, signatureHeader, secret, toleranceSeconds);
        }

        // Fallback to V1 format (signatureHeader is raw HMAC hex digest and timestampHeader is provided)
        if (
            !string.IsNullOrWhiteSpace(timestampHeader)
            && long.TryParse(timestampHeader, out var timestamp)
        )
        {
            return VerifyV1Signature(
                payloadBytes,
                signatureHeader,
                timestamp,
                secret,
                toleranceSeconds
            );
        }

        return false;
    }

    private static bool VerifyV2Signature(
        byte[] payloadBytes,
        string header,
        string secret,
        int toleranceSeconds
    )
    {
        var parts = header.Split(',');
        long? timestamp = null;
        var signatures = new List<string>();

        foreach (var part in parts)
        {
            var kv = part.Split('=', 2);
            if (kv.Length != 2)
                continue;

            var key = kv[0].Trim();
            var value = kv[1].Trim();

            if (key == "t" && long.TryParse(value, out var t))
            {
                timestamp = t;
            }
            else if (key == "v1")
            {
                signatures.Add(value);
            }
        }

        if (!timestamp.HasValue || signatures.Count == 0)
        {
            return false;
        }

        // Reject stale deliveries
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (Math.Abs(now - timestamp.Value) > toleranceSeconds)
        {
            return false;
        }

        var expectedHex = ComputeHmac(payloadBytes, timestamp.Value, secret);
        var expectedBytes = Encoding.UTF8.GetBytes(expectedHex);

        foreach (var sig in signatures)
        {
            var sigBytes = Encoding.UTF8.GetBytes(sig);
            if (
                sigBytes.Length == expectedBytes.Length
                && CryptographicOperations.FixedTimeEquals(sigBytes, expectedBytes)
            )
            {
                return true;
            }
        }

        return false;
    }

    private static bool VerifyV1Signature(
        byte[] payloadBytes,
        string signature,
        long timestamp,
        string secret,
        int toleranceSeconds
    )
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (Math.Abs(now - timestamp) > toleranceSeconds)
            return false;

        var expectedHex = ComputeHmac(payloadBytes, timestamp, secret);
        var expectedBytes = Encoding.UTF8.GetBytes(expectedHex);
        var sigBytes = Encoding.UTF8.GetBytes(signature.Trim());

        return sigBytes.Length == expectedBytes.Length
            && CryptographicOperations.FixedTimeEquals(sigBytes, expectedBytes);
    }

    private static string ComputeHmac(byte[] payloadBytes, long timestamp, string secret)
    {
        var secretBytes = Encoding.UTF8.GetBytes(secret);
        var prefixBytes = Encoding.UTF8.GetBytes($"{timestamp}.");
        var combinedBytes = new byte[prefixBytes.Length + payloadBytes.Length];

        Buffer.BlockCopy(prefixBytes, 0, combinedBytes, 0, prefixBytes.Length);
        Buffer.BlockCopy(payloadBytes, 0, combinedBytes, prefixBytes.Length, payloadBytes.Length);

        using var hmac = new HMACSHA256(secretBytes);
        var hash = hmac.ComputeHash(combinedBytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    /// <summary>
    /// Verifies the webhook signature and deserializes the payload into a <see cref="BachsWebhookEvent"/>.
    /// </summary>
    /// <param name="payload">The raw request body string.</param>
    /// <param name="signatureHeader">The signature header value.</param>
    /// <param name="secret">Your webhook signing secret.</param>
    /// <param name="toleranceSeconds">Timestamp tolerance in seconds.</param>
    /// <param name="timestampHeader">Optional timestamp header for V1 signatures.</param>
    /// <returns>The verified and parsed <see cref="BachsWebhookEvent"/>.</returns>
    /// <exception cref="BachsException">Thrown if signature verification fails.</exception>
    public static BachsWebhookEvent ConstructEvent(
        string payload,
        string signatureHeader,
        string secret,
        int toleranceSeconds = DefaultToleranceSeconds,
        string? timestampHeader = null
    )
    {
        if (!VerifySignature(payload, signatureHeader, secret, toleranceSeconds, timestampHeader))
        {
            throw new BachsException(
                "Webhook signature verification failed. The signature does not match or is stale."
            );
        }

        var evt = BachsJsonSerializer.Deserialize<BachsWebhookEvent>(payload);
        if (evt == null)
        {
            throw new BachsException(
                "Unable to deserialize webhook payload into a BachsWebhookEvent."
            );
        }

        return evt;
    }

    /// <summary>
    /// Verifies the webhook signature and deserializes the payload bytes into a <see cref="BachsWebhookEvent"/>.
    /// </summary>
    /// <param name="payloadBytes">The raw request body bytes.</param>
    /// <param name="signatureHeader">The signature header value.</param>
    /// <param name="secret">Your webhook signing secret.</param>
    /// <param name="toleranceSeconds">Timestamp tolerance in seconds.</param>
    /// <param name="timestampHeader">Optional timestamp header for V1 signatures.</param>
    /// <returns>The verified and parsed <see cref="BachsWebhookEvent"/>.</returns>
    /// <exception cref="BachsException">Thrown if signature verification fails.</exception>
    public static BachsWebhookEvent ConstructEvent(
        byte[] payloadBytes,
        string signatureHeader,
        string secret,
        int toleranceSeconds = DefaultToleranceSeconds,
        string? timestampHeader = null
    )
    {
        ArgumentNullException.ThrowIfNull(payloadBytes);
        if (
            !VerifySignature(
                payloadBytes,
                signatureHeader,
                secret,
                toleranceSeconds,
                timestampHeader
            )
        )
        {
            throw new BachsException(
                "Webhook signature verification failed. The signature does not match or is stale."
            );
        }

        var payload = Encoding.UTF8.GetString(payloadBytes);
        var evt =
            BachsJsonSerializer.Deserialize<BachsWebhookEvent>(payload)
            ?? throw new BachsException(
                "Unable to deserialize webhook payload into a BachsWebhookEvent."
            );
        return evt;
    }

    /// <summary>
    /// Validates the webhook signature and deserializes the payload into a <see cref="BachsWebhookEvent"/>.
    /// </summary>
    /// <param name="payload">The raw request body string.</param>
    /// <param name="signatureHeader">The signature header value.</param>
    /// <param name="secret">Your webhook signing secret.</param>
    /// <param name="toleranceSeconds">Timestamp tolerance in seconds.</param>
    /// <param name="timestampHeader">Optional timestamp header for V1 signatures.</param>
    /// <returns>The verified and parsed <see cref="BachsWebhookEvent"/>.</returns>
    /// <exception cref="BachsException">Thrown if signature verification fails.</exception>
    public static BachsWebhookEvent ValidateEvent(
        string payload,
        string signatureHeader,
        string secret,
        int toleranceSeconds = DefaultToleranceSeconds,
        string? timestampHeader = null
    ) => ConstructEvent(payload, signatureHeader, secret, toleranceSeconds, timestampHeader);

    /// <summary>
    /// Validates the webhook signature and deserializes the payload bytes into a <see cref="BachsWebhookEvent"/>.
    /// </summary>
    /// <param name="payloadBytes">The raw request body bytes.</param>
    /// <param name="signatureHeader">The signature header value.</param>
    /// <param name="secret">Your webhook signing secret.</param>
    /// <param name="toleranceSeconds">Timestamp tolerance in seconds.</param>
    /// <param name="timestampHeader">Optional timestamp header for V1 signatures.</param>
    /// <returns>The verified and parsed <see cref="BachsWebhookEvent"/>.</returns>
    /// <exception cref="BachsException">Thrown if signature verification fails.</exception>
    public static BachsWebhookEvent ValidateEvent(
        byte[] payloadBytes,
        string signatureHeader,
        string secret,
        int toleranceSeconds = DefaultToleranceSeconds,
        string? timestampHeader = null
    ) => ConstructEvent(payloadBytes, signatureHeader, secret, toleranceSeconds, timestampHeader);

    /// <summary>
    /// Attempts to validate the incoming webhook payload and signature without throwing exceptions on validation failures.
    /// </summary>
    public static bool TryValidateEvent(
        string payload,
        string signatureHeader,
        string secret,
        [NotNullWhen(true)] out BachsWebhookEvent? webhookEvent,
        int toleranceSeconds = DefaultToleranceSeconds,
        string? timestampHeader = null
    )
    {
        ArgumentNullException.ThrowIfNull(payload);
        try
        {
            if (
                !VerifySignature(
                    payload,
                    signatureHeader,
                    secret,
                    toleranceSeconds,
                    timestampHeader
                )
            )
            {
                webhookEvent = null;
                return false;
            }

            webhookEvent = BachsJsonSerializer.Deserialize<BachsWebhookEvent>(payload);
            return webhookEvent != null;
        }
        catch
        {
            webhookEvent = null;
            return false;
        }
    }

    /// <summary>
    /// Attempts to validate incoming webhook payload bytes and signature without throwing exceptions on validation failures.
    /// </summary>
    public static bool TryValidateEvent(
        byte[] payloadBytes,
        string signatureHeader,
        string secret,
        [NotNullWhen(true)] out BachsWebhookEvent? webhookEvent,
        int toleranceSeconds = DefaultToleranceSeconds,
        string? timestampHeader = null
    )
    {
        ArgumentNullException.ThrowIfNull(payloadBytes);
        try
        {
            if (
                !VerifySignature(
                    payloadBytes,
                    signatureHeader,
                    secret,
                    toleranceSeconds,
                    timestampHeader
                )
            )
            {
                webhookEvent = null;
                return false;
            }

            var payload = Encoding.UTF8.GetString(payloadBytes);
            webhookEvent = BachsJsonSerializer.Deserialize<BachsWebhookEvent>(payload);
            return webhookEvent != null;
        }
        catch
        {
            webhookEvent = null;
            return false;
        }
    }

    /// <summary>
    /// Verifies the webhook signature and deserializes the payload into a strongly-typed <see cref="BachsTypedEvent"/>.
    /// </summary>
    public static BachsTypedEvent ConstructTypedEvent(
        string payload,
        string signatureHeader,
        string secret,
        int toleranceSeconds = DefaultToleranceSeconds,
        string? timestampHeader = null
    )
    {
        var rawEvent = ConstructEvent(
            payload,
            signatureHeader,
            secret,
            toleranceSeconds,
            timestampHeader
        );
        return rawEvent.ToTypedEvent();
    }

    /// <summary>
    /// Verifies the webhook signature and deserializes the payload bytes into a strongly-typed <see cref="BachsTypedEvent"/>.
    /// </summary>
    public static BachsTypedEvent ConstructTypedEvent(
        byte[] payloadBytes,
        string signatureHeader,
        string secret,
        int toleranceSeconds = DefaultToleranceSeconds,
        string? timestampHeader = null
    )
    {
        var rawEvent = ConstructEvent(
            payloadBytes,
            signatureHeader,
            secret,
            toleranceSeconds,
            timestampHeader
        );
        return rawEvent.ToTypedEvent();
    }

    /// <summary>
    /// Validates the webhook signature and deserializes the payload into a strongly-typed <see cref="BachsTypedEvent"/>.
    /// </summary>
    public static BachsTypedEvent ValidateTypedEvent(
        string payload,
        string signatureHeader,
        string secret,
        int toleranceSeconds = DefaultToleranceSeconds,
        string? timestampHeader = null
    ) => ConstructTypedEvent(payload, signatureHeader, secret, toleranceSeconds, timestampHeader);

    /// <summary>
    /// Validates the webhook signature and deserializes the payload bytes into a strongly-typed <see cref="BachsTypedEvent"/>.
    /// </summary>
    public static BachsTypedEvent ValidateTypedEvent(
        byte[] payloadBytes,
        string signatureHeader,
        string secret,
        int toleranceSeconds = DefaultToleranceSeconds,
        string? timestampHeader = null
    ) =>
        ConstructTypedEvent(
            payloadBytes,
            signatureHeader,
            secret,
            toleranceSeconds,
            timestampHeader
        );

    /// <summary>
    /// Attempts to validate incoming webhook payload and signature into a strongly-typed <see cref="BachsTypedEvent"/> without throwing exceptions.
    /// </summary>
    public static bool TryValidateTypedEvent(
        string payload,
        string signatureHeader,
        string secret,
        [NotNullWhen(true)] out BachsTypedEvent? typedEvent,
        int toleranceSeconds = DefaultToleranceSeconds,
        string? timestampHeader = null
    )
    {
        if (
            TryValidateEvent(
                payload,
                signatureHeader,
                secret,
                out var rawEvent,
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

    /// <summary>
    /// Attempts to validate incoming webhook payload bytes and signature into a strongly-typed <see cref="BachsTypedEvent"/> without throwing exceptions.
    /// </summary>
    public static bool TryValidateTypedEvent(
        byte[] payloadBytes,
        string signatureHeader,
        string secret,
        [NotNullWhen(true)] out BachsTypedEvent? typedEvent,
        int toleranceSeconds = DefaultToleranceSeconds,
        string? timestampHeader = null
    )
    {
        if (
            TryValidateEvent(
                payloadBytes,
                signatureHeader,
                secret,
                out var rawEvent,
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
}
