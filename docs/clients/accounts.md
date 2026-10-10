# Connected Accounts (Bachs Connect) Client

The Accounts client manages multi-party marketplaces, platforms, and merchant onboarding via Bachs Connect. Create connected accounts, generate hosted onboarding links, query sub-account status, and inspect self-profile information.

- **Interface**: [`IAccountsClient`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Clients/IAccountsClient.cs)
- **Implementation**: [`AccountsClient`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Clients/AccountsClient.cs)
- **Property**: `client.Accounts`
- **DI Service**: `services.AddBachsClient(...)` registers `IAccountsClient` as a transient service.

---

## Methods Overview

| Method | HTTP | Path | Description |
|---|---|---|---|
| [`CreateAsync`](#1-create-a-connected-account) | `POST` | `/v1/accounts` | Registers a new connected account (sub-account). |
| [`GetAsync`](#2-retrieve-a-connected-account) | `GET` | `/v1/accounts/{accountId}` | Retrieves account status and capabilities. |
| [`ListAsync`](#3-list-connected-accounts) | `GET` | `/v1/accounts` | Lists connected accounts with pagination. |
| [`UpdateAsync`](#4-update-a-connected-account) | `PATCH` | `/v1/accounts/{accountId}` | Modifies account contact or business details. |
| [`CreateAccountLinkAsync`](#5-generate-onboarding-link) | `POST` | `/v1/accounts/{accountId}/account_links` | Generates a hosted URL for merchant onboarding or identity verification. |
| [`GetMeAsync`](#6-retrieve-current-account-profile) | `GET` | `/v1/accounts/me` | Inspects the platform or current authenticated account profile. |

---

## Usage Examples

### 1. Create a Connected Account

Create a merchant account for your marketplace:

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Common;
using Bachs.Net.Models.Accounts;

var account = await client.Accounts.CreateAsync(new CreateAccountRequest
{
    Type = "standard", // 'standard', 'custom', 'express'
    Country = "NG",
    Email = "vendor@shopmarketplace.com",
    BusinessName = "Lagos Artisan Roasters",
    Capabilities = new List<string> { "transfers", "payouts" }
}, new RequestOptions
{
    IdempotencyKey = "onboard_vendor_4819"
});

Console.WriteLine($"Connected Account ID: {account.Id}");
```

---

### 2. Generate Onboarding Link (Hosted KYC)

Redirect the merchant to Bachs's hosted onboarding flow to securely collect banking, tax, and KYC identity documents:

```csharp
var link = await client.Accounts.CreateAccountLinkAsync(account.Id, new CreateAccountLinkRequest
{
    Type = "onboarding",
    ReturnUrl = "https://marketplace.com/onboarding/complete",
    RefreshUrl = "https://marketplace.com/onboarding/refresh"
});

Console.WriteLine($"Direct merchant to: {link.Url}");
Console.WriteLine($"Link expires at: {link.ExpiresAt}");
```

---

### 3. Retrieve a Connected Account

```csharp
var details = await client.Accounts.GetAsync("acc_100200300");

Console.WriteLine($"Business: {details.BusinessName}");
Console.WriteLine($"Type: {details.Type}");
Console.WriteLine($"Country: {details.Country}");
```

---

### 4. Acting on Behalf of a Connected Account

Once onboarded, execute operations (such as creating checkout sessions or requesting payouts) on their behalf using `RequestOptions.AccountId`:

```csharp
var merchantSession = await client.CheckoutSessions.CreateAsync(new CreateCheckoutSessionRequest
{
    Pricing = new CheckoutPricingRequest { Currency = "USD", Amount = 100.00m },
    SuccessUrl = "https://shop.com/success",
    CancelUrl = "https://shop.com/cancel"
}, new RequestOptions
{
    AccountId = account.Id // Routes request to this sub-account
});
```

---

### 5. Inspect Self / Platform Profile

```csharp
var me = await client.Accounts.GetMeAsync();

Console.WriteLine($"Current platform account: {me.Id} ({me.BusinessName})");
```

---

## Models

### [`Account`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Models/Accounts/Account.cs)
- `Id` (`string`): Starts with `acc_`.
- `Type` (`string`): `standard`, `custom`, `express`.
- `Country` (`string`): Two-letter ISO country code.
- `Email` (`string?`): Merchant contact email.
- `BusinessName` (`string?`): Commercial or legal name.
- `CreatedAt` (`DateTimeOffset`): Timestamp of creation.

### [`AccountLink`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Models/Accounts/Account.cs)
- `Url` (`string`): Hosted Bachs onboarding / identity verification web page.
- `ExpiresAt` (`DateTimeOffset`): Expiration of the one-time link.
