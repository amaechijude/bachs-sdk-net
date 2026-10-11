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

All models are **sealed records with init setters**. Use [`CreateCheckoutSessionRequestBuilder`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Checkout/CreateCheckoutSessionRequestBuilder.cs) to ensure compile-time mutual exclusivity.

#### Example A: Catalog Product Cart Builder

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Common;
using Bachs.Net.Models.Checkout;

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

var request = CreateCheckoutSessionRequestBuilder
    .WithPricing(new CheckoutPricingRequest
    {
        Currency = "USD",
        Amount = 49.99m // Transparent decimal Price handling
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

### [`CreateCheckoutSessionRequest`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Checkout/CreateCheckoutSessionRequest.cs)
- `ProductCart` (`IList<CheckoutCartItemRequest>?`): Catalog items purchased (mutually exclusive with `Pricing`).
- `Pricing` (`CheckoutPricingRequest?`): Direct ad-hoc pricing (mutually exclusive with `ProductCart`).
- `Customer` (`CheckoutCustomerRequest?`): Customer ID or contact details.
- `SuccessUrl` (`string?`): Absolute redirect URL on success.
- `CancelUrl` (`string?`): Absolute redirect URL on cancellation.
- `BillingCurrency` (`string?`): Checkout billing currency override.
- `PaymentMethodTypes` (`IList<string>?`): Corridor restrictions (e.g. `USD_CARD`, `NGN_BANK_TRANSFER`).
- `Reference` (`string?`): External order reference.
- `PlatformFee` (`Price?`): Platform fee cut from merchant proceeds.
- `TransferData` (`CheckoutTransferDataRequest?`): Destination account payout split.
- `CustomerCreation` (`string?`): 'always' or 'if_required'.
- `ExpiresInMinutes` (`int?`): Lifetime in minutes (1 to 1440).
- `Metadata` (`IDictionary<string, string>?`): Up to 20 key-value pairs.

### [`CreateCheckoutSessionResponse`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Checkout/CreateCheckoutSessionResponse.cs)
- `CheckoutId` (`string`): Unique session ID starting with `chk_`.
- `CheckoutUrl` (`string`): Hosted Bachs checkout page URL.
- `Status` (`string`): `open`, `completed`, `expired`, or `cancelled`.
- `ExpiresAt` (`DateTimeOffset?`): Expiry timestamp.
- `CreatedAt` (`DateTimeOffset`): Creation timestamp.
- `Reference` (`string?`): Echoed reference.
- `PlatformFee` (`Price?`): Echoed platform cut.
- `DestinationAmount` (`Price?`): Echoed destination account share.

### [`GetCheckoutSessionResponse`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Checkout/GetCheckoutSessionResponse.cs)
- `CheckoutId` (`string`): Unique session ID starting with `chk_`.
- `Status` (`string`): Lifecycle status.
- `Recurring` (`CheckoutRecurring?`): Subscription recurrence cadence.
- `PaymentStatus` (`string?`): Payment state (`succeeded`, `requires_payment_method`, etc.).
- `SourceType` (`string?`): Creation origin (`CHECKOUT_SESSION`, `API`).
- `Amount` (`Price?`): Total amount charged.
- `Currency` (`string?`): Currency of the session.
- `Charge` (`CheckoutPaymentResponse?`): Attached charge details once payment is attempted.
- `Customer` (`CheckoutCustomer?`): Attached customer profile.
- `CustomerDetails` (`CheckoutCustomerDetails?`): Buyer details collected during checkout.
- `Products` (`IReadOnlyList<ResolvedProductItem>?`): Resolved product items with unit amounts and totals.
- `SessionMode` (`string?`): Presentation mode (`CART` or `SELECTION`).
- `CreatedAt`, `ExpiresAt`, `CompletedAt`, `UpdatedAt` (`DateTimeOffset`): Lifecycle timestamps.

---

## Associated Webhooks

- `checkout.completed`: Fired when a customer completes payment through the hosted checkout page.
- `checkout.expired`: Fired when an open checkout session expires without completion.
