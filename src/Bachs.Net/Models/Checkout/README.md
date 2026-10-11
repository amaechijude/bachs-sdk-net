# Bachs Checkout Models, Builders & Responses

This module provides strongly-typed models, builders, and differentiated responses for managing hosted checkout sessions via the Bachs API.

---

## Two Ways to Build a Checkout Request

According to the Bachs API specification, a checkout session can be constructed in one of two mutually exclusive ways:
1. **Catalog Product Cart**: Purchase one or more pre-defined catalog products with quantities, optional ad-hoc pricing overrides, and customer information.
2. **Raw Pricing**: Dynamic ad-hoc pricing with an amount and currency directly on the checkout, without referencing catalog products.

All request and response models are implemented as **`sealed record`** types with **`init` setters** for thread safety, immutability, and value semantics.

---

## 1. Product Cart Builder

Use `CreateCheckoutSessionRequestBuilder.WithProductCart(...)` or `ProductCartCheckoutRequestBuilder.Create()`:

```csharp
using Bachs.Net.Models.Checkout;

// Using the static entry point:
var request = CreateCheckoutSessionRequestBuilder
    .WithProductCart(new List<CheckoutCartItemRequest>
    {
        new()
        {
            ProductId = "prod_premium_plan",
            Quantity = 2,
            Amount = 49.00m // Optional pay-what-you-want amount
        }
    })
    .WithCustomer(new CheckoutCustomerRequest
    {
        Email = "buyer@example.com",
        Name = "Jane Doe"
    })
    .WithSuccessUrl("https://example.com/checkout/success")
    .WithCancelUrl("https://example.com/checkout/cancel")
    .WithBillingCurrency("USD")
    .WithPaymentMethodTypes("USD_CARD", "NGN_BANK_TRANSFER")
    .WithPlatformFee(5.00m)
    .WithTransferData("acct_dest_123", 44.00m)
    .WithExpiresInMinutes(60)
    .WithMetadata("order_id", "order_1002")
    .Build();

// Or fluent chaining from an empty cart:
var request = ProductCartCheckoutRequestBuilder
    .Create()
    .AddProduct("prod_item1", quantity: 1)
    .AddProduct("prod_item2", quantity: 3, amount: 15.00m)
    .WithNewCustomer("buyer@example.com", "Jane Doe")
    .WithSuccessUrl("https://example.com/checkout/success")
    .Build();
```

### Constraints enforced by `ProductCartCheckoutRequestBuilder`:
- Must have between 1 and 20 cart items (`minItems: 1`, `maxItems: 20`).
- Each cart item must specify a valid `ProductId` and `Quantity >= 1`.
- Root `Pricing` is guaranteed to be `null` (enforcing mutual exclusivity).

---

## 2. Raw Pricing Builder

Use `CreateCheckoutSessionRequestBuilder.WithPricing(...)` or `RawPricingCheckoutRequestBuilder.Create()`:

```csharp
using Bachs.Net.Models.Checkout;

// Using the static entry point:
var request = CreateCheckoutSessionRequestBuilder
    .WithPricing(new CheckoutPricingRequest
    {
        Currency = "USD",
        Amount = 49.99m,
        PriceType = "fixed"
    })
    .WithNewCustomer("buyer@example.com", "Jane Doe")
    .WithSuccessUrl("https://example.com/checkout/success")
    .WithCancelUrl("https://example.com/checkout/cancel")
    .WithCurrencyOption("NGN", 75000.00m)
    .WithCustomerCreationAlways()
    .Build();

// Or fluent convenience methods:
var request = CreateCheckoutSessionRequestBuilder
    .ForRawPricing("USD")
    .WithAmount(25.00m)
    .WithCustomer(new CheckoutCustomerRequest { Email = "buyer@example.com" })
    .WithSuccessUrl("https://example.com/checkout/success")
    .Build();
```

### Constraints enforced by `RawPricingCheckoutRequestBuilder`:
- `Pricing` must be supplied with a non-empty `Currency`.
- Root `ProductCart` is guaranteed to be `null` (enforcing mutual exclusivity).

---

## Differentiated Response Payloads

The Bachs API returns distinct response schemas when creating versus retrieving checkout sessions:

| Operation | Response Type | Key Properties |
|---|---|---|
| **Create** (`POST /v1/checkout-sessions`) | [`CreateCheckoutSessionResponse`](CreateCheckoutSessionResponse.cs) | `CheckoutId`, `CheckoutUrl`, `Status`, `ExpiresAt`, `CreatedAt`, `Reference`, `PlatformFee`, `DestinationAmount` |
| **Get** (`GET /v1/checkout-sessions/{id}`) | [`GetCheckoutSessionResponse`](GetCheckoutSessionResponse.cs) | `CheckoutId`, `Status`, `Recurring`, `PaymentStatus`, `SourceType`, `Amount`, `Currency`, `Charge`, `Customer`, `CustomerDetails`, `Products`, `SessionMode`, `CompletedAt`, `UpdatedAt` |

### `CreateCheckoutSessionResponse` (Creation)
Returned immediately upon session creation. Focuses on the checkout URL to redirect the buyer:

```csharp
CreateCheckoutSessionResponse response = await client.CheckoutSessions.CreateAsync(request);

Console.WriteLine($"Session ID: {response.CheckoutId}");
Console.WriteLine($"Payment URL: {response.CheckoutUrl}");
Console.WriteLine($"Status: {response.Status}"); // 'open'
```

### `GetCheckoutSessionResponse` (Retrieval)
Returned when fetching an existing session by ID. Provides comprehensive fulfillment and lifecycle details:

```csharp
GetCheckoutSessionResponse session = await client.CheckoutSessions.GetAsync("chk_1M2N3o4P5q6R7s8T");

Console.WriteLine($"Status: {session.Status}");
Console.WriteLine($"Payment Status: {session.PaymentStatus}"); // 'succeeded', 'requires_payment_method', etc.
Console.WriteLine($"Settled Amount: {session.Amount} {session.Currency}");

if (session.Charge != null)
{
    Console.WriteLine($"Payment ID: {session.Charge.PaymentId}");
    Console.WriteLine($"Payment Method: {session.Charge.PaymentMethod}");
}

if (session.Products != null)
{
    foreach (var product in session.Products)
    {
        Console.WriteLine($" - {product.ProductName} x {product.Quantity} = {product.LineTotal} {product.Currency}");
    }
}
```

---

## Immutability & Sealed Records

All models in this namespace are defined as:
```csharp
public sealed record ModelName
{
    public PropertyType PropertyName { get; init; }
}
```
Benefits:
- Value-based equality comparisons.
- Thread-safe immutability.
- Safe modification via C# `with` expressions.
