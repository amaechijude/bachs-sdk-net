# Bachs.Net API Documentation

Welcome to the comprehensive reference documentation for the **Bachs .NET SDK** (`Bachs.Net`).

This guide details all 16 specialized sub-clients available through [`IBachsClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/IBachsClient.cs), enabling seamless integration with Bachs payment infrastructure, hosted checkout, product catalog, customer management, subscriptions, payouts, Connect accounts, and webhooks.

## Disclaimer

This project is independently developed and maintained by Amaechi Ugwu. It is not an official Bachs product and is not supported by the Bachs team.

Issues related to the SDK should be reported in this repository, not to Bachs support.

---

## Architecture Overview

`Bachs.Net` organizes operations into domain-specific clients. You can interact with these clients either:
1. **Directly via the root client instance**:
   ```csharp
   using Bachs.Net.Clients;

   var client = new BachsClient("sk_sandbox_...", "whsec_...");
   var response = await client.CheckoutSessions.CreateAsync(request);

   if (!response.IsSuccessStatusCode)
   {
       // Handle the error: log it and take the appropriate action (return, retry, or surface it).
       // e.g. logger.LogError("Checkout session creation failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
       return;
   }

   var session = response.Data;
   ```
2. **Via Granular ASP.NET Core Dependency Injection**:
   ```csharp
   // In Program.cs
   builder.Services.AddBachsClient(options =>
   {
       options.ApiKey = "sk_test_...";
       options.TotalRequestTimeout = TimeSpan.FromSeconds(30);
       options.AttemptTimeout = TimeSpan.FromSeconds(10);
       options.MaxRetries = 3;
       options.CircuitBreakerFailureRatio = 0.1;
       options.CircuitBreakerMinimumThroughput = 100;
       options.CircuitBreakerSamplingDuration = TimeSpan.FromSeconds(30);
       options.CircuitBreakerBreakDuration = TimeSpan.FromSeconds(5);
   });

   // In your endpoint or service
   app.MapPost("/checkout", async (ICheckoutSessionsClient checkoutClient) => { ... });
   ```

All sub-clients inherit from [`BaseClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/BaseClient.cs), execute resilient HTTP requests with built-in retries, exponential backoff, and a configurable circuit breaker when registered through dependency injection, and deserialize responses directly into strongly-typed C# models.

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

### 1. Per-Request Options ([`BachsRequestOptions`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Common/BachsRequestOptions.cs))
All write and read operations accept an optional `BachsRequestOptions` parameter:
```csharp
var options = new BachsRequestOptions
{
    IdempotencyKey = "order_tx_9841284", // Prevents double-charging or duplicate resource creation
    AccountId = "acc_connected_12345",    // Execute on behalf of a Bachs Connect account
    CustomHeaders = { ["X-Correlation-ID"] = Guid.NewGuid().ToString() }
};

var response = await client.Payments.GetAsync("ch_123", options);
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it (response.Error has the parsed details) and take the appropriate action.
    return;
}

var payment = response.Data;
```

### 2. Pagination Options ([`BachsListRequestOptions`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Common/BachsListRequestOptions.cs))
All list operations accept specialized options derived from `BachsListRequestOptions`:
```csharp
var listOptions = new ListPaymentsOptions
{
    Limit = 50,              // 1 to 100
    Cursor = "cur_next_abc", // Cursor-based pagination
    Offset = 0               // Offset-based pagination
};

var pageResponse = await client.Payments.ListAsync(listOptions);
if (!pageResponse.IsSuccessStatusCode)
{
    // Handle the error: log it (pageResponse.Error has the parsed details) and take the appropriate action.
    return;
}

var page = pageResponse.Data;
Console.WriteLine($"Retrieved {page.Items.Count} items. HasMore: {page.Pagination?.HasMore}");
```

### 3. Error Handling
API errors (4xx/5xx) are returned in `BachsResponse<T>.Error`; check `IsSuccessStatusCode` before using `.Data`. When you prefer exception-style handling, the SDK also provides typed exceptions inheriting from [`BachsApiException`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Exceptions/BachsApiException.cs) (produced via `BachsApiException.Create(...)`):
- [`BachsValidationException`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Exceptions/BachsValidationException.cs) — HTTP 400 / 422 with `ValidationErrors` list.
- [`BachsAuthenticationException`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Exceptions/BachsAuthenticationException.cs) — HTTP 401 Unauthorized.
- [`BachsForbiddenException`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Exceptions/BachsForbiddenException.cs) — HTTP 403 Forbidden.
- [`BachsNotFoundException`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Exceptions/BachsNotFoundException.cs) — HTTP 404 Not Found.
- [`BachsConflictException`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Exceptions/BachsConflictException.cs) — HTTP 409 Conflict.
- [`BachsRateLimitException`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Exceptions/BachsRateLimitException.cs) — HTTP 429 Too Many Requests with `RetryAfter` parsed from header.

---

## Response Handling

Every public method returns a `BachsResponse<T>` and, on API errors (4xx/5xx), **returns instead of throwing**. Always branch on the result before touching the payload:

```csharp
var response = await client.Payments.GetAsync("ch_123456789");

if (!response.IsSuccessStatusCode)
{
    // Failure — response.Error holds the parsed BachsError (error code, detail, field errors).
    // Log it and take the appropriate action (return, retry, or surface it to the caller).
    logger.LogError(
        "Payment lookup failed ({StatusCode}): {Detail}",
        (int)response.StatusCode,
        response.Error?.Detail);
    return;
}

// Success — use the strongly-typed payload.
var payment = response.Data;
Console.WriteLine($"Payment {payment.Id} is {payment.Status}");
```

| Member | Meaning |
|---|---|
| `IsSuccessStatusCode` | `true` for 2xx responses — the fast check to branch on. |
| `Data` | The deserialized payload (`T`); valid on success. |
| `Error` | The parsed [`BachsError`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Common/BachsError.cs) (error code, message, and field-level errors); populated on failure. |
| `StatusCode` | The raw `System.Net.HttpStatusCode`. |

> **Do not ignore the result.** Skipping the `IsSuccessStatusCode` check silently discards failed operations and leaves `response.Data` unusable.

### Exceptions still thrown

Transport and configuration failures are the only things that throw (every exception derives from `BachsException`):

| Exception | When |
|---|---|
| `BachsNetworkException` | The underlying HTTP call failed (DNS, socket, or connection reset). |
| `BachsTimeoutException` | The request exceeded the per-attempt or total timeout. |
| `BachsServiceUnavailableException` | The resilience circuit breaker is open. |
| `BachsSerializationException` | The response body could not be deserialized. |
| `BachsInvalidOperationException` | Invalid configuration or builder usage (e.g. no webhook secret configured). |
| `BachsArgumentNullException` | A required constructor argument was `null`. |
| `ArgumentNullException` / `ArgumentException` | A required method argument (request body or identifier) was `null`, empty, or whitespace. |

If you prefer exception-style handling for API failures, convert an unsuccessful response with `BachsApiException.Create(...)` — see [Error Handling](#3-error-handling).
