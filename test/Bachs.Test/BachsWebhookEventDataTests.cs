using Bachs.Models.Common;
using Bachs.Serialization;
using Bachs.WebHooks;
using Bachs.WebHooks.EventData;

namespace Bachs.Test;

public sealed class BachsWebhookEventDataTests
{
    [Fact]
    public void CheckoutCompleted_Deserializes_Correctly()
    {
        var json = """
            {
              "id": "evt_7f6e5d4c3b2a1908",
              "type": "checkout.completed",
              "created_at": "2026-07-20T09:15:00.000000+00:00",
              "organization_id": "acct_7KpQ2mNv4XbR9dLc",
              "data": {
                "checkout_id": "chk_6R7s8T9u0V1w2X3y",
                "status": "completed",
                "mode": "payment",
                "payment_status": "paid",
                "amount": "19.00",
                "currency": "USD",
                "reference": "order_1042",
                "customer": {
                  "customer_id": "cust_1a2b3c4d5e6f",
                  "email": "jane@example.com",
                  "name": "Jane Doe",
                  "billing_address": {
                    "line1": "40 Yaba Road",
                    "city": "Lagos",
                    "country": "NG"
                  }
                },
                "customer_details": {
                  "email": "jane@example.com",
                  "name": "Jane Doe"
                },
                "charge": {
                  "id": "ch_1a2b3c4d5e6f",
                  "amount": "19.00",
                  "currency": "USD",
                  "settlement_currency": "USD",
                  "settlement_amount": "18.62",
                  "status": "succeeded"
                },
                "subscription": null,
                "metadata": {
                  "order_id": "ORD-12345"
                }
              }
            }
            """;

        var evt = BachsJsonSerializer.Deserialize<BachsWebhookEvent>(json);
        Assert.NotNull(evt);
        Assert.Equal("evt_7f6e5d4c3b2a1908", evt.Id);
        Assert.Equal("checkout.completed", evt.Type);

        var data = evt.GetData<CheckoutEventData>();
        Assert.NotNull(data);
        Assert.Equal("chk_6R7s8T9u0V1w2X3y", data.CheckoutId);
        Assert.Equal("chk_6R7s8T9u0V1w2X3y", data.Id);
        Assert.Equal("completed", data.Status);
        Assert.Equal("payment", data.Mode);
        Assert.Equal("paid", data.PaymentStatus);
        Assert.Equal(19.00m, (decimal)data.Amount!);
        Assert.Equal(BachsCurrency.USD, data.Currency);
        Assert.Equal("order_1042", data.Reference);
        Assert.NotNull(data.Customer);
        Assert.Equal("cust_1a2b3c4d5e6f", data.Customer.CustomerId);
        Assert.Equal("cust_1a2b3c4d5e6f", data.Customer.Id);
        Assert.NotNull(data.CustomerDetails);
        Assert.Equal("jane@example.com", data.CustomerDetails.Email);
        Assert.NotNull(data.Charge);
        Assert.Equal("ch_1a2b3c4d5e6f", data.Charge.Id);
        Assert.Equal(18.62m, (decimal)data.Charge.SettlementAmount!);
    }

    [Fact]
    public void CollectionSucceeded_Deserializes_Correctly()
    {
        var json = """
            {
              "id": "evt_3ab4e0d5d27445cf8a52ab3d8cb8f0b1",
              "type": "collection.succeeded",
              "created_at": "2026-09-22T10:41:18.402Z",
              "organization_id": "acct_7KpQ2mNv4XbR9dLc",
              "data": {
                "charge_id": "ch_1a2b3c4d5e6f",
                "checkout_id": "chk_6R7s8T9u0V1w2X3y",
                "reference": "ORD-20260922-1041",
                "status": "SUCCEEDED",
                "amount": "75000.00",
                "currency": "NGN",
                "settlement_amount": "73875.00",
                "settlement_currency": "NGN",
                "processing_fee": "1125.00",
                "processing_fee_currency": "NGN",
                "fee_bearer": "merchant",
                "product_cart": [
                  { "product_id": "prod_1a2b3c", "quantity": 2 },
                  { "product_id": "prod_9x8y7z", "quantity": 1, "amount": "5000.00" }
                ],
                "customer": {
                  "id": "cust_xyz789",
                  "email": "jane@example.com",
                  "name": "Jane Doe"
                },
                "payment_method_details": {
                  "type": "bank_transfer",
                  "bank_transfer": {
                    "sender_name": "JANE ADEYEMI",
                    "sender_bank": "Guaranty Trust Bank",
                    "virtual_account": {
                      "account_number": "9902847361",
                      "bank_name": "Example Bank"
                    }
                  }
                }
              }
            }
            """;

        var evt = BachsJsonSerializer.Deserialize<BachsWebhookEvent>(json);
        Assert.NotNull(evt);

        var data = evt.GetData<CollectionEventData>();
        Assert.NotNull(data);
        Assert.Equal("ch_1a2b3c4d5e6f", data.ChargeId);
        Assert.Equal("ch_1a2b3c4d5e6f", data.Id);
        Assert.Equal("SUCCEEDED", data.Status);
        Assert.Equal(75000.00m, (decimal)data.Amount!);
        Assert.Equal(BachsCurrency.NGN, data.Currency);
        Assert.NotNull(data.ProductCart);
        Assert.Equal(2, data.ProductCart.Count);
        Assert.Equal("prod_1a2b3c", data.ProductCart[0].ProductId);
        Assert.NotNull(data.PaymentMethodDetails?.BankTransfer?.VirtualAccount);
        Assert.Equal(
            "9902847361",
            data.PaymentMethodDetails.BankTransfer.VirtualAccount.AccountNumber
        );
    }

    [Fact]
    public void DisputeCreated_Deserializes_Correctly()
    {
        var json = """
            {
              "id": "evt_1a2b3c4d5e6f7g8h",
              "type": "dispute.created",
              "created_at": "2026-04-27T12:00:00.000000+00:00",
              "organization_id": "acct_7KpQ2mNv4XbR9dLc",
              "account": "acct_7KpQ2mNv4XbR9dLc",
              "data": {
                "dispute_id": "dsp_1a2b3c4d5e",
                "charge_id": "ch_9f4c1d2e7b6a4f8e9c0d1a2b3c4d5e6f",
                "amount": "75000.00",
                "currency": "NGN",
                "status": "needs_response",
                "is_response_editable": true,
                "reason": "fraudulent",
                "charge_amount": "75000.00",
                "charge_currency": "NGN"
              }
            }
            """;

        var evt = BachsJsonSerializer.Deserialize<BachsWebhookEvent>(json);
        Assert.NotNull(evt);

        var data = evt.GetData<DisputeEventData>();
        Assert.NotNull(data);
        Assert.Equal("dsp_1a2b3c4d5e", data.DisputeId);
        Assert.Equal("dsp_1a2b3c4d5e", data.Id);
        Assert.Equal("ch_9f4c1d2e7b6a4f8e9c0d1a2b3c4d5e6f", data.ChargeId);
        Assert.True(data.IsResponseEditable);
        Assert.Equal("fraudulent", data.Reason);
    }
}
