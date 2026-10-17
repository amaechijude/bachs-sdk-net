using System.Net;
using System.Text;
using Bachs.Net.Clients;
using Bachs.Net.Common;
using Bachs.Net.Models.Checkout;
using Bachs.Net.Models.Common;
using Bachs.Net.Serialization;

namespace Bachs.Test;

public sealed class CheckoutSessionTests
{
    private sealed class MockHttpMessageHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        public string? LastRequestBody { get; private set; }
        public HttpResponseMessage ResponseToReturn { get; set; } = new(HttpStatusCode.OK);

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken
        )
        {
            LastRequest = request;
            if (request.Content != null)
            {
                LastRequestBody = await request.Content.ReadAsStringAsync(cancellationToken);
            }

            return ResponseToReturn;
        }
    }

    [Fact]
    public void ProductCartBuilder_BuildsValidRequest()
    {
        var request = CreateCheckoutSessionRequestBuilder
            .WithProductCart(
                new List<CheckoutCartItemRequest>
                {
                    new()
                    {
                        ProductId = "prod_12345",
                        Quantity = 2,
                        Amount = 29.00m,
                        Pricing = new CheckoutItemPricingOverrideRequest
                        {
                            PriceType = "fixed",
                            Amount = 29.00m,
                        },
                    },
                }
            )
            .WithCustomer(
                new CheckoutCustomerRequest
                {
                    Email = "buyer@example.com",
                    Name = "Jane Doe",
                    PhoneNumber = "+1234567890",
                }
            )
            .WithSuccessUrl("https://example.com/checkout/success")
            .WithCancelUrl("https://example.com/checkout/cancel")
            .WithBillingCurrency(BachsCurrency.USD)
            .WithPaymentMethodTypes("USD_CARD", "NGN_BANK_TRANSFER")
            .WithReference("order_9876")
            .WithPlatformFee(5.00m)
            .WithTransferData("acct_dest_123", 24.00m)
            .WithCustomerCreationAlways()
            .WithExpiresInMinutes(45)
            .WithSavePaymentMethod(true)
            .WithMetadata("order_id", "12345")
            .Build();

        Assert.NotNull(request.ProductCart);
        Assert.Single(request.ProductCart);
        Assert.Equal("prod_12345", request.ProductCart[0].ProductId);
        Assert.Equal(2, request.ProductCart[0].Quantity);
        Assert.Null(request.Pricing);
        Assert.NotNull(request.Customer);
        Assert.Equal("buyer@example.com", request.Customer.Email);
        Assert.Equal("Jane Doe", request.Customer.Name);
        Assert.Equal("https://example.com/checkout/success", request.SuccessUrl);
        Assert.Equal("https://example.com/checkout/cancel", request.CancelUrl);
        Assert.Equal(BachsCurrency.USD, request.BillingCurrency);
        Assert.Equal(2, request.PaymentMethodTypes?.Count);
        Assert.Equal("order_9876", request.Reference);
        Assert.Equal(5.00m, request.PlatformFee?.Value);
        Assert.NotNull(request.TransferData);
        Assert.Equal("acct_dest_123", request.TransferData.Destination);
        Assert.Equal(24.00m, request.TransferData.Amount?.Value);
        Assert.Equal("always", request.CustomerCreation);
        Assert.Equal(45, request.ExpiresInMinutes);
        Assert.True(request.SavePaymentMethod);
        Assert.Equal("12345", request.Metadata?["order_id"]);
    }

    [Fact]
    public void RawPricingBuilder_BuildsValidRequest()
    {
        var request = CreateCheckoutSessionRequestBuilder
            .WithPricing(49.99m, BachsCurrency.USD)
            .WithPriceType("fixed")
            .WithCustomer(
                new CheckoutCustomerRequest { Email = "buyer@example.com", Name = "Jane Doe" }
            )
            .WithSuccessUrl("https://example.com/checkout/success")
            .WithCancelUrl("https://example.com/checkout/cancel")
            .WithCurrencyOption(BachsCurrency.NGN, 75000.00m)
            .WithCustomerCreationIfRequired()
            .Build();

        Assert.NotNull(request.Pricing);
        Assert.Equal(BachsCurrency.USD, request.Pricing.Currency);
        Assert.Equal(49.99m, request.Pricing.Amount?.Value);
        Assert.Equal("fixed", request.Pricing.PriceType);
        Assert.NotNull(request.Pricing.CurrencyOptions);
        Assert.Equal(75000.00m, request.Pricing.CurrencyOptions["NGN"].Value);
        Assert.Null(request.ProductCart);
        Assert.Equal("if_required", request.CustomerCreation);
    }

    [Fact]
    public void Deserializes_CreateCheckoutSessionResponse()
    {
        var json = """
            {
                "checkout_id": "chk_1M2N3o4P5q6R7s8T",
                "checkout_url": "https://checkout.bachs.io/c/Tb5rHn8YkQ2vXpL",
                "status": "open",
                "expires_at": "2026-01-24T15:30:00.000Z",
                "created_at": "2026-01-24T14:30:00.000Z",
                "reference": "order_9876",
                "platform_fee": "20.00",
                "destination_amount": "80.00"
            }
            """;

        var response = BachsJsonSerializer.Deserialize<CreateCheckoutSessionResponse>(json);

        Assert.NotNull(response);
        Assert.Equal("chk_1M2N3o4P5q6R7s8T", response.CheckoutId);
        Assert.Equal("chk_1M2N3o4P5q6R7s8T", response.Id);
        Assert.Equal("https://checkout.bachs.io/c/Tb5rHn8YkQ2vXpL", response.CheckoutUrl);
        Assert.Equal("open", response.Status);
        Assert.Equal("order_9876", response.Reference);
        Assert.Equal(20.00m, response.PlatformFee?.Value);
        Assert.Equal(80.00m, response.DestinationAmount?.Value);
        Assert.NotNull(response.ExpiresAt);
        Assert.Equal(2026, response.CreatedAt.Year);
    }

    [Fact]
    public void Deserializes_GetCheckoutSessionResponse()
    {
        var json = """
            {
                "checkout_id": "chk_1M2N3o4P5q6R7s8T",
                "status": "completed",
                "recurring": {
                    "interval": "month",
                    "interval_count": 1
                },
                "payment_status": "succeeded",
                "source_type": "CHECKOUT_SESSION",
                "amount": "50.00",
                "currency": "USD",
                "reference": "order_9876",
                "charge": {
                    "payment_id": "pay_1a2b3c4d5e",
                    "billing_reason": "purchase",
                    "status": "succeeded",
                    "amount": "29.00",
                    "currency": "USD",
                    "fee_usd": "0.59"
                },
                "payment_method": "CARD",
                "customer": {
                    "id": "cust_1a2b3c4d5e6f",
                    "email": "jane@example.com",
                    "name": "Jane Doe"
                },
                "customer_details": {
                    "email": "jane@example.com",
                    "name": "Jane Doe"
                },
                "success_url": "https://yourapp.com/success",
                "cancel_url": "https://yourapp.com/cancel",
                "products": [
                    {
                        "product_id": "prod_abc123",
                        "product_name": "Premium Plan",
                        "quantity": 1,
                        "unit_amount": "50.00",
                        "currency": "USD",
                        "price_type": "fixed",
                        "line_total": "50.00"
                    }
                ],
                "billing_currency": "NGN",
                "session_mode": "CART",
                "metadata": {
                    "order_id": "ORD-9876"
                },
                "created_at": "2026-01-24T14:30:00.000Z",
                "expires_at": "2026-01-24T15:30:00.000Z",
                "completed_at": "2026-01-24T14:35:00.000Z",
                "updated_at": "2026-01-24T14:35:00.000Z"
            }
            """;

        var response = BachsJsonSerializer.Deserialize<GetCheckoutSessionResponse>(json);

        Assert.NotNull(response);
        Assert.Equal("chk_1M2N3o4P5q6R7s8T", response.CheckoutId);
        Assert.Equal("chk_1M2N3o4P5q6R7s8T", response.Id);
        Assert.Equal("completed", response.Status);
        Assert.NotNull(response.Recurring);
        Assert.Equal("month", response.Recurring.Interval);
        Assert.Equal(1, response.Recurring.IntervalCount);
        Assert.Equal("succeeded", response.PaymentStatus);
        Assert.Equal("CHECKOUT_SESSION", response.SourceType);
        Assert.Equal(50.00m, response.Amount?.Value);
        Assert.Equal(BachsCurrency.USD, response.Currency);
        Assert.Equal("order_9876", response.Reference);
        Assert.NotNull(response.Charge);
        Assert.Equal("pay_1a2b3c4d5e", response.Charge.PaymentId);
        Assert.Equal("pay_1a2b3c4d5e", response.Charge.ChargeId);
        Assert.Equal("purchase", response.Charge.BillingReason);
        Assert.Equal(29.00m, response.Charge.Amount?.Value);
        Assert.Equal("CARD", response.PaymentMethod);
        Assert.NotNull(response.Customer);
        Assert.Equal("cust_1a2b3c4d5e6f", response.Customer.Id);
        Assert.NotNull(response.CustomerDetails);
        Assert.Equal("jane@example.com", response.CustomerDetails.Email);
        Assert.Equal("https://yourapp.com/success", response.SuccessUrl);
        Assert.Equal("https://yourapp.com/cancel", response.CancelUrl);
        Assert.NotNull(response.Products);
        Assert.Single(response.Products);
        Assert.Equal("prod_abc123", response.Products[0].ProductId);
        Assert.Equal("Premium Plan", response.Products[0].ProductName);
        Assert.Equal(50.00m, response.Products[0].UnitAmount.Value);
        Assert.Equal(BachsCurrency.NGN, response.BillingCurrency);
        Assert.Equal("CART", response.SessionMode);
        Assert.Equal("ORD-9876", response.Metadata?["order_id"]);
        Assert.NotNull(response.CompletedAt);
    }

    [Fact]
    public async Task CheckoutSessionsClient_CreateAsync_And_GetAsync()
    {
        var createJson = """
            {
                "checkout_id": "chk_created_1",
                "checkout_url": "https://checkout.bachs.io/c/test",
                "status": "open",
                "created_at": "2026-04-27T12:00:00Z"
            }
            """;

        var getJson = """
            {
                "checkout_id": "chk_created_1",
                "status": "completed",
                "amount": "100.00",
                "currency": "USD",
                "created_at": "2026-04-27T12:00:00Z",
                "updated_at": "2026-04-27T12:05:00Z"
            }
            """;

        var handler = new MockHttpMessageHandler
        {
            ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(createJson, Encoding.UTF8, "application/json"),
            },
        };

        var httpClient = new HttpClient(handler);
        var client = new BachsClient(
            httpClient,
            new BachsClientOptions { ApiKey = "sk_sandbox_test123" }
        );

        var createReq = CreateCheckoutSessionRequestBuilder
            .WithPricing(100.00m, BachsCurrency.USD)
            .WithSuccessUrl("https://example.com/success")
            .Build();

        var createRes = await client.CheckoutSessions.CreateAsync(createReq);
        Assert.NotNull(createRes.Data);
        Assert.Equal("chk_created_1", createRes.Data.CheckoutId);
        Assert.Equal("https://checkout.bachs.io/c/test", createRes.Data.CheckoutUrl);

        handler.ResponseToReturn = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(getJson, Encoding.UTF8, "application/json"),
        };

        var getRes = await client.CheckoutSessions.GetAsync("chk_created_1");
        Assert.NotNull(getRes.Data);
        Assert.Equal("chk_created_1", getRes.Data.CheckoutId);
        Assert.Equal("completed", getRes.Data.Status);
        Assert.Equal(100.00m, getRes.Data.Amount?.Value);
    }
}
