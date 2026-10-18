using System.Security.Cryptography;
using System.Text;
using Bachs.Common;
using Bachs.Exceptions;
using Bachs.WebHooks;

namespace Bachs.Test;

public sealed class BachsWebhookTests
{
    private const string Secret = "whsec_test_secret_key_12345";

    private static string ComputeHmac(string payload, long timestamp, string secret)
    {
        var secretBytes = Encoding.UTF8.GetBytes(secret);
        var messageBytes = Encoding.UTF8.GetBytes($"{timestamp}.{payload}");
        using var hmac = new HMACSHA256(secretBytes);
        return Convert.ToHexString(hmac.ComputeHash(messageBytes)).ToLowerInvariant();
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
        var options = new Bachs.Common.BachsClientOptions
        {
            ApiKey = "sk_sandbox_test123",
            WebhookSecret = Secret,
        };
        using var client = new Bachs.Clients.BachsClient(options);

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
        var options = new Bachs.Common.BachsClientOptions
        {
            ApiKey = "sk_sandbox_test123",
            WebhookSecret = "whsec_other_secret",
        };
        using var client = new Bachs.Clients.BachsClient(options);

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
        var options = new Bachs.Common.BachsClientOptions
        {
            ApiKey = "sk_sandbox_test123",
            WebhookSecret = null,
        };
        using var client = new Bachs.Clients.BachsClient(options);

        var payload = """{"id":"evt_client_123","type":"collection.succeeded"}""";
        var header = "t=123,v1=abc";

        Assert.Throws<BachsInvalidOperationException>(() =>
            client.Webhooks.ValidateEvent(payload, header)
        );
    }

    [Fact]
    public void WebhooksClient_Supports_BytePayload_And_TryValidateEvent()
    {
        var options = new Bachs.Common.BachsClientOptions
        {
            ApiKey = "sk_sandbox_test123",
            WebhookSecret = Secret,
        };
        using var client = new Bachs.Clients.BachsClient(options);

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

    [Fact]
    public void BachsEventTypes_Contains_Expected_Constants()
    {
        Assert.Equal("collection.succeeded", BachsEventTypes.CollectionSucceeded);
        Assert.Equal("checkout.completed", BachsEventTypes.CheckoutCompleted);
        Assert.Equal("payout.paid", BachsEventTypes.PayoutPaid);
        Assert.Equal("refund.paid", BachsEventTypes.RefundPaid);
        Assert.Equal("customer.subscription.created", BachsEventTypes.CustomerSubscriptionCreated);
        Assert.Equal("invoice.paid", BachsEventTypes.InvoicePaid);
        Assert.Equal("account.updated", BachsEventTypes.AccountUpdated);
        Assert.Equal("transfer.created", BachsEventTypes.TransferCreated);

        Assert.Contains(BachsEventTypes.CollectionSucceeded, BachsEventTypes.All);
        Assert.Contains(BachsEventTypes.CheckoutCompleted, BachsEventTypes.All);
    }

    [Fact]
    public void ConstructTypedEvent_Parses_CollectionSucceeded_To_CollectionSucceededEvent()
    {
        var payload = """
            {
                "id": "evt_coll_123",
                "type": "collection.succeeded",
                "created_at": "2026-09-22T10:41:18Z",
                "organization_id": "acct_org123",
                "data": {
                    "charge_id": "ch_charge123",
                    "checkout_id": "chk_sess123",
                    "reference": "ORD-1234",
                    "status": "SUCCEEDED",
                    "amount": "75000.00",
                    "currency": "NGN",
                    "settlement_amount": "73875.00",
                    "settlement_currency": "NGN",
                    "processing_fee": "1125.00",
                    "processing_fee_currency": "NGN",
                    "fee_bearer": "merchant"
                }
            }
            """;
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        var typedEvent = BachsWebhook.ConstructTypedEvent(payload, header, Secret);

        var collEvent = Assert.IsType<CollectionSucceededEvent>(typedEvent);
        Assert.Equal("evt_coll_123", collEvent.Id);
        Assert.Equal(BachsEventTypes.CollectionSucceeded, collEvent.Type);
        Assert.Equal("ch_charge123", collEvent.Data.ChargeId);
        Assert.Equal("ch_charge123", collEvent.Data.Id);
        Assert.Equal("chk_sess123", collEvent.Data.CheckoutId);
        Assert.Equal("ORD-1234", collEvent.Data.Reference);
        Assert.Equal(75000.00m, collEvent.Data.Amount);
        Assert.Equal(Bachs.Models.Common.BachsCurrency.NGN, collEvent.Data.Currency);
        Assert.Equal(73875.00m, (decimal)collEvent.Data.SettlementAmount!.Value);
        Assert.Equal("merchant", collEvent.Data.FeeBearer);
    }

    [Fact]
    public void ConstructTypedEvent_Parses_CheckoutCompleted_To_CheckoutCompletedEvent()
    {
        var payload = """
            {
                "id": "evt_chk_123",
                "type": "checkout.completed",
                "created_at": "2026-07-20T09:15:00Z",
                "organization_id": "acct_org123",
                "data": {
                    "checkout_id": "chk_session_abc",
                    "status": "completed",
                    "amount": "19.00",
                    "currency": "USD",
                    "payment_status": "paid"
                }
            }
            """;
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        var typedEvent = BachsWebhook.ConstructTypedEvent(payload, header, Secret);

        var chkEvent = Assert.IsType<CheckoutCompletedEvent>(typedEvent);
        Assert.Equal("evt_chk_123", chkEvent.Id);
        Assert.Equal("chk_session_abc", chkEvent.Data.CheckoutId);
        Assert.Equal("chk_session_abc", chkEvent.Data.Id);
        Assert.Equal("completed", chkEvent.Data.Status);
        Assert.Equal(19.00m, (decimal)chkEvent.Data.Amount!.Value);
    }

    [Fact]
    public void ConstructTypedEvent_Parses_PayoutPaid_To_PayoutPaidEvent()
    {
        var payload = """
            {
                "id": "evt_payout_123",
                "type": "payout.paid",
                "created_at": "2026-04-27T12:00:00Z",
                "organization_id": "acct_org123",
                "data": {
                    "withdrawal_id": "pay_with123",
                    "amount": "500.00",
                    "currency": "USD",
                    "status": "completed",
                    "from_currency": "USD",
                    "to_currency": "NGN",
                    "exchange_rate": 1650.00,
                    "to_amount": "825000.00"
                }
            }
            """;
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        var typedEvent = BachsWebhook.ConstructTypedEvent(payload, header, Secret);

        var payoutEvent = Assert.IsType<PayoutPaidEvent>(typedEvent);
        Assert.Equal("evt_payout_123", payoutEvent.Id);
        Assert.Equal("pay_with123", payoutEvent.Data.WithdrawalId);
        Assert.Equal("pay_with123", payoutEvent.Data.Id);
        Assert.Equal(500.00m, (decimal)payoutEvent.Data.Amount);
        Assert.Equal(1650.00m, payoutEvent.Data.ExchangeRate);
        Assert.Equal(825000.00m, (decimal)payoutEvent.Data.ToAmount!.Value);
    }

    [Fact]
    public void ConstructTypedEvent_Parses_RefundPaid_To_RefundPaidEvent()
    {
        var payload = """
            {
                "id": "evt_ref_123",
                "type": "refund.paid",
                "created_at": "2026-04-27T12:00:00Z",
                "organization_id": "acct_org123",
                "data": {
                    "refund_id": "ref_98765",
                    "charge_id": "ch_orig123",
                    "reference": "ref_order_1",
                    "status": "paid",
                    "amount": "10.00",
                    "requested_amount": "10.00",
                    "refunded_amount": "10.00",
                    "fee_bearer": "merchant"
                }
            }
            """;
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        var typedEvent = BachsWebhook.ConstructTypedEvent(payload, header, Secret);

        var refEvent = Assert.IsType<RefundPaidEvent>(typedEvent);
        Assert.Equal("evt_ref_123", refEvent.Id);
        Assert.Equal("ref_98765", refEvent.Data.RefundId);
        Assert.Equal("ref_98765", refEvent.Data.Id);
        Assert.Equal("ch_orig123", refEvent.Data.ChargeId);
        Assert.Equal(10.00m, refEvent.Data.Amount);
    }

    [Fact]
    public void ConstructTypedEvent_Parses_SubscriptionCreated_To_SubscriptionCreatedEvent()
    {
        var payload = """
            {
                "id": "evt_sub_123",
                "type": "customer.subscription.created",
                "created_at": "2026-04-27T12:00:00Z",
                "organization_id": "acct_org123",
                "data": {
                    "subscription_id": "sub_premium_123",
                    "product_id": "prod_monthly",
                    "status": "active",
                    "customer":{
                    "customer_id": "cust_user123"
                    }
                }
            }
            """;
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        var typedEvent = BachsWebhook.ConstructTypedEvent(payload, header, Secret);

        var subEvent = Assert.IsType<SubscriptionCreatedEvent>(typedEvent);
        Assert.Equal("evt_sub_123", subEvent.Id);
        Assert.Equal("sub_premium_123", subEvent.Data.SubscriptionId);
        Assert.Equal("sub_premium_123", subEvent.Data.Id);
        Assert.Equal("cust_user123", subEvent.Data.Customer?.CustomerId);
        Assert.Equal("active", subEvent.Data.Status);
    }

    [Fact]
    public void ConstructTypedEvent_Parses_InvoiceCreated_To_InvoiceCreatedEvent()
    {
        var payload = """
            {
                "id": "evt_inv_123",
                "type": "invoice.created",
                "created_at": "2026-04-27T12:00:00Z",
                "organization_id": "acct_org123",
                "data": {
                    "invoice_id": "inv_bill_456",
                    "status": "open",
                    "currency": "USD",
                    "total": "29.00",
                    "amount_remaining": "29.00",
                    "subscription": { "subscription_id": "sub_premium_123" },
                    "customer": { "customer_id": "cust_123", "email": "test@example.com", "name": "Test User" }
                }
            }
            """;
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        var typedEvent = BachsWebhook.ConstructTypedEvent(payload, header, Secret);

        var invEvent = Assert.IsType<InvoiceCreatedEvent>(typedEvent);
        Assert.Equal("evt_inv_123", invEvent.Id);
        Assert.Equal("inv_bill_456", invEvent.Data.InvoiceId);
        Assert.Equal("inv_bill_456", invEvent.Data.Id);
        Assert.Equal("open", invEvent.Data.Status);
        Assert.Equal(29.00m, (decimal)invEvent.Data.Total!.Value);
        Assert.Equal("sub_premium_123", invEvent.Data.Subscription?.SubscriptionId);
        Assert.Equal("test@example.com", invEvent.Data.Customer?.Email);
    }

    [Fact]
    public void ConstructTypedEvent_Parses_ConnectEvents_With_Account_Field()
    {
        var payload = """
            {
                "id": "evt_conn_123",
                "type": "account.updated",
                "created_at": "2026-08-07T11:04:22Z",
                "organization_id": "acct_child_123",
                "account": "acct_child_123",
                "data": {
                    "account": "acct_child_123",
                    "outstanding": ["company.registration_number"]
                }
            }
            """;
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        var typedEvent = BachsWebhook.ConstructTypedEvent(payload, header, Secret);

        var acctEvent = Assert.IsType<AccountUpdatedEvent>(typedEvent);
        Assert.Equal("evt_conn_123", acctEvent.Id);
        Assert.Equal("acct_child_123", acctEvent.Account);
        Assert.Equal("acct_child_123", acctEvent.Data.Account);
        Assert.Single(acctEvent.Data.Outstanding);
        Assert.Equal("company.registration_number", acctEvent.Data.Outstanding[0]);
    }

    [Fact]
    public void ConstructTypedEvent_FallsBack_To_UnknownBachsEvent_For_UnrecognizedType()
    {
        var payload = """
            {
                "id": "evt_future_999",
                "type": "future_feature.completed",
                "created_at": "2026-10-10T10:00:00Z",
                "organization_id": "acct_org123",
                "data": {
                    "foo": "bar"
                }
            }
            """;
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        var typedEvent = BachsWebhook.ConstructTypedEvent(payload, header, Secret);

        var unknownEvent = Assert.IsType<UnknownBachsEvent>(typedEvent);
        Assert.Equal("evt_future_999", unknownEvent.Id);
        Assert.Equal("future_feature.completed", unknownEvent.Type);

        var rawDict = unknownEvent.GetData<Dictionary<string, string>>();
        Assert.NotNull(rawDict);
        Assert.Equal("bar", rawDict["foo"]);
    }

    [Fact]
    public void PatternMatching_SwitchExpression_Dispatches_Accurately()
    {
        var payload = """
            {
                "id": "evt_switch_1",
                "type": "collection.succeeded",
                "created_at": "2026-09-22T10:41:18Z",
                "data": { "charge_id": "ch_switch123", "amount": "99.00" }
            }
            """;
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        var typedEvent = BachsWebhook.ConstructTypedEvent(payload, header, Secret);

        // Pattern matching demo
        string result = typedEvent switch
        {
            CollectionSucceededEvent e => $"Paid: {e.Data.ChargeId} ({e.Data.Amount})",
            CheckoutCompletedEvent e => $"Checkout: {e.Data.CheckoutId}",
            PayoutPaidEvent e => $"Payout: {e.Data.WithdrawalId}",
            UnknownBachsEvent e => $"Unknown: {e.Type}",
            _ => "Other",
        };

        Assert.Equal("Paid: ch_switch123 (99.00)", result);
    }

    [Fact]
    public void Client_And_Validator_Support_TryValidateTypedEvent()
    {
        IWebhookEventValidator validator = new WebhookEventValidator(Secret);

        var payload = """
            {
                "id": "evt_try_typed",
                "type": "checkout.expired",
                "created_at": "2026-07-20T10:14:00Z",
                "data": { "checkout_id": "chk_expired_1", "status": "expired" }
            }
            """;
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var hmac = ComputeHmac(payload, timestamp, Secret);
        var header = $"t={timestamp},v1={hmac}";

        var success = validator.TryValidateTypedEvent(payload, header, out var typedEvent);

        Assert.True(success);
        var expEvent = Assert.IsType<CheckoutExpiredEvent>(typedEvent);
        Assert.Equal("chk_expired_1", expEvent.Data.CheckoutId);
    }
}
