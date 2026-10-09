using System.Security.Cryptography;
using System.Text;
using Bachs.Net.WebHooks;

namespace Bachs.Test;

public class BachsWebhookTests
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
        var payload = """{"id":"evt_123","type":"collection.succeeded","created_at":"2026-04-27T12:00:00Z","data":{"amount":"29.00"}}""";
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
}
