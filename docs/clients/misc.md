# Reference & Utility Client (Misc)

The Misc client provides reference and lookup utilities, such as listing supported currencies, querying financial institution / bank codes, and resolving recipient bank account names before initiating payouts.

- **Interface**: [`IMiscClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/IMiscClient.cs)
- **Implementation**: [`MiscClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/MiscClient.cs)
- **Property**: `client.Misc`
- **DI Service**: `services.AddBachsClient(...)` registers `IMiscClient` as a transient service.

---

## Methods Overview

| Method | HTTP | Path | Description |
|---|---|---|---|
| [`GetSupportedCurrenciesAsync`](#1-list-supported-currencies) | `GET` | `/v1/currencies` | Returns a list of currency codes supported for processing and payouts. |
| [`GetBanksAsync`](#2-query-financial-institutions) | `GET` | `/v1/banks?country={country}` | Retrieves financial institutions and central bank routing codes for a country. |
| [`ResolveBankAccountAsync`](#3-resolve-bank-account-name) | `POST` | `/v1/resolve-account` | Validates a bank account number and resolves the account holder's legal name. |

---

## Usage Examples

### 1. List Supported Currencies

```csharp
using Bachs.Net.Clients;

var currencies = await client.Misc.GetSupportedCurrenciesAsync();

Console.WriteLine("Supported Currencies:");
foreach (var cur in currencies)
{
    Console.WriteLine($" - {cur}"); // e.g. USD, EUR, NGN, GBP
}
```

---

### 2. Query Financial Institutions / Banks

Look up bank routing codes (default country is `"NG"`):

```csharp
var banks = await client.Misc.GetBanksAsync("NG");

foreach (var bank in banks)
{
    Console.WriteLine($"Bank: {bank.Name} (Code: {bank.Code})");
}
```

---

### 3. Resolve Bank Account Name (Account Validation)

Validate that an account number belongs to the expected recipient before initiating a payout or saving a destination:

```csharp
using Bachs.Net.Models.Misc;

var resolution = await client.Misc.ResolveBankAccountAsync(new ResolveBankAccountRequest
{
    AccountNumber = "0123456789",
    BankCode = "058" // GTBank
});

Console.WriteLine($"Account Number: {resolution.AccountNumber}");
Console.WriteLine($"Account Holder: {resolution.AccountName}");
Console.WriteLine($"Bank Code:      {resolution.BankCode}");
```

---

## Models

### [`BankInfo`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Misc/MiscModels.cs)
- `Code` (`string`): Institutional sorting/routing code.
- `Name` (`string`): Commercial bank or financial institution name.

### [`ResolveBankAccountRequest`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Misc/MiscModels.cs)
- `AccountNumber` (`string`): Account number to resolve.
- `BankCode` (`string`): Target bank's clearing code.

### [`ResolveBankAccountResponse`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Misc/MiscModels.cs)
- `AccountNumber` (`string`): Account number verified.
- `AccountName` (`string`): Legal name registered to the account.
- `BankCode` (`string`): Bank code.
