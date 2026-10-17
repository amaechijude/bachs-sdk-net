# Balances & Payout Schedule Settings Client

The Balances client allows you to inspect account funds across all supported currencies, checking available, pending, and locked reserve funds, as well as configuring automated payout schedule preferences.

- **Interface**: [`IBalancesClient`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs/Clients/IBalancesClient.cs)
- **Implementation**: [`BalancesClient`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs/Clients/BalancesClient.cs)
- **Property**: `client.Balances`
- **DI Service**: `services.AddBachsClient(...)` registers `IBalancesClient` as a transient service.

> **Handling responses:** Every client method returns `BachsResponse<T>` and returns (rather than throws) on API errors (4xx/5xx). Always check `response.IsSuccessStatusCode` first: on **failure**, inspect `response.Error`/`response.StatusCode` and handle it (log it, retry, or surface it to the caller); on **success**, read the parsed payload from `response.Data`. Transport, timeout, circuit-breaker, and serialization failures still throw typed `Bachs.Exceptions` — see [Response Handling](../README.md#response-handling).

---

## Methods Overview

| Method | HTTP | Path | Description |
|---|---|---|---|
| [`GetBalancesAsync`](#1-retrieve-account-balances) | `GET` | `/v1/balances` | Retrieves multi-currency ledger balances (available, pending, locked). |
| [`GetSettingsAsync`](#2-retrieve-balance-settings) | `GET` | `/v1/balances/settings` | Retrieves payout schedule settings and default destination. |
| [`UpdateSettingsAsync`](#3-update-balance-settings) | `PATCH` | `/v1/balances/settings` | Updates automatic payout schedule and destination defaults. |

---

## Usage Examples

### 1. Retrieve Account Balances

```csharp
using Bachs.Clients;

var response = await client.Balances.GetBalancesAsync();
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Balance lookup failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var balance = response.Data;

Console.WriteLine($"Total Consolidated (USD): ${balance.TotalUsd}");

foreach (var (currency, bucket) in balance.Currencies)
{
    Console.WriteLine($"[{currency}]");
    Console.WriteLine($"  Available: {bucket.Available}");
    Console.WriteLine($"  Pending:   {bucket.Pending}");
    Console.WriteLine($"  Locked:    {bucket.Locked}");
}
```

#### Querying for a Connect Account:
```csharp
using Bachs.Common;

// Check balance of a connected merchant
var response = await client.Balances.GetBalancesAsync(new BachsRequestOptions
{
    AccountId = "acc_merchant_456"
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Merchant balance lookup failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var merchantBalance = response.Data;
```

---

### 2. Retrieve Balance Settings

```csharp
var response = await client.Balances.GetSettingsAsync();
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Balance settings lookup failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var settings = response.Data;

Console.WriteLine($"Payout Schedule: {settings.PayoutSchedule}"); // 'manual', 'daily', 'weekly', 'monthly'
Console.WriteLine($"Default Destination: {settings.DefaultDestinationId}");
```

---

### 3. Update Balance Settings

```csharp
using Bachs.Models.Balances;

var response = await client.Balances.UpdateSettingsAsync(new BalanceSettings
{
    PayoutSchedule = "daily",
    DefaultDestinationId = "dest_bank_998877"
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Balance settings update failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var updated = response.Data;

Console.WriteLine($"Updated Schedule to: {updated.PayoutSchedule}");
```

---

## Models

### [`Balance`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs/Models/Balances/Balance.cs)
- `Currencies` (`IReadOnlyDictionary<string, BalanceBucket>`): Dictionary mapping currency codes (e.g. `USD`, `NGN`, `EUR`) to their balance breakdown.
- `TotalUsd` (`BachsPrice?`): Consolidated portfolio balance converted to USD.

### [`BalanceBucket`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs/Models/Balances/Balance.cs)
- `Available` (`BachsPrice`): Liquid funds available immediately for payouts or conversions.
- `Pending` (`BachsPrice`): In-transit settlements from recent customer charges.
- `Locked` (`BachsPrice`): Funds held for ongoing dispute resolutions or collateral reserves.
- `Currency` (`BachsCurrency`): ISO currency code (e.g. `BachsCurrency.USD`).

### [`BalanceSettings`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs/Models/Balances/Balance.cs)
- `PayoutSchedule` (`string?`): Automation cadence (`manual`, `daily`, `weekly`, `monthly`).
- `DefaultDestinationId` (`string?`): The target [`PayoutDestination`](payouts.md#payout-destinations-client) ID for automatic sweep withdrawals.
