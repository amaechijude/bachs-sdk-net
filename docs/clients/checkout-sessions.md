# Hosted Checkout Sessions Client

The Checkout Sessions client enables you to create and inspect hosted checkout pages on Bachs. Customers are redirected to the secure hosted URL to complete payments via cards, bank transfers, mobile money, and alternative payment methods.

- **Interface**: [`ICheckoutSessionsClient`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Clients/ICheckoutSessionsClient.cs)
- **Implementation**: [`CheckoutSessionsClient`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Clients/CheckoutSessionsClient.cs)
- **Property**: `client.CheckoutSessions`
- **DI Service**: `services.AddBachsClient(...)` registers `ICheckoutSessionsClient` as a transient service.

---

## Methods Overview

| Method | HTTP | Path | Description |
|---|---|---|---|
| [`CreateAsync`](#1-create-a-checkout-session) | `POST` | `/v1/checkout` | Creates a new hosted checkout session and returns the checkout URL. |
| [`GetAsync`](#2-retrieve-a-checkout-session) | `GET` | `/v1/checkout/{checkoutId}` | Retrieves current session details, payment status, and order summary. |

---

## Usage Examples

### 1. Create a Checkout Session

You can configure checkout sessions using **custom line pricing** or by referencing items from your **product catalog**.

#### Example A: Ad-Hoc / Custom Pricing

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Common;
using Bachs.Net.Models.Checkout;

var session = await client.CheckoutSessions.CreateAsync(new CreateCheckoutSessionRequest
{
    Pricing = new CheckoutPricingRequest
    {
        Currency = "USD",
        Amount = 49.99m // Transparent decimal handling
    },
    Customer = new CheckoutCustomerRequest
    {
        Email = "jane.doe@example.com",
        Name = "Jane Doe"
    },
    SuccessUrl = "https://example.com/checkout/success?session_id={CHECKOUT_SESSION_ID}",
    CancelUrl = "https://example.com/checkout/cart",
    Metadata = new Dictionary<string, string>
    {
        ["order_id"] = "order_100234",
        ["tenant_id"] = "tenant_abc"
    }
}, new RequestOptions
{
    IdempotencyKey = "checkout_order_100234"
});

Console.WriteLine($"Session ID: {session.Id}");
Console.WriteLine($"Hosted Checkout URL: {session.CheckoutUrl}");
```

#### Example B: Product Catalog Items

```csharp
var session = await client.CheckoutSessions.CreateAsync(new CreateCheckoutSessionRequest
{
    ProductCart = new List<CheckoutCartItemRequest>
    {
        new() { ProductId = "prod_premium_plan", Quantity = 1 },
        new() { ProductId = "prod_add_on_storage", Quantity = 2 }
    },
    Customer = new CheckoutCustomerRequest
    {
        Email = "customer@example.com",
        Name = "Acme Corp"
    },
    SuccessUrl = "https://example.com/billing/confirmed",
    CancelUrl = "https://example.com/billing/cancel"
});
```

---

### 2. Retrieve a Checkout Session

Fetch the status of a session to verify if payment was completed or inspect the payment record ID:

```csharp
var session = await client.CheckoutSessions.GetAsync("chk_987654321");

Console.WriteLine($"Status: {session.Status}");          // 'open', 'completed', 'expired'
Console.WriteLine($"Payment Status: {session.PaymentStatus}"); // 'unpaid', 'paid'
Console.WriteLine($"Payment ID: {session.PaymentId}");   // 'ch_...' if paid
Console.WriteLine($"Total Amount: {session.Pricing?.Amount} {session.Pricing?.Currency}");
```

---

## Request & Response Models

### [`CreateCheckoutSessionRequest`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Models/Checkout/CreateCheckoutSessionRequest.cs)
- `Pricing` (`CheckoutPricingRequest?`): Direct currency and monetary amount.
- `ProductCart` (`IList<CheckoutCartItemRequest>?`): Items referenced from catalog with quantities.
- `Customer` (`CheckoutCustomerRequest?`): Pre-filled buyer email and name.
- `SuccessUrl` (`string`): Absolute URL to redirect the buyer after successful payment.
- `CancelUrl` (`string`): Absolute URL to redirect the buyer if they cancel checkout.
- `Metadata` (`IDictionary<string, string>?`): Key-value pairs attached to the session.

### [`CheckoutSession`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Models/Checkout/CheckoutSession.cs)
- `Id` (`string`): Unique session ID starting with `chk_`.
- `Status` (`string`): `open`, `completed`, or `expired`.
- `PaymentStatus` (`string`): `unpaid`, `paid`, or `no_payment_required`.
- `CheckoutUrl` (`string`): Hosted Bachs checkout page URL.
- `PaymentId` (`string?`): Associated charge identifier (`ch_...`).
- `Customer` (`CheckoutCustomerRecord?`): Embedded customer information.
- `Pricing` (`CheckoutPricing?`): Total amount, currency, and discount details.
- `ChargeSummary` (`CheckoutChargeSummary?`): Breakdown of subtotal, tax, and fees.
- `CreatedAt` (`DateTimeOffset`): Timestamp when the session was initialized.

---

## Associated Webhooks

- `checkout.completed`: Fired when a customer completes payment through the hosted checkout page.
- `checkout.expired`: Fired when an open checkout session expires without completion.
