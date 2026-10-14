# Hosted Checkout Sessions Client

The Checkout Sessions client enables you to create and inspect hosted checkout pages on Bachs. Customers are redirected to the secure hosted URL to complete payments via cards, bank transfers, mobile money, and alternative payment methods.

- **Interface**: [`ICheckoutSessionsClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/ICheckoutSessionsClient.cs)
- **Implementation**: [`CheckoutSessionsClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/CheckoutSessionsClient.cs)
- **Property**: `client.CheckoutSessions`
- **DI Service**: `services.AddBachsClient(...)` registers `ICheckoutSessionsClient` as a transient service.

---

## Methods Overview

| Method | HTTP | Path | Description |
|---|---|---|---|
| [`CreateAsync`](#1-create-a-checkout-session) | `POST` | `/v1/checkout-sessions` | Creates a new hosted checkout session and returns `CreateCheckoutSessionResponse`. |
| [`GetAsync`](#2-retrieve-a-checkout-session) | `GET` | `/v1/checkout-sessions/{checkoutId}` | Retrieves full session details, payment status, and order summary as `GetCheckoutSessionResponse`. |

---

## Usage Examples

### 1. Create a Checkout Session

There are two main ways to construct a checkout request:
1. **Catalog Product Cart**: Purchase items from your product catalog.
2. **Raw Ad-Hoc Pricing**: Dynamic amount and currency without catalog products.

All models are **sealed records with init setters**. Use [`CreateCheckoutSessionRequestBuilder`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Checkout/RequestBuilders/CreateCheckoutSessionRequestBuilder.cs) to ensure compile-time mutual exclusivity: its `WithProductCart(...)` entry point returns a `ProductCartCheckoutRequestBuilder`, and its `WithPricing(...)` entry point returns a `RawPricingCheckoutRequestBuilder`. Both inherit the shared fluent options (customer, URLs, billing currency, payment methods, reference, platform fee, transfer data, customer creation, expiry, metadata) from `CheckoutSessionRequestBuilder<TBuilder>`.

Both builders validate on `Build()` and throw `BachsInvalidOperationException` before any HTTP call when a constraint is violated (cart must hold 1–20 items with a non-empty `ProductId` and `Quantity >= 1`; raw pricing must be seeded via `WithPricing(...)` first).

#### Example A: Catalog Product Cart Builder

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Common;
using Bachs.Net.Models.Checkout;
using Bachs.Net.Models.Checkout.Request;

var request = CreateCheckoutSessionRequestBuilder
    .WithProductCart(new List<CheckoutCartItemRequest>
    {
        new() { ProductId = "prod_premium_plan", Quantity = 1 },
        new() { ProductId = "prod_add_on_storage", Quantity = 2 }
    })
    .WithCustomer(new CheckoutCustomerRequest
    {
        Email = "customer@example.com",
        Name = "Acme Corp"
    })
    .WithSuccessUrl("https://example.com/billing/confirmed")
    .WithCancelUrl("https://example.com/billing/cancel")
    .Build();

CreateCheckoutSessionResponse session = await client.CheckoutSessions.CreateAsync(request);

Console.WriteLine($"Checkout ID: {session.CheckoutId}");
Console.WriteLine($"Redirect Customer to: {session.CheckoutUrl}");
```

#### Example B: Raw Pricing Builder

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Common;
using Bachs.Net.Models.Checkout;
using Bachs.Net.Models.Checkout.Request;
using Bachs.Net.Models.Common;

var request = CreateCheckoutSessionRequestBuilder
    .WithPricing(new CheckoutPricingRequest
    {
        Currency = BachsCurrency.USD, // Required member, defaults to BachsCurrency.USD
        Amount = 49.99m               // BachsPrice; implicitly convertible from decimal
    })
    .WithCustomer(new CheckoutCustomerRequest
    {
        Email = "jane.doe@example.com",
        Name = "Jane Doe"
    })
    .WithSuccessUrl("https://example.com/checkout/success")
    .WithCancelUrl("https://example.com/checkout/cart")
    .WithMetadata("order_id", "order_100234")
    .Build();

// Or the shorthand overload: CreateCheckoutSessionRequestBuilder.WithPricing(49.99m)
// Currency defaults to BachsCurrency.USD.

CreateCheckoutSessionResponse session = await client.CheckoutSessions.CreateAsync(
    request,
    new RequestOptions { IdempotencyKey = "checkout_order_100234" }
);

Console.WriteLine($"Checkout ID: {session.CheckoutId}");
Console.WriteLine($"Hosted Checkout URL: {session.CheckoutUrl}");
```

---

### 2. Retrieve a Checkout Session

Retrieve detailed session status, charge information, and resolved line items:

```csharp
GetCheckoutSessionResponse session = await client.CheckoutSessions.GetAsync("chk_1M2N3o4P5q6R7s8T");

Console.WriteLine($"Status: {session.Status}");          // 'open', 'completed', 'expired', 'cancelled'
Console.WriteLine($"Payment Status: {session.PaymentStatus}"); // 'succeeded', 'requires_payment_method', etc.
Console.WriteLine($"Total Amount: {session.Amount} {session.Currency}");

if (session.Charge != null)
{
    Console.WriteLine($"Payment ID: {session.Charge.PaymentId}");
    Console.WriteLine($"Payment Method: {session.Charge.PaymentMethod}");
}

if (session.Products != null)
{
    foreach (var item in session.Products)
    {
        Console.WriteLine($" - {item.ProductName} x {item.Quantity} = {item.LineTotal} {item.Currency}");
    }
}
```

---

## Request & Response Models

### [`CreateCheckoutSessionRequest`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Checkout/Request/CreateCheckoutSessionRequest.cs)
- `ProductCart` (`IList<CheckoutCartItemRequest>?`): Catalog items purchased (mutually exclusive with `Pricing`).
- `Pricing` (`CheckoutPricingRequest?`): Direct ad-hoc pricing (mutually exclusive with `ProductCart`).
- `Customer` (`CheckoutCustomerRequest?`): Customer ID or contact details.
- `SuccessUrl` (`string?`): Absolute redirect URL on success.
- `CancelUrl` (`string?`): Absolute redirect URL on cancellation.
- `ReturnUrl` (`string?`): Deprecated alias for `SuccessUrl`.
- `BillingCurrency` (`BachsCurrency?`): Checkout billing currency override.
- `PaymentMethodTypes` (`IList<string>?`): Corridor restrictions (e.g. `USD_CARD`, `NGN_BANK_TRANSFER`).
- `Reference` (`string?`): External order reference (max 128 characters).
- `PlatformFee` (`BachsPrice?`): Platform fee cut from merchant proceeds.
- `TransferData` (`CheckoutTransferDataRequest?`): Destination account payout split.
- `CustomerCreation` (`string?`): 'always' or 'if_required'.
- `ExpiresInMinutes` (`int?`): Lifetime in minutes (1 to 1440, default 60).
- `SavePaymentMethod` (`bool?`): Persist the buyer's card for off-session charges.
- `Metadata` (`IDictionary<string, string>?`): Up to 20 key-value pairs (max 10 KB).

Supporting request records live in the same file: `CheckoutCartItemRequest` (`ProductId` is `required`, `Quantity` defaults to 1), `CheckoutItemPricingOverrideRequest`, `CheckoutPricingRequest` (`Currency` is `required` and defaults to `BachsCurrency.USD`), `CheckoutTransferDataRequest` (`Destination` is `required`), and `CheckoutCustomerRequest`.

### [`CreateCheckoutSessionResponse`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Checkout/Responses/CreateCheckoutSessionResponse.cs)
- `CheckoutId` (`string`): Unique session ID starting with `chk_`.
- `CheckoutUrl` (`string`): Hosted Bachs checkout page URL.
- `Status` (`string`): `open`, `completed`, `expired`, or `cancelled`.
- `ExpiresAt` (`DateTimeOffset?`): Expiry timestamp.
- `CreatedAt` (`DateTimeOffset`): Creation timestamp.
- `Reference` (`string?`): Echoed reference.
- `PlatformFee` (`BachsPrice?`): Echoed platform cut.
- `DestinationAmount` (`BachsPrice?`): Echoed destination account share.

### [`GetCheckoutSessionResponse`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Checkout/Responses/GetCheckoutSessionResponse.cs)
- `CheckoutId` (`string`): Unique session ID starting with `chk_`.
- `Status` (`string`): Lifecycle status.
- `Recurring` (`CheckoutRecurring?`): Subscription recurrence cadence.
- `PaymentStatus` (`string?`): Payment state (`succeeded`, `requires_payment_method`, etc.).
- `SourceType` (`string?`): Creation origin (`CHECKOUT_SESSION`, `API`).
- `Amount` (`BachsPrice?`): Total amount charged.
- `Currency` (`BachsCurrency?`): Currency of the session.
- `BillingCurrency` (`BachsCurrency?`): Presentment currency for checkout.
- `Charge` (`CheckoutPaymentResponse?`): Attached charge details once payment is attempted.
- `Customer` (`CheckoutCustomer?`): Attached customer profile.
- `CustomerDetails` (`CheckoutCustomerDetails?`): Buyer details collected during checkout.
- `Products` (`IReadOnlyList<ResolvedProductItem>?`): Resolved product items with unit amounts and totals.
- `SessionMode` (`string?`): Presentation mode (`CART` or `SELECTION`).
- `CreatedAt`, `ExpiresAt`, `CompletedAt`, `UpdatedAt` (`DateTimeOffset`): Lifecycle timestamps.

---

## Request Builders

| Type | Location | Purpose |
|---|---|---|
| `CreateCheckoutSessionRequestBuilder` | [`RequestBuilders/CreateCheckoutSessionRequestBuilder.cs`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Checkout/RequestBuilders/CreateCheckoutSessionRequestBuilder.cs) | Static entry point: `WithProductCart(...)` and `WithPricing(...)` overloads. |
| `CheckoutSessionRequestBuilder<TBuilder>` | [`RequestBuilders/CheckoutSessionRequestBuilder.cs`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Checkout/RequestBuilders/CheckoutSessionRequestBuilder.cs) | Abstract base holding the shared fluent options and `Build()` contract. |
| `ProductCartCheckoutRequestBuilder` | [`RequestBuilders/ProductCartCheckoutRequestBuilder.cs`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Checkout/RequestBuilders/ProductCartCheckoutRequestBuilder.cs) | Cart seeding (`WithProductCart`, `AddProduct`) plus cart validation. |
| `RawPricingCheckoutRequestBuilder` | [`RequestBuilders/RawPricingCheckoutRequestBuilder.cs`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Checkout/RequestBuilders/RawPricingCheckoutRequestBuilder.cs) | Pricing seeding (`WithPricing`) and tuning (`WithPriceType`, `WithPresetAmount`, `WithPriceBounds`, `WithCurrencyOption(s)`). |

See the module [README](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Checkout/README.md) for the full builder reference.

---

## Associated Webhooks

- `checkout.completed`: Fired when a customer completes payment through the hosted checkout page.
- `checkout.expired`: Fired when an open checkout session expires without completion.
