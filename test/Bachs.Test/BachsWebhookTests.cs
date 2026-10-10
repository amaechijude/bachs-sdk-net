using System.Security.Cryptography;
using System.Text;
using Bachs.Net.WebHooks;

namespace Bachs.Test;

public sealed class BachsWebhookTests
{
    private const string Secret = "whsec_test_secret_key_12345";

    private static string ComputeHmac(string payload, long timestamp, string secret)
    {
        var secretBytes = Encoding.UTF8.GetBytes(secret);
        var messageBytes = Encoding.UTF8.GetBytes($"{timestamp}.{payload}");
        using var hmac = new HMACSHA256(secretBytes);
        return Convert.ToHexStringLower(hmac.ComputeHash(messageBytes));
    }

    [Fact]
    public void VerifySignature_Succeeds_For_V2Signature()
    {
        var payload =
            """{"id":"evt_123","type":"collection.succeeded","created_at":"2026-04-27T12:00:00Z","data":{"amount":"29.00"}}""";
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        var isValid = BachsWebhook.VerifySignature(payload, header, Secret);

        Assert.True(isValid);
    }

    [Fact]
    public void VerifySignature_Succeeds_During_SecretRotation_WhenSecondSignatureMatches()
    {
        var payload = """{"id":"evt_123","type":"collection.succeeded"}""";
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var oldSecret = "whsec_old";
        var newSecret = "whsec_new";

        var oldHmac = ComputeHmac(payload, timestamp, oldSecret);
        var newHmac = ComputeHmac(payload, timestamp, newSecret);

        // Header carries both during 24hr rotation
        var header = $"t={timestamp},v1={oldHmac},v1={newHmac}";

        // Verifying with either secret must succeed
        Assert.True(BachsWebhook.VerifySignature(payload, header, oldSecret));
        Assert.True(BachsWebhook.VerifySignature(payload, header, newSecret));
    }

    [Fact]
    public void VerifySignature_Fails_WhenPayloadIsTampered()
    {
        var originalPayload = """{"amount":"29.00"}""";
        var tamperedPayload = """{"amount":"2900.00"}""";
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(originalPayload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        var isValid = BachsWebhook.VerifySignature(tamperedPayload, header, Secret);

        Assert.False(isValid);
    }

    [Fact]
    public void VerifySignature_Fails_WhenTimestampIsStale()
    {
        var payload = """{"id":"evt_123"}""";
        // 10 minutes ago (exceeds default 300s tolerance)
        var staleTimestamp = DateTimeOffset.UtcNow.AddMinutes(-10).ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, staleTimestamp, Secret);
        var header = $"t={staleTimestamp},v1={hmac}";

        var isValid = BachsWebhook.VerifySignature(payload, header, Secret, toleranceSeconds: 300);

        Assert.False(isValid);
    }

    [Fact]
    public void ConstructEvent_Parses_Payload_Successfully()
    {
        var payload = """
            {
                "id": "evt_3ab4e0d5d2",
                "type": "collection.succeeded",
                "created_at": "2026-04-27T12:04:00Z",
                "organization_id": "acct_7KpQ2mNv4XbR9dLc",
                "data": {
                    "charge_id": "ch_1a2b3c4d5e6f",
                    "amount": "29.00",
                    "currency": "USD"
                }
            }
            """;
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        var evt = BachsWebhook.ConstructEvent(payload, header, Secret);

        Assert.NotNull(evt);
        Assert.Equal("evt_3ab4e0d5d2", evt.Id);
        Assert.Equal("collection.succeeded", evt.Type);
        Assert.Equal("acct_7KpQ2mNv4XbR9dLc", evt.OrganizationId);

        var data = evt.GetData<Dictionary<string, string>>();
        Assert.NotNull(data);
        Assert.Equal("ch_1a2b3c4d5e6f", data["charge_id"]);
        Assert.Equal("29.00", data["amount"]);
        Assert.Equal("USD", data["currency"]);
    }

    [Fact]
    public void ConstructEvent_And_ValidateEvent_Succeed_With_ByteArrayPayload()
    {
        var payload = """{"id":"evt_bytes_123","type":"checkout.completed"}""";
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        var evt1 = BachsWebhook.ConstructEvent(payloadBytes, header, Secret);
        var evt2 = BachsWebhook.ValidateEvent(payloadBytes, header, Secret);

        Assert.NotNull(evt1);
        Assert.Equal("evt_bytes_123", evt1.Id);
        Assert.NotNull(evt2);
        Assert.Equal("evt_bytes_123", evt2.Id);
    }

    [Fact]
    public void TryValidateEvent_Returns_True_When_Valid()
    {
        var payload = """{"id":"evt_try_123","type":"refund.succeeded"}""";
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        var result = BachsWebhook.TryValidateEvent(payload, header, Secret, out var evt);

        Assert.True(result);
        Assert.NotNull(evt);
        Assert.Equal("evt_try_123", evt.Id);
        Assert.Equal("refund.succeeded", evt.Type);
    }

    [Fact]
    public void TryValidateEvent_Returns_False_When_Invalid()
    {
        var payload = """{"id":"evt_try_123","type":"refund.succeeded"}""";
        var header = "t=12345,v1=invalidhmac";

        var result = BachsWebhook.TryValidateEvent(payload, header, Secret, out var evt);

        Assert.False(result);
        Assert.Null(evt);
    }

    [Fact]
    public void WebhooksClient_ValidateEvent_Uses_Configured_WebhookSecret()
    {
        var options = new Bachs.Net.Common.BachsClientOptions
        {
            ApiKey = "sk_sandbox_test123",
            WebhookSecret = Secret,
        };
        using var client = new Bachs.Net.Clients.BachsClient(options);

        var payload = """{"id":"evt_client_123","type":"collection.succeeded"}""";
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        // Call without passing secret explicitly
        var evt = client.Webhooks.ValidateEvent(payload, header);

        Assert.NotNull(evt);
        Assert.Equal("evt_client_123", evt.Id);
        Assert.Equal("collection.succeeded", evt.Type);
    }

    [Fact]
    public void WebhooksClient_ValidateEvent_Prefers_Explicit_Secret()
    {
        var options = new Bachs.Net.Common.BachsClientOptions
        {
            ApiKey = "sk_sandbox_test123",
            WebhookSecret = "whsec_other_secret",
        };
        using var client = new Bachs.Net.Clients.BachsClient(options);

        var payload = """{"id":"evt_client_custom","type":"payment.captured"}""";
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        // Pass explicit secret overriding options
        var evt = client.Webhooks.ValidateEvent(payload, header, secret: Secret);

        Assert.NotNull(evt);
        Assert.Equal("evt_client_custom", evt.Id);
    }

    [Fact]
    public void WebhooksClient_ValidateEvent_Throws_When_No_Secret_Provided_Or_Configured()
    {
        var options = new Bachs.Net.Common.BachsClientOptions
        {
            ApiKey = "sk_sandbox_test123",
            WebhookSecret = null,
        };
        using var client = new Bachs.Net.Clients.BachsClient(options);

        var payload = """{"id":"evt_client_123","type":"collection.succeeded"}""";
        var header = "t=123,v1=abc";

        Assert.Throws<InvalidOperationException>(() => client.Webhooks.ValidateEvent(payload, header));
    }

    [Fact]
    public void WebhooksClient_Supports_BytePayload_And_TryValidateEvent()
    {
        var options = new Bachs.Net.Common.BachsClientOptions
        {
            ApiKey = "sk_sandbox_test123",
            WebhookSecret = Secret,
        };
        using var client = new Bachs.Net.Clients.BachsClient(options);

        var payload = """{"id":"evt_bytes_client","type":"invoice.paid"}""";
        var payloadBytes = Encoding.UTF8.GetBytes(payload);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        var evt = client.Webhooks.ValidateEvent(payloadBytes, header);
        Assert.NotNull(evt);
        Assert.Equal("evt_bytes_client", evt.Id);

        var isValid = client.Webhooks.VerifySignature(payloadBytes, header);
        Assert.True(isValid);

        var trySuccess = client.Webhooks.TryValidateEvent(payloadBytes, header, out var tryEvt);
        Assert.True(trySuccess);
        Assert.NotNull(tryEvt);
        Assert.Equal("evt_bytes_client", tryEvt.Id);
    }

    [Fact]
    public void Standalone_WebhookEventValidator_Works_As_Expected()
    {
        IWebhookEventValidator validator = new WebhookEventValidator(Secret);

        var payload = """{"id":"evt_standalone","type":"transfer.created"}""";
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        var evt = validator.ValidateEvent(payload, header);
        Assert.NotNull(evt);
        Assert.Equal("evt_standalone", evt.Id);
        Assert.True(validator.VerifySignature(payload, header));

        var tryValid = validator.TryValidateEvent(payload, header, out var parsed);
        Assert.True(tryValid);
        Assert.NotNull(parsed);
    }
}
