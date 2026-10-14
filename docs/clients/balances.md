# Balances & Payout Schedule Settings Client

The Balances client allows you to inspect account funds across all supported currencies, checking available, pending, and locked reserve funds, as well as configuring automated payout schedule preferences.

- **Interface**: [`IBalancesClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/IBalancesClient.cs)
- **Implementation**: [`BalancesClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/BalancesClient.cs)
- **Property**: `client.Balances`
- **DI Service**: `services.AddBachsClient(...)` registers `IBalancesClient` as a transient service.

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
using Bachs.Net.Clients;

var response = await client.Balances.GetBalancesAsync();
if (!response.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Balance lookup failed: {response.Error?.Detail ?? "Unknown error"}");
}

var balance = response.Data;

Console.WriteLine($"Total Consolidated (USD): ${balance.TotalUsd:F2}");

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
using Bachs.Net.Common;

// Check balance of a connected merchant
var response = await client.Balances.GetBalancesAsync(new RequestOptions
{
    AccountId = "acc_merchant_456"
});
if (!response.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Merchant balance lookup failed: {response.Error?.Detail ?? "Unknown error"}");
}

var merchantBalance = response.Data;
```

---

### 2. Retrieve Balance Settings

```csharp
var response = await client.Balances.GetSettingsAsync();
if (!response.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Balance settings lookup failed: {response.Error?.Detail ?? "Unknown error"}");
}

var settings = response.Data;

Console.WriteLine($"Payout Schedule: {settings.PayoutSchedule}"); // 'manual', 'daily', 'weekly', 'monthly'
Console.WriteLine($"Default Destination: {settings.DefaultDestinationId}");
```

---

### 3. Update Balance Settings

```csharp
using Bachs.Net.Models.Balances;

var response = await client.Balances.UpdateSettingsAsync(new BalanceSettings
{
    PayoutSchedule = "daily",
    DefaultDestinationId = "dest_bank_998877"
});
if (!response.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Balance settings update failed: {response.Error?.Detail ?? "Unknown error"}");
}

var updated = response.Data;

Console.WriteLine($"Updated Schedule to: {updated.PayoutSchedule}");
```

---

## Models

### [`Balance`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Balances/Balance.cs)
- `Currencies` (`IReadOnlyDictionary<string, BalanceBucket>`): Dictionary mapping currency codes (e.g. `USD`, `NGN`, `EUR`) to their balance breakdown.
- `TotalUsd` (`BachsPrice?`): Consolidated portfolio balance converted to USD.

### [`BalanceBucket`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Balances/Balance.cs)
- `Available` (`BachsPrice`): Liquid funds available immediately for payouts or conversions.
- `Pending` (`BachsPrice`): In-transit settlements from recent customer charges.
- `Locked` (`BachsPrice`): Funds held for ongoing dispute resolutions or collateral reserves.
- `Currency` (`BachsCurrency`): ISO currency code (e.g. `BachsCurrency.USD`).

### [`BalanceSettings`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Balances/Balance.cs)
- `PayoutSchedule` (`string?`): Automation cadence (`manual`, `daily`, `weekly`, `monthly`).
- `DefaultDestinationId` (`string?`): The target [`PayoutDestination`](payouts.md#payout-destinations-client) ID for automatic sweep withdrawals.
