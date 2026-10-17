# Bachs SDK For .Net

> Modern, Developer-first C# / .NET 8+ SDK for [Bachs](https://docs.bachs.io/) payments, billing, hosted checkout, subscriptions, and webhooks.

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

## Disclaimer

This project is independently developed and maintained by Amaechi Ugwu. It is not an official Bachs product and is not supported by the Bachs team.

Issues related to the SDK should be reported in this repository, not to Bachs support.

---

## Features

- ⚡ **Full API Coverage**: Checkout sessions, products, customers, subscriptions, payments, refunds, payouts, balances, conversions, transfers, disputes, connected accounts (Connect), and virtual accounts.
- 🔒 **Production & Sandbox Ready**: Automatically selects Sandbox (`https://sandbox-api.bachs.io`) for `sk_sandbox_` keys and Production (`https://api.bachs.io`) for `sk_live_` keys.
- 💉 **First-Class ASP.NET Core DI Support**: Easily configure with `services.AddBachsClient(...)` using typed HTTP client factories and modular service injection.
- 🔁 **Resilience & Rate Limiting**: Built-in retries with exponential backoff and jitter plus a configurable circuit breaker for transient errors (429, 5xx, network failures).
- 🛡️ **Webhook Verification**: Built-in verification for `X-Bachs-Signature` and `X-Bachs-Signature-V2` with zero-downtime secret rotation support and anti-replay tolerance checks.
- 💰 **Monetary Precision**: Transparent decimal-string handling (`"29.00"`) compliant with the Bachs API precision standard.
- 🔑 **Idempotency & Connect Support**: Effortlessly pass `Idempotency-Key` and `X-Account-Id` per request.

---

## Installation

Install the package via NuGet CLI:

```bash
dotnet add package Bachs.Sdk
```

Or via Package Manager Console in Visual Studio:

```powershell
Install-Package Bachs.Sdk
```

---

## Getting Started

### 1. Standalone Client Usage

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Common;

// Initializes automatically against the sandbox based on the key prefix.
// The second argument is the webhook signing secret (optional for pure API usage).
var client = new BachsClient("sk_sandbox_your_secret_key", "whsec_your_webhook_secret");

// Or configure explicitly with options:
var client = new BachsClient(new BachsClientOptions
{
    ApiKey = "sk_sandbox_your_secret_key",
    WebhookSecret = "whsec_your_webhook_secret"
});
```

Standalone clients use a shared `HttpClient`. To apply the configurable timeout,
retry, and circuit-breaker pipeline, register the client with `AddBachsClient`.
When the webhook secret is configured, the DI-registered `IWebhookEventValidator`
uses it as the default secret during verification.

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
    options.WebhookSecret = builder.Configuration["Bachs:WebhookSecret"]!;
});

var app = builder.Build();
```

You can now inject either the root `IBachsClient` or specific sub-clients into your endpoints and services:

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Models.Checkout;
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
        // Handle the error: log it, then return an appropriate problem response to the caller.
        return Results.Problem($"Checkout session creation failed: {response.Error?.Detail ?? "Unknown error"}");
    }

    return Results.Ok(new { checkoutUrl = response.Data.CheckoutUrl });
});
```

---

### 3. Retry and Circuit Breaker Configuration

The dependency-injected client uses a standard HTTP resilience pipeline configured
through `BachsClientOptions`. By default, requests have a 30-second total timeout
and a 10-second per-attempt timeout, with up to 3 retries using exponential
backoff (500 ms base delay, capped at 5 seconds per delay, with jitter). The
circuit breaker uses a 50% failure ratio after at least 20 requests in a 30-second
sampling window and remains open for 15 seconds. Retries can be disabled with
`MaxRetries = 0`, and the circuit breaker with `EnableCircuitBreaker = false`.

```csharp
builder.Services.AddBachsClient(options =>
{
    options.ApiKey = builder.Configuration["Bachs:ApiKey"]!;
    options.WebhookSecret = builder.Configuration["Bachs:WebhookSecret"]!;

    options.TotalRequestTimeout = TimeSpan.FromSeconds(45);
    options.AttemptTimeout = TimeSpan.FromSeconds(12);

    options.MaxRetries = 2;
    options.RetryDelay = TimeSpan.FromSeconds(5); // Bachs official delay is 5 seconds

    options.CircuitBreakerFailureRatio = 0.2;
    options.CircuitBreakerMinimumThroughput = 20;
    options.CircuitBreakerSamplingDuration = TimeSpan.FromSeconds(30);
    options.CircuitBreakerBreakDuration = TimeSpan.FromSeconds(15);
});
```

Standalone clients created with `new BachsClient(...)` use a shared `HttpClient`
without this resilience pipeline. The timeout, retry, and circuit-breaker options
are applied to clients registered with `AddBachsClient`.

---

## Result Pattern & Error Handling

All public API methods return a `BachsResponse<T>` that exposes the parsed payload on `.Data`, the HTTP `StatusCode`, and — on failure — the parsed `BachsError` on `.Error` (with `IsSuccessStatusCode` as the fast success check). API errors are therefore data rather than exceptions, so you don't need a `try/catch` around every call.

```csharp
var response = await client.CheckoutSessions.CreateAsync(request);
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it (response.Error has the parsed details) and take the appropriate action.
    return;
}

var session = response.Data;
```

API errors (4xx/5xx) are returned in `BachsResponse<T>.Error`; the HTTP status is on
`StatusCode` and the success check is `IsSuccessStatusCode`. Genuine transport and
serialization failures still throw typed exceptions from the `Bachs.Net.Exceptions`
namespace — see [Error Handling](#error-handling) below.

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
    new BachsRequestOptions("order-invoice-1002")
);

if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Checkout session creation failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
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
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Product creation failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
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

    // Validates the signature, enforces the 5-minute timestamp tolerance, and parses a typed event.
    // Uses the configured BachsClientOptions.WebhookSecret when secret is omitted.
    if (!webhooks.TryValidateTypedEvent(rawBody, signature, out var webhookEvent, timestampHeader: timestamp))
    {
        return Results.BadRequest(new { error = "Invalid webhook signature or payload." });
    }

    switch (webhookEvent)
    {
        case CollectionSucceededEvent paymentEvent:
            var paymentData = paymentEvent.Data;
            // Fulfill order
            break;

        case CheckoutCompletedEvent checkoutEvent:
            // Handle checkout completed
            break;

        case SubscriptionCreatedEvent subscriptionEvent:
            // Activate customer subscription
            break;

        case UnknownBachsEvent unknownEvent:
            // Log or safely ignore unrecognized event types.
            Console.WriteLine($"Unhandled webhook event: {unknownEvent.Type}");
            break;
    }

    return Results.Ok();
});
```

`webhooks.TryValidateTypedEvent(rawBody, signature, out var webhookEvent, timestampHeader: timestamp)` returns `false` when validation fails and otherwise provides a `BachsTypedEvent` you can pattern-match on (`CollectionSucceededEvent`, `CheckoutCompletedEvent`, …, with `UnknownBachsEvent` as the forward-compatible fallback). Signature validation without an API client is available through the standalone `WebhookEventValidator` or the static `BachsWebhook` class. See [Webhooks documentation](docs/clients/webhooks.md#cryptographic-signature-verification) for secret resolution, tolerance windows, and both signature header formats.

### Error Handling

API failures (4xx/5xx) are returned, not thrown:

```csharp
var response = await client.Products.CreateAsync(new CreateProductRequest { /* ... */ });
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log the parsed details (or surface them to the caller).
    var error = response.Error;
    Console.WriteLine($"API Error [{error?.ErrorCode}] (HTTP {(int)response.StatusCode}): {error?.Detail}");
    foreach (var fieldError in error?.Errors ?? [])
    {
        Console.WriteLine($" - {fieldError.Field}: {fieldError.Message}");
    }
}
```

Transport and serialization failures throw typed exceptions instead. Every exception
derives from `BachsException` in the `Bachs.Net.Exceptions` namespace:

```csharp
using Bachs.Net.Exceptions;

try
{
    await client.Products.CreateAsync(new CreateProductRequest { /* ... */ });
}
catch (BachsTimeoutException)               // per-attempt / total timeout exceeded
{
}
catch (BachsServiceUnavailableException)    // circuit breaker open
{
}
catch (BachsNetworkException)               // DNS / socket / connection failure
{
}
catch (BachsSerializationException)         // malformed JSON
{
}
```

In ASP.NET Core, you can handle these exceptions centrally with `IExceptionHandler`.
This example preserves the HTTP status from API exceptions and maps transport
failures to appropriate gateway statuses:

```csharp
using Bachs.Net.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddExceptionHandler<BachsExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();
app.UseExceptionHandler();

public sealed class BachsExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is not BachsException bachsException)
        {
            return false; // Allow other exception handler catch it;
        }

        var (statusCode, title, detail) = bachsException switch
        {
            BachsValidationException apiException =>
                (apiException.StatusCode, "Bachs request validation failed", apiException.Detail),
            BachsApiException apiException =>
                (apiException.StatusCode, "Bachs API request failed", apiException.Detail),
            BachsTimeoutException =>
                (HttpStatusCode.GatewayTimeout, "Bachs request timed out", "The Bachs request timed out."),
            BachsServiceUnavailableException =>
                (HttpStatusCode.ServiceUnavailable, "Bachs is temporarily unavailable", "Please try again later."),
            BachsNetworkException =>
                (HttpStatusCode.BadGateway, "Could not reach Bachs", "The upstream request failed."),
            BachsSerializationException =>
                (HttpStatusCode.BadGateway, "Invalid response from Bachs", "The upstream response could not be read."),
            _ =>
                (HttpStatusCode.InternalServerError, "Bachs SDK error", "The request could not be completed.")
        };

        httpContext.Response.StatusCode = (int)statusCode;
        await httpContext.Response.WriteAsJsonAsync(
            new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = detail
            },
            cancellationToken);

        return true;
    }
}
```

This handles only thrown `BachsException` instances. API errors returned in
`BachsResponse<T>` still need to be checked and handled by your endpoint unless
you convert them to exceptions with `BachsApiException.Create(...)`.

The typed HTTP-error exceptions (`BachsValidationException`, `BachsNotFoundException`,
`BachsRateLimitException`, …) live in the same namespace and are produced by
`BachsApiException.Create(...)` when you want to convert an unsuccessful response into
a rich exception:

```csharp
var response = await client.Products.CreateAsync(new CreateProductRequest { /* ... */ });
if (!response.IsSuccessStatusCode && response.Error is { } apiError)
{
    throw BachsApiException.Create(response.StatusCode, apiError, rateLimit: null, rawResponse: null);
}
```

---

## License

This project is licensed under the [MIT License](LICENSE).
