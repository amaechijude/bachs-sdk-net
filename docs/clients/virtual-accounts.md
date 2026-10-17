# Dedicated Virtual Accounts Client

The Virtual Accounts client provides fixed, permanent virtual bank accounts (e.g., Nigerian NUBAN accounts) that your users or customers can transfer funds into at any time for continuous automated payment collections.

- **Interface**: [`IVirtualAccountsClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/IVirtualAccountsClient.cs)
- **Implementation**: [`VirtualAccountsClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/VirtualAccountsClient.cs)
- **Property**: `client.VirtualAccounts`
- **DI Service**: `services.AddBachsClient(...)` registers `IVirtualAccountsClient` as a transient service.

> **Handling responses:** Every client method returns `BachsResponse<T>` and returns (rather than throws) on API errors (4xx/5xx). Always check `response.IsSuccessStatusCode` first: on **failure**, inspect `response.Error`/`response.StatusCode` and handle it (log it, retry, or surface it to the caller); on **success**, read the parsed payload from `response.Data`. Transport, timeout, circuit-breaker, and serialization failures still throw typed `Bachs.Net.Exceptions` — see [Response Handling](../README.md#response-handling).

---

## Methods Overview

| Method | HTTP | Path | Description |
|---|---|---|---|
| [`CreateAsync`](#1-create-a-virtual-account) | `POST` | `/v1/virtual-accounts` | Provisions a new permanent virtual deposit account. |
| [`GetAsync`](#2-retrieve-a-virtual-account) | `GET` | `/v1/virtual-accounts?currency={currency}` | Retrieves existing virtual account coordinates for a currency. |

---

## Usage Examples

### 1. Create a Dedicated Virtual Account

Generate a virtual bank account for bank transfer collections:

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Common;
using Bachs.Net.Models.Common;
using Bachs.Net.Models.VirtualAccounts;

var response = await client.VirtualAccounts.CreateAsync(new CreateVirtualAccountRequest
{
    Currency = BachsCurrency.NGN // Defaults to BachsCurrency.USD
}, new BachsRequestOptions
{
    IdempotencyKey = "va_provision_ngn_01"
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Virtual account creation failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var account = response.Data;

Console.WriteLine($"Bank Name: {account.BankName}");
Console.WriteLine($"Account Number: {account.AccountNumber}");
Console.WriteLine($"Account Name: {account.AccountName}");
Console.WriteLine($"Status: {account.Status}"); // 'active'
```

#### Provisioning for a Customer or Connected Account:
```csharp
var accountResponse = await client.VirtualAccounts.CreateAsync(
    new CreateVirtualAccountRequest { Currency = BachsCurrency.NGN },
    new BachsRequestOptions { AccountId = "acc_merchant_123" }
);
if (!accountResponse.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Customer virtual account creation failed ({StatusCode}): {Detail}", (int)accountResponse.StatusCode, accountResponse.Error?.Detail);
    return;
}

var customerAccount = accountResponse.Data;
```

---

### 2. Retrieve Virtual Account Details

```csharp
var response = await client.VirtualAccounts.GetAsync(currency: BachsCurrency.NGN);
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Virtual account lookup failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var existing = response.Data;

Console.WriteLine($"Account: {existing.AccountNumber} ({existing.BankName})");
```

---

## Models

### [`CreateVirtualAccountRequest`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/VirtualAccounts/VirtualAccount.cs)
- `Currency` (`BachsCurrency`): Target currency code (defaults to `BachsCurrency.USD`).

### [`VirtualAccount`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/VirtualAccounts/VirtualAccount.cs)
- `AccountNumber` (`string`): The 10-digit NUBAN account number.
- `AccountName` (`string`): Beneficiary name returned upon bank lookup.
- `BankName` (`string`): Financial institution name (e.g., Wema Bank, Sterling Bank).
- `BankCode` (`string?`): Central bank code for the institution.
- `Currency` (`BachsCurrency`): Currency code (`BachsCurrency.NGN`, `BachsCurrency.USD`, etc.).
- `Status` (`string`): `active`, `suspended`.
- `CreatedAt` (`DateTimeOffset`): Provisioning timestamp.

---

## Associated Webhooks

- `virtual_account.assigned`: Fired when a virtual account number is assigned and ready to receive deposits.
- `collection.succeeded`: Fired whenever a customer transfers money into the virtual account number.

