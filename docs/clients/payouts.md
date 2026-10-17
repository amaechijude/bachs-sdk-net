# Payouts & Payout Destinations Client

The Payouts module provides comprehensive withdrawal capabilities, international FX conversion quotes, and payout destination management (bank accounts, mobile money wallets, and crypto addresses).

In `Bachs.Net`, payouts functionality is split into two specialized interfaces:
1. [`IPayoutsClient`](#payouts-client): Initiate withdrawals, request payout quotes, and inspect withdrawal history.
2. [`IPayoutDestinationsClient`](#payout-destinations-client): Register and manage settlement recipient accounts.

---

## Payouts Client

- **Interface**: [`IPayoutsClient`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs.Net/Clients/IPayoutsClient.cs)
- **Property**: `client.Payouts`
- **DI Service**: `services.AddBachsClient(...)` registers `IPayoutsClient` as a transient service.

> **Handling responses:** Every client method returns `BachsResponse<T>` and returns (rather than throws) on API errors (4xx/5xx). Always check `response.IsSuccessStatusCode` first: on **failure**, inspect `response.Error`/`response.StatusCode` and handle it (log it, retry, or surface it to the caller); on **success**, read the parsed payload from `response.Data`. Transport, timeout, circuit-breaker, and serialization failures still throw typed `Bachs.Net.Exceptions` — see [Response Handling](../README.md#response-handling).

### Methods Overview

| Method | HTTP | Path | Description |
|---|---|---|---|
| [`CreateAsync`](#1-initiate-a-payout-withdrawal) | `POST` | `/v1/payouts` | Initiates a withdrawal to a registered destination. |
| [`CreateQuoteAsync`](#2-request-a-payout-quote) | `POST` | `/v1/payouts/quotes` | Requests an exchange rate quote for cross-currency payouts. |
| [`GetAsync`](#3-retrieve-a-payout) | `GET` | `/v1/payouts/{withdrawalId}` | Retrieves a payout withdrawal record by ID. |
| [`ListAsync`](#4-list-payouts) | `GET` | `/v1/payouts` | Lists payouts with status filtering and pagination. |

### Usage Examples

#### 1. Initiate a Payout Withdrawal

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Common;
using Bachs.Net.Models.Common;
using Bachs.Net.Models.Payouts;

var response = await client.Payouts.CreateAsync(new CreatePayoutRequest
{
    Amount = 500.00m,
    Currency = BachsCurrency.USD, // Defaults to BachsCurrency.USD
    DestinationId = "dest_bank_998877",
    Reference = "October 2026 Merchant Settlement"
}, new BachsRequestOptions
{
    IdempotencyKey = "payout_batch_oct_01"
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Payout creation failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var payout = response.Data;

Console.WriteLine($"Payout ID: {payout.Id}");
Console.WriteLine($"Status: {payout.Status}"); // 'pending', 'processing', 'completed', 'failed'
```

#### 2. Request a Payout Quote (Cross-Currency FX)

```csharp
var response = await client.Payouts.CreateQuoteAsync(new CreatePayoutQuoteRequest
{
    DestinationId = "dest_bank_998877",
    Currency = BachsCurrency.USD, // Defaults to BachsCurrency.USD
    Amount = 500.00m
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Payout quote request failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var quote = response.Data;

Console.WriteLine($"Source Amount required: {quote.SourceAmount} {quote.SourceCurrency}");
Console.WriteLine($"Exchange Rate: {quote.Rate}");
Console.WriteLine($"Quote valid until: {quote.ExpiresAt}");
```

#### 3. Retrieve a Payout

```csharp
var response = await client.Payouts.GetAsync("wd_100200300");
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Payout lookup failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var withdrawal = response.Data;

Console.WriteLine($"Payout: {withdrawal.Amount} {withdrawal.Currency} -> {withdrawal.Status}");
```

#### 4. List Payouts

```csharp
var response = await client.Payouts.ListAsync(new ListPayoutsOptions
{
    Status = "completed",
    Limit = 20
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Payout list request failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var page = response.Data;

foreach (var item in page.Items)
{
    Console.WriteLine($"Payout {item.Id}: {item.Amount} {item.Currency} ({item.Status})");
}
```

---

## Payout Destinations Client

- **Interface**: [`IPayoutDestinationsClient`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs.Net/Clients/IPayoutsClient.cs#L51)
- **Property**: `client.PayoutDestinations`
- **DI Service**: `services.AddBachsClient(...)` registers `IPayoutDestinationsClient` as a transient service.

### Methods Overview

| Method | HTTP | Path | Description |
|---|---|---|---|
| `CreateAsync` | `POST` | `/v1/payouts/destinations` | Registers a new payout destination. |
| `GetAsync` | `GET` | `/v1/payouts/destinations/{destinationId}` | Retrieves destination configuration by ID. |
| `ListAsync` | `GET` | `/v1/payouts/destinations` | Lists all registered payout destinations. |
| `UpdateAsync` | `PATCH` | `/v1/payouts/destinations/{destinationId}` | Updates destination details or metadata. |
| `DeleteAsync` | `DELETE` | `/v1/payouts/destinations/{destinationId}` | Deletes/deactivates a payout destination. |

### Usage Examples

#### Register a Bank Account Destination

```csharp
var response = await client.PayoutDestinations.CreateAsync(new CreatePayoutDestinationRequest
{
    Type = "bank_account",
    Currency = BachsCurrency.NGN, // Defaults to BachsCurrency.USD
    Details = new Dictionary<string, object?>
    {
        ["account_number"] = "0123456789",
        ["bank_code"] = "058", // GTBank Nigeria
        ["account_name"] = "Acme Technologies Ltd"
    }
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Payout destination creation failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var destination = response.Data;

Console.WriteLine($"Destination Registered: {destination.Id}");
```

#### Delete a Destination

```csharp
var response = await client.PayoutDestinations.DeleteAsync("dest_bank_998877");
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Payout destination deletion failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

Console.WriteLine("Payout destination removed.");
```

---

## Models

### [`Payout`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs.Net/Models/Payouts/Payout.cs)
- `Id` (`string`): Starts with `wd_`.
- `Amount` (`BachsPrice`): Monetary withdrawal value.
- `Currency` (`BachsCurrency`): Currency code (defaults to `BachsCurrency.USD` on creation).
- `Status` (`string`): `pending`, `processing`, `completed`, `failed`.
- `DestinationId` (`string`): Target payout destination identifier.
- `Reference` (`string?`): Transfer description shown on bank statements.
- `FromCurrency` / `ToCurrency` (`BachsCurrency?`), `ExchangeRate` (`decimal?`), `ToAmount` (`BachsPrice?`): Cross-currency payout details.
- `WithdrawalFee` / `NetFromAmount` (`BachsPrice?`): Fee and net amount debited.
- `CreatedAt` (`DateTimeOffset`): Timestamp of payout initiation.

### [`PayoutDestination`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs.Net/Models/Payouts/Payout.cs)
- `Id` (`string`): Starts with `dest_`.
- `Type` (`string`): Destination category (`bank_account`, `mobile_money`, `crypto`).
- `Currency` (`BachsCurrency`): Settlement currency code.
- `Details` (`JsonElement?`): Provider-specific destination attributes.
- `CreatedAt` (`DateTimeOffset`): Creation timestamp.

---

## Associated Webhooks

- `payout.initiated`: Fired when a withdrawal request is accepted for processing.
- `payout.succeeded`: Fired when settlement funds clear in the recipient's account.
- `payout.failed`: Fired if destination bank returns an error or rejects funds.
