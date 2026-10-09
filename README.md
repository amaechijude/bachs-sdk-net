# Bachs.Net

> Modern, Developer-first C# / .NET 8+ SDK for [Bachs](https://docs.bachs.io/) payments, billing, hosted checkout, subscriptions, and webhooks.

[![NuGet Version](https://img.shields.io/nuget/v/Bachs.Net.svg)](https://www.nuget.org/packages/Bachs.Net/)
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
app.MapPost("/create-checkout", async (ICheckoutSessionsClient checkoutClient) =>
{
    var session = await checkoutClient.CreateAsync(new CreateCheckoutSessionRequest
    {
        Pricing = new CheckoutPricingRequest
        {
            Currency = "USD",
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

    return Results.Ok(new { checkoutUrl = session.CheckoutUrl });
});
```

---

## Usage Examples

### Hosted Checkout Sessions

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Common;
using Bachs.Net.Models.Checkout;

// Create a checkout session with product catalog items
var session = await client.CheckoutSessions.CreateAsync(
    new CreateCheckoutSessionRequest
    {
        ProductCart = new List<CheckoutCartItemRequest>
        {
            new() { ProductId = "prod_pro_plan", Quantity = 1 }
        },
        Customer = new CheckoutCustomerRequest
        {
            Email = "buyer@example.com",
            Name = "Alex Smith"
        },
        SuccessUrl = "https://myapp.com/success",
        CancelUrl = "https://myapp.com/cart"
    },
    // Optional request options: specify idempotency key
    new RequestOptions("order-invoice-1002")
);

Console.WriteLine($"Redirect customer to: {session.CheckoutUrl}");
```

### Products and Recurring Pricing

```csharp
using Bachs.Net.Models.Products;

// Create a recurring subscription product (Pro Plan, $29/month, 14-day trial)
var product = await client.Products.CreateAsync(new CreateProductRequest
{
    Name = "Pro Plan",
    Description = "Monthly access to Pro features",
    Price = new ProductPriceRequest
    {
        PriceType = "fixed",
        Amount = 29.00m,
        Currency = "USD"
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

Console.WriteLine($"Created product: {product.Id}");
```

### Handling and Verifying Webhooks

In your ASP.NET Core Web API:

```csharp
using Bachs.Net.WebHooks;

app.MapPost("/api/webhooks/bachs", async (HttpRequest request, IConfiguration config) =>
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
    var secret = config["Bachs:WebhookSecret"]!;

    try
    {
        // Verifies the signature (and timestamp tolerance) and parses the event
        var webhookEvent = BachsWebhook.ConstructEvent(rawBody, signature, secret, timestampHeader: timestamp);

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
