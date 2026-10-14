# BachsSDK.Net

> Modern, Developer-first C# / .NET 8+ SDK for [Bachs](https://docs.bachs.io/) payments, billing, hosted checkout, subscriptions, and webhooks.

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

---

## Features

- ⚡ **Full API Coverage**: Checkout sessions, products, customers, subscriptions, payments, refunds, payouts, balances, conversions, transfers, disputes, connected accounts (Connect), and virtual accounts.
- 🔒 **Production & Sandbox Ready**: Automatically selects Sandbox (`https://sandbox-api.bachs.io`) for `sk_sandbox_` keys and Production (`https://api.bachs.io`) for `sk_live_` keys.
- 💉 **First-Class ASP.NET Core DI Support**: Easily configure with `services.AddBachsClient(...)` using typed HTTP client factories and modular service injection.
- 🔁 **Resilience & Rate Limiting**: Built-in automatic retries with exponential backoff and jitter for transient errors (429, 5xx), respecting `Retry-After` headers.
- 🛡️ **Webhook Verification**: Built-in verification for `X-Bachs-Signature` and `X-Bachs-Signature-V2` with zero-downtime secret rotation support and anti-replay tolerance checks.
- 💰 **Monetary Precision**: Transparent decimal-string handling (`"29.00"`) compliant with the Bachs API precision standard.
- 🔑 **Idempotency & Connect Support**: Effortlessly pass `Idempotency-Key` and `X-Account-Id` per request.

---

## Installation

Install the package via NuGet CLI:

```bash
dotnet add package Bachs.Net
```

Or via Package Manager Console in Visual Studio:

```powershell
Install-Package Bachs.Net
```

---

## Getting Started

### 1. Standalone Client Usage

```csharp
using Bachs.Net.Clients;

// Initializes automatically against the sandbox based on the key prefix
var client = new BachsClient("sk_sandbox_your_secret_key");

// Or configure explicitly with options:
var client = new BachsClient(new BachsClientOptions
{
    ApiKey = "sk_sandbox_your_secret_key",
    Timeout = TimeSpan.FromSeconds(30),
    MaxRetries = 2
});
```

---

### 2. ASP.NET Core Dependency Injection

In `Program.cs`:

```csharp
using Bachs.Net.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register Bachs Client from configuration or delegate
builder.Services.AddBachsClient(options =>
{
    options.ApiKey = builder.Configuration["Bachs:ApiKey"]!;
});

var app = builder.Build();
```

You can now inject either the root `IBachsClient` or specific sub-clients into your endpoints and services:

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Models.Checkout;
using Bachs.Net.Models.Checkout.Request;
using Bachs.Net.Models.Common;

app.MapPost("/create-checkout", async (ICheckoutSessionsClient checkoutClient) =>
{
    var response = await checkoutClient.CreateAsync(new CreateCheckoutSessionRequest
    {
        Pricing = new CheckoutPricingRequest
        {
            Currency = BachsCurrency.USD,
            Amount = 49.99m
        },
        Customer = new CheckoutCustomerRequest
        {
            Email = "customer@example.com",
            Name = "Jane Doe"
        },
        SuccessUrl = "https://example.com/checkout/success",
        CancelUrl = "https://example.com/checkout/cancel"
    });

    if (!response.IsSuccessStatusCode)
    {
        return Results.Problem($"Checkout session creation failed: {response.Error?.Detail ?? "Unknown error"}");
    }

    return Results.Ok(new { checkoutUrl = response.Data.CheckoutUrl });
});
```

---

### 3. Custom Resilience with Microsoft.Extensions.Http.Resilience

`AddBachsClient` returns an `IHttpClientBuilder`, allowing you to seamlessly integrate your own resilience pipelines via [Microsoft.Extensions.Http.Resilience](https://www.nuget.org/packages/Microsoft.Extensions.Http.Resilience):

```bash
dotnet add package Microsoft.Extensions.Http.Resilience
```

#### Standard Resilience Handler (Recommended)

When using Microsoft's standard resilience handler (retry, circuit breaker, rate limiter, and timeout), disable internal retries by setting `MaxRetries = 0`:

```csharp
builder.Services.AddBachsClient(options =>
{
    options.ApiKey = builder.Configuration["Bachs:ApiKey"]!;
    options.MaxRetries = 0; // Handled by Polly / Microsoft Resilience pipeline
})
.AddStandardResilienceHandler();
```

#### Custom Pipeline Configuration

You can also customize retry parameters, timeouts, and hedged requests:

```csharp
builder.Services.AddBachsClient(options =>
{
    options.ApiKey = builder.Configuration["Bachs:ApiKey"]!;
    options.MaxRetries = 0;
})
.AddStandardResilienceHandler(options =>
{
    options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(15);
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
});
```

Or configure inline via delegate:

```csharp
builder.Services.AddBachsClient(
    apiKey: builder.Configuration["Bachs:ApiKey"]!,
    configureHttpClient: httpClientBuilder =>
    {
        httpClientBuilder.AddStandardResilienceHandler();
    }
);
```

---

## Result Pattern & Error Handling

All public API methods return a `BachsResponse<T>` wrapper with the parsed payload exposed on `.Data`. This keeps status codes, headers, rate-limit metadata, and raw payloads available without forcing a `try/catch` around every call.

```csharp
var response = await client.CheckoutSessions.CreateAsync(request);
if (!response.IsSuccessStatusCode)
{
    // Inspect response.Error, response.StatusCode, response.RawJson, etc.
    return;
}

var session = response.Data;
```

When you want exception-based behavior for HTTP failures, set `ThrowOnApiErrors = true` on `BachsClientOptions`.

## Sub API Clients & Documentation

`Bachs.Net` provides 16 domain-focused clients. Access them directly via `client.<Resource>` on `IBachsClient` or inject their interfaces individually using ASP.NET Core Dependency Injection:

| Client | Interface | Access Property | Capabilities | Guide |
|---|---|---|---|---|
| **Hosted Checkout** | `ICheckoutSessionsClient` | `client.CheckoutSessions` | Create checkout pages, pricing, cart items, customer details | [Documentation](docs/clients/checkout-sessions.md) |
| **Products & Catalog** | `IProductsClient` | `client.Products` | Products, recurring pricing, intervals, trial periods, archiving | [Documentation](docs/clients/products.md) |
| **Customers & Portal** | `ICustomersClient` | `client.Customers` | Customer profiles, contact details, hosted Customer Portal sessions | [Documentation](docs/clients/customers.md) |
| **Subscriptions** | `ISubscriptionsClient` | `client.Subscriptions` | Recurring billing, plan switching/upgrades, cancellations | [Documentation](docs/clients/subscriptions.md) |
| **Payments & Charges** | `IPaymentsClient` | `client.Payments` | Transaction lookups, payment statuses, platform fee audits | [Documentation](docs/clients/payments.md) |
| **Refunds** | `IRefundsClient` | `client.Refunds` | Full and partial refunds, chargeback refunds | [Documentation](docs/clients/refunds.md) |
| **Payouts & Quotes** | `IPayoutsClient` | `client.Payouts` | Withdrawals, cross-currency FX quotes, status tracking | [Documentation](docs/clients/payouts.md#payouts-client) |
| **Payout Destinations** | `IPayoutDestinationsClient` | `client.PayoutDestinations` | Bank accounts, mobile money, crypto destination management | [Documentation](docs/clients/payouts.md#payout-destinations-client) |
| **Balances & Settings** | `IBalancesClient` | `client.Balances` | Multi-currency ledger balances, sweep payout schedules | [Documentation](docs/clients/balances.md) |
| **Currency Conversions**| `IConversionsClient` | `client.Conversions` | Real-time FX quotes and balance conversions | [Documentation](docs/clients/conversions.md) |
| **Internal Transfers** | `ITransfersClient` | `client.Transfers` | Internal balance movements to connected accounts | [Documentation](docs/clients/transfers.md) |
| **Disputes** | `IDisputesClient` | `client.Disputes` | Chargeback inquiries, evidence uploads, formal defense | [Documentation](docs/clients/disputes.md) |
| **Connected Accounts** | `IAccountsClient` | `client.Accounts` | Bachs Connect marketplace onboarding, KYC account links | [Documentation](docs/clients/accounts.md) |
| **Virtual Accounts** | `IVirtualAccountsClient` | `client.VirtualAccounts` | Permanent virtual bank accounts for deposit collections | [Documentation](docs/clients/virtual-accounts.md) |
| **Webhook Endpoints** | `IWebhooksClient` | `client.Webhooks` | Endpoint registration, secret rotation, event history | [Documentation](docs/clients/webhooks.md) |
| **Reference & Utilities**| `IMiscClient` | `client.Misc` | Supported currencies, Nigerian bank codes, account lookup | [Documentation](docs/clients/misc.md) |

> 📖 **Full Documentation Hub**: View the [Complete API Client Documentation Directory](docs/README.md) for architecture, error handling, and request option patterns.

---

## Usage Examples

### Hosted Checkout Sessions

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Common;
using Bachs.Net.Models.Checkout;
using Bachs.Net.Models.Checkout.Request;

// Create a checkout session with product catalog items using the builder
var request = CreateCheckoutSessionRequestBuilder
    .WithProductCart(new List<CheckoutCartItemRequest>
    {
        new() { ProductId = "prod_pro_plan", Quantity = 1 }
    })
    .WithCustomer(new CheckoutCustomerRequest
    {
        Email = "buyer@example.com",
        Name = "Alex Smith"
    })
    .WithSuccessUrl("https://myapp.com/success")
    .WithCancelUrl("https://myapp.com/cart")
    .Build();

var response = await client.CheckoutSessions.CreateAsync(
    request,
    // Optional request options: specify idempotency key
    new RequestOptions("order-invoice-1002")
);

if (!response.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Checkout session creation failed: {response.Error?.Detail ?? "Unknown error"}");
}

var session = response.Data;

Console.WriteLine($"Redirect customer to: {session.CheckoutUrl}");
```

### Products and Recurring Pricing

```csharp
using Bachs.Net.Models.Common;
using Bachs.Net.Models.Products;

// Create a recurring subscription product (Pro Plan, $29/month, 14-day trial)
var response = await client.Products.CreateAsync(new CreateProductRequest
{
    Name = "Pro Plan",
    Description = "Monthly access to Pro features",
    Price = new ProductPriceRequest
    {
        PriceType = "fixed",
        Amount = 29.00m,
        Currency = BachsCurrency.USD
    },
    BillingCycle = new ProductBillingCycleRequest
    {
        Interval = "month",
        Frequency = 1
    },
    TrialPeriod = new ProductTrialPeriodRequest
    {
        Interval = "day",
        Frequency = 14
    }
});

if (!response.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Product creation failed: {response.Error?.Detail ?? "Unknown error"}");
}

var product = response.Data;

Console.WriteLine($"Created product: {product.Id}");
```

### Handling and Verifying Webhooks

In your ASP.NET Core Web API:

```csharp
using Bachs.Net.Clients;
using Bachs.Net.WebHooks;

app.MapPost("/api/webhooks/bachs", async (HttpRequest request, IWebhooksClient webhooks) =>
{
    // Read the raw body
    using var reader = new StreamReader(request.Body);
    var rawBody = await reader.ReadToEndAsync();

    // Prefer X-Bachs-Signature-V2 header
    var signature = request.Headers["X-Bachs-Signature-V2"].ToString();
    if (string.IsNullOrEmpty(signature))
    {
        signature = request.Headers["X-Bachs-Signature"].ToString();
    }

    var timestamp = request.Headers["X-Bachs-Timestamp"].ToString();

    try
    {
        // Validates signature, anti-replay timestamp tolerance (5 mins), and parses event.
        // Uses configured BachsClientOptions.WebhookSecret if secret parameter is omitted.
        var webhookEvent = webhooks.ValidateEvent(rawBody, signature, timestampHeader: timestamp);

        switch (webhookEvent.Type)
        {
            case "collection.succeeded":
                var paymentData = webhookEvent.GetData<Dictionary<string, object>>();
                // Fulfill order
                break;

            case "checkout.completed":
                // Handle checkout completed
                break;

            case "customer.subscription.created":
                // Activate customer subscription
                break;
        }

        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});
```

Prefer the strongly-typed surface when handling events: `webhooks.ValidateTypedEvent(rawBody, signature, timestampHeader: timestamp)` returns a `BachsTypedEvent` you can pattern-match on (`CollectionSucceededEvent`, `CheckoutCompletedEvent`, …, with `UnknownBachsEvent` as the forward-compatible fallback). Signature validation without an API client is available through the standalone `WebhookEventValidator` or the static `BachsWebhook` class. See [Webhooks documentation](docs/clients/webhooks.md#cryptographic-signature-verification) for secret resolution, tolerance windows, and both signature header formats.

### Error Handling

The SDK throws descriptive, typed exceptions for API failures:

```csharp
using Bachs.Net.Common;

try
{
    await client.Products.CreateAsync(new CreateProductRequest());
}
catch (BachsValidationException ex)
{
    Console.WriteLine($"Validation failed: {ex.Detail}");
    foreach (var error in ex.ValidationErrors)
    {
        Console.WriteLine($" - {error.Field}: {error.Message}");
    }
}
catch (BachsRateLimitException ex)
{
    Console.WriteLine($"Rate limited. Retry after: {ex.RetryAfter?.TotalSeconds} seconds");
}
catch (BachsApiException ex)
{
    Console.WriteLine($"API Error [{ex.ErrorCode}] (HTTP {(int)ex.StatusCode}): {ex.Detail}");
}
```

---

## License

This project is licensed under the [MIT License](LICENSE).
