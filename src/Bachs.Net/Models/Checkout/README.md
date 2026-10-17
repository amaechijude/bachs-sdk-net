# Bachs Checkout Models, Builders & Responses

This module provides strongly-typed models, builders, and differentiated responses for managing hosted checkout sessions via the Bachs API.

---

## Directory Layout

```
Models/Checkout/
├── CreateCheckoutSessionRequest.cs          // Request, cart item, pricing, customer & transfer records
├── CheckoutSessionRequestBuilder.cs         // Shared fluent options (abstract base)
├── CreateCheckoutSessionRequestBuilder.cs   // Static entry point
├── ProductCartCheckoutRequestBuilder.cs     // Catalog product cart builder
├── RawPricingCheckoutRequestBuilder.cs      // Raw ad-hoc pricing builder
├── CreateCheckoutSessionResponse.cs         // Returned by POST /v1/checkout-sessions
└── GetCheckoutSessionResponse.cs            // Returned by GET  /v1/checkout-sessions/{id}
```

All request and response models are **`sealed record`** types with **`init` setters** for thread safety, immutability, and value semantics. Monetary fields use [`BachsPrice`](../Common/BachsPrice.cs) (implicitly convertible from/to `decimal`) and currency fields use the [`BachsCurrency`](../Common/BachsCurrency.cs) enum.

---

## Two Ways to Build a Checkout Request

According to the Bachs API specification, a checkout session can be constructed in one of two mutually exclusive ways:
1. **Catalog Product Cart**: Purchase one or more pre-defined catalog products with quantities, optional ad-hoc pricing overrides, and customer information.
2. **Raw Pricing**: Dynamic ad-hoc pricing with an amount and currency directly on the checkout, without referencing catalog products.

The static `CreateCheckoutSessionRequestBuilder` is the entry point. It always hands back one of the two concrete builders, so you cannot accidentally emit both `product_cart` and `pricing` on the same request:

| Entry point | Returns | `Build()` guarantees |
|---|---|---|
| `CreateCheckoutSessionRequestBuilder.WithProductCart(...)` | `ProductCartCheckoutRequestBuilder` | `Pricing == null`, 1–20 valid cart items |
| `CreateCheckoutSessionRequestBuilder.WithPricing(...)` | `RawPricingCheckoutRequestBuilder` | `ProductCart == null`, pricing configured |

---

## 1. Product Cart Builder

Use `CreateCheckoutSessionRequestBuilder.WithProductCart(...)` (an `IList<CheckoutCartItemRequest>` or `params CheckoutCartItemRequest[]` overload), or instantiate `new ProductCartCheckoutRequestBuilder()` directly to start from an empty cart:

```csharp
using Bachs.Net.Models.Checkout;
using Bachs.Net.Models.Common;

// Using the static entry point:
var request = CreateCheckoutSessionRequestBuilder
    .WithProductCart(
        new List<CheckoutCartItemRequest>
        {
            new()
            {
                ProductId = "prod_premium_plan",
                Quantity = 2,
                Amount = 49.00m, // Optional pay-what-you-want amount (BachsPrice)
            },
        }
    )
    .WithCustomer(
        new CheckoutCustomerRequest
        {
            Email = "buyer@example.com",
            Name = "Jane Doe",
        }
    )
    .WithSuccessUrl("https://example.com/checkout/success")
    .WithCancelUrl("https://example.com/checkout/cancel")
    .WithBillingCurrency(BachsCurrency.USD)
    .WithPaymentMethodTypes("USD_CARD", "NGN_BANK_TRANSFER")
    .WithReference("order_1002")
    .WithPlatformFee(5.00m)
    .WithTransferData("acct_dest_123", 44.00m)
    .WithCustomerCreationAlways()
    .WithExpiresInMinutes(60)
    .WithSavePaymentMethod(true)
    .WithMetadata("order_id", "order_1002")
    .Build();

// Or fluent chaining from an empty cart:
var request = new ProductCartCheckoutRequestBuilder()
    .AddProduct("prod_item1", quantity: 1)
    .AddProduct("prod_item2", quantity: 3, amount: 15.00m)
    .WithNewCustomer("buyer@example.com", "Jane Doe")
    .WithSuccessUrl("https://example.com/checkout/success")
    .Build();
```

Cart-specific methods on `ProductCartCheckoutRequestBuilder`:

| Method | Description |
|---|---|
| `WithProductCart(IList<...> / IEnumerable<...> / params ...[])` | Replaces the whole cart. |
| `AddProduct(CheckoutCartItemRequest item)` | Appends one cart item. |
| `AddProduct(productId, quantity = 1, amount = null, pricing = null)` | Builds and appends a cart item; `productId` is checked with `ArgumentException.ThrowIfNullOrWhiteSpace`. |

### Validation enforced by `ProductCartCheckoutRequestBuilder.Build()`
- At least 1 cart item and at most 20 (`minItems: 1`, `maxItems: 20`).
- Every cart item must specify a non-empty `ProductId`.
- Every cart item must have `Quantity >= 1`.
- Root `Pricing` is guaranteed to be `null` (enforcing mutual exclusivity).
- Violations throw [`BachsInvalidOperationException`](../../Exceptions/BachsInvalidOperationException.cs) before any HTTP call is made.

---

## 2. Raw Pricing Builder

Use `CreateCheckoutSessionRequestBuilder.WithPricing(...)` (a `CheckoutPricingRequest` overload, or a `BachsPrice amount` + optional `BachsCurrency currency` overload defaulting to `BachsCurrency.USD`), or instantiate `new RawPricingCheckoutRequestBuilder()` directly:

```csharp
using Bachs.Net.Models.Checkout;
using Bachs.Net.Models.Common;

// Using the static entry point with a full pricing record:
var request = CreateCheckoutSessionRequestBuilder
    .WithPricing(
        new CheckoutPricingRequest
        {
            Currency = BachsCurrency.USD, // Required member with a BachsCurrency.USD default
            Amount = 49.99m,
            PriceType = "fixed",
        }
    )
    .WithNewCustomer("buyer@example.com", "Jane Doe")
    .WithSuccessUrl("https://example.com/checkout/success")
    .WithCancelUrl("https://example.com/checkout/cancel")
    .WithCurrencyOption(BachsCurrency.NGN, 75000.00m)
    .WithCustomerCreationAlways()
    .Build();

// Or the amount + currency shorthand (currency defaults to BachsCurrency.USD):
var request = CreateCheckoutSessionRequestBuilder
    .WithPricing(25.00m)
    .WithCustomer(new CheckoutCustomerRequest { Email = "buyer@example.com" })
    .WithSuccessUrl("https://example.com/checkout/success")
    .Build();

// Or build the pricing progressively from an empty builder:
var request = new RawPricingCheckoutRequestBuilder()
    .WithPricing(100.00m, BachsCurrency.USD)
    .WithPriceType("custom")
    .WithPresetAmount(10.00m)
    .WithPriceBounds(minimumAmount: 5.00m, maximumAmount: 250.00m)
    .WithCurrencyOption(BachsCurrency.NGN, 75000.00m)
    .WithSuccessUrl("https://example.com/checkout/success")
    .Build();
```

Pricing-specific methods on `RawPricingCheckoutRequestBuilder`:

| Method | Description |
|---|---|
| `WithPricing(CheckoutPricingRequest)` / `WithPricing(BachsPrice, BachsCurrency)` | Seeds the pricing record. Must be called first. |
| `WithPriceType(string)` | `'fixed'`, `'custom'`, or `'free'`. |
| `WithPresetAmount(BachsPrice)` | Suggested starting amount for a custom price. |
| `WithPriceBounds(BachsPrice? minimum, BachsPrice? maximum)` | Lower/upper bounds for a custom price. |
| `WithCurrencyOption(BachsCurrency, BachsPrice)` | Adds or overwrites one currency-specific override. |
| `WithCurrencyOptions(IDictionary<string, BachsPrice>)` | Replaces all currency-specific overrides. |

> **Ordering**: `WithPriceType`, `WithPresetAmount`, `WithPriceBounds`, `WithCurrencyOption(s)` all mutate the seeded pricing record, so they throw `ArgumentNullException` when called before `WithPricing(...)`.

### Validation enforced by `RawPricingCheckoutRequestBuilder.Build()`
- `Pricing` must be configured; otherwise `Build()` throws [`BachsInvalidOperationException`](../../Exceptions/BachsInvalidOperationException.cs).
- Root `ProductCart` is guaranteed to be `null` (enforcing mutual exclusivity).

---

## Shared Options (`CheckoutSessionRequestBuilder<TBuilder>`)

Both builders inherit the same fluent options from `CheckoutSessionRequestBuilder<TBuilder>`:

| Method | Maps to |
|---|---|
| `WithCustomer(CheckoutCustomerRequest)` | `Customer` |
| `WithExistingCustomer(string customerId)` | `Customer` with `CustomerId` (starts with `cust_`) |
| `WithNewCustomer(email, name?, phoneNumber?)` | `Customer` with contact details |
| `WithSuccessUrl(string)` | `SuccessUrl` (Bachs appends `?checkout_id=<id>`) |
| `WithCancelUrl(string)` | `CancelUrl` |
| `WithReturnUrl(string)` | `ReturnUrl` (deprecated alias for `success_url`) |
| `WithBillingCurrency(BachsCurrency)` | `BillingCurrency` |
| `WithPaymentMethodTypes(IList<string> / params string[])` | `PaymentMethodTypes` (e.g. `USD_CARD`, `NGN_BANK_TRANSFER`, `MOMO_GHS`, `CRYPTO`) |
| `WithReference(string)` | `Reference` (max 128 characters) |
| `WithPlatformFee(BachsPrice)` | `PlatformFee` (mutually exclusive with `TransferData.Amount`) |
| `WithTransferData(CheckoutTransferDataRequest)` / `WithTransferData(destination, amount?)` | `TransferData` |
| `WithCustomerCreation(string)` / `WithCustomerCreationAlways()` / `WithCustomerCreationIfRequired()` | `CustomerCreation` (`always` or `if_required`) |
| `WithExpiresInMinutes(int)` | `ExpiresInMinutes` (1–1440, default 60) |
| `WithSavePaymentMethod(bool)` | `SavePaymentMethod` |
| `WithMetadata(IDictionary<string, string>)` / `WithMetadata(key, value)` | `Metadata` (up to 20 keys, max 10 KB) |
| `Build()` | Abstract; validates and produces the `CreateCheckoutSessionRequest` |

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
var response = await client.CheckoutSessions.CreateAsync(request);
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Checkout session creation failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var created = response.Data;
Console.WriteLine($"Session ID: {created.CheckoutId}");
Console.WriteLine($"Payment URL: {created.CheckoutUrl}");
Console.WriteLine($"Status: {created.Status}"); // 'open'
```

### `GetCheckoutSessionResponse` (Retrieval)
Returned when fetching an existing session by ID. Provides comprehensive fulfillment and lifecycle details:

```csharp
var response = await client.CheckoutSessions.GetAsync("chk_1M2N3o4P5q6R7s8T");
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Checkout session lookup failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var session = response.Data;
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
