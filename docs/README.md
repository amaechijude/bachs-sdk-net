# Bachs.Net API Documentation

Welcome to the comprehensive reference documentation for the **Bachs .NET SDK** (`Bachs.Net`).

This guide details all 16 specialized sub-clients available through [`IBachsClient`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Clients/IBachsClient.cs), enabling seamless integration with Bachs payment infrastructure, hosted checkout, product catalog, customer management, subscriptions, payouts, Connect accounts, and webhooks.

---

## Architecture Overview

`Bachs.Net` organizes operations into domain-specific clients. You can interact with these clients either:
1. **Directly via the root client instance**:
   ```csharp
   var client = new BachsClient("sk_test_...");
   var session = await client.CheckoutSessions.CreateAsync(request);
   ```
2. **Via Granular ASP.NET Core Dependency Injection**:
   ```csharp
   // In Program.cs
   builder.Services.AddBachsClient(options => options.ApiKey = "sk_test_...")
       .AddStandardResilienceHandler(); // Configurable via Microsoft.Extensions.Http.Resilience

   // In your endpoint or service
   app.MapPost("/checkout", async (ICheckoutSessionsClient checkoutClient) => { ... });
   ```

All sub-clients inherit from [`BaseClient`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Clients/BaseClient.cs), execute resilient HTTP requests with built-in retries and exponential backoff (or through custom `Microsoft.Extensions.Http.Resilience` pipelines configured on `IHttpClientBuilder`), and deserialize responses directly into strongly-typed C# models.

---

## API Clients Directory

| Client | Interface | Access Property | Description | Guide |
|---|---|---|---|---|
| **Hosted Checkout** | `ICheckoutSessionsClient` | `client.CheckoutSessions` | Create and inspect hosted checkout sessions with hosted URLs | [Checkout Sessions](clients/checkout-sessions.md) |
| **Products & Catalog** | `IProductsClient` | `client.Products` | Manage product catalog items, prices, billing cycles, and archiving | [Products](clients/products.md) |
| **Customers & Portal** | `ICustomersClient` | `client.Customers` | Create, retrieve, update customers and create billing portal sessions | [Customers](clients/customers.md) |
| **Subscriptions** | `ISubscriptionsClient` | `client.Subscriptions` | Inspect, update billing tiers, and cancel customer subscriptions | [Subscriptions](clients/subscriptions.md) |
| **Payments** | `IPaymentsClient` | `client.Payments` | Retrieve and query payment transactions, charges, and settlement fees | [Payments](clients/payments.md) |
| **Refunds** | `IRefundsClient` | `client.Refunds` | Issue full and partial refunds against captured charges | [Refunds](clients/refunds.md) |
| **Payouts & Quotes** | `IPayoutsClient` | `client.Payouts` | Initiate withdrawals, calculate cross-currency FX quotes, and list payouts | [Payouts & Quotes](clients/payouts.md#payouts-client) |
| **Payout Destinations** | `IPayoutDestinationsClient` | `client.PayoutDestinations` | Register and manage bank accounts, mobile money, and crypto wallets | [Payout Destinations](clients/payouts.md#payout-destinations-client) |
| **Balances & Settings** | `IBalancesClient` | `client.Balances` | Check ledger balances across currencies and configure payout schedules | [Balances](clients/balances.md) |
| **Currency Conversions** | `IConversionsClient` | `client.Conversions` | Request FX conversion quotes and execute balance conversions | [Conversions](clients/conversions.md) |
| **Internal Transfers** | `ITransfersClient` | `client.Transfers` | Transfer balances between platform and connected accounts | [Transfers](clients/transfers.md) |
| **Disputes & Chargebacks**| `IDisputesClient` | `client.Disputes` | Manage disputed payments, attach evidence documents, and submit claims | [Disputes](clients/disputes.md) |
| **Connected Accounts** | `IAccountsClient` | `client.Accounts` | Onboard and manage Connect accounts, persons, and onboarding links | [Connected Accounts](clients/accounts.md) |
| **Virtual Bank Accounts** | `IVirtualAccountsClient` | `client.VirtualAccounts` | Generate dedicated virtual bank accounts for automated deposit collections | [Virtual Accounts](clients/virtual-accounts.md) |
| **Webhook Endpoints** | `IWebhooksClient` | `client.Webhooks` | Manage destination URLs, rotate signing secrets, and inspect event deliveries | [Webhooks](clients/webhooks.md) |
| **Reference & Utilities** | `IMiscClient` | `client.Misc` | Query supported currencies, Nigerian bank codes, and resolve account names | [Misc & Utilities](clients/misc.md) |

---

## Common Features Across All Clients

### 1. Per-Request Options ([`RequestOptions`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Common/RequestOptions.cs))
All write and read operations accept an optional `RequestOptions` parameter:
```csharp
var options = new RequestOptions
{
    IdempotencyKey = "order_tx_9841284", // Prevents double-charging or duplicate resource creation
    AccountId = "acc_connected_12345",    // Execute on behalf of a Bachs Connect account
    CustomHeaders = { ["X-Correlation-ID"] = Guid.NewGuid().ToString() }
};

var response = await client.Payments.GetAsync("ch_123", options);
```

### 2. Pagination Options ([`ListRequestOptions`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Common/ListRequestOptions.cs))
All list operations accept specialized options derived from `ListRequestOptions`:
```csharp
var listOptions = new ListPaymentsOptions
{
    Limit = 50,              // 1 to 100
    Cursor = "cur_next_abc", // Cursor-based pagination
    Offset = 0               // Offset-based pagination
};

var page = await client.Payments.ListAsync(listOptions);
Console.WriteLine($"Retrieved {page.Data.Count} items. HasMore: {page.Pagination?.HasMore}");
```

### 3. Error Handling
The SDK provides typed exceptions inheriting from [`BachsApiException`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Common/BachsApiException.cs):
- [`BachsValidationException`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Common/BachsValidationException.cs) — HTTP 400 / 422 with `ValidationErrors` list.
- [`BachsAuthenticationException`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Common/BachsAuthenticationException.cs) — HTTP 401 Unauthorized.
- [`BachsForbiddenException`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Common/BachsForbiddenException.cs) — HTTP 403 Forbidden.
- [`BachsNotFoundException`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Common/BachsNotFoundException.cs) — HTTP 404 Not Found.
- [`BachsConflictException`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Common/BachsConflictException.cs) — HTTP 409 Conflict.
- [`BachsRateLimitException`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Common/BachsRateLimitException.cs) — HTTP 429 Too Many Requests with `RetryAfter` parsed from header.
