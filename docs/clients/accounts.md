# Connected Accounts (Bachs Connect) Client

The Accounts client manages multi-party marketplaces, platforms, and merchant onboarding via Bachs Connect. Create connected accounts, generate hosted onboarding links, query sub-account status, and inspect self-profile information.

- **Interface**: [`IAccountsClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/IAccountsClient.cs)
- **Implementation**: [`AccountsClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/AccountsClient.cs)
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

var response = await client.Accounts.CreateAsync(new CreateAccountRequest
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
if (!response.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Connected account creation failed: {response.Error?.Detail ?? "Unknown error"}");
}

var account = response.Data;

Console.WriteLine($"Connected Account ID: {account.Id}");
```

---

### 2. Generate Onboarding Link (Hosted KYC)

Redirect the merchant to Bachs's hosted onboarding flow to securely collect banking, tax, and KYC identity documents:

```csharp
var response = await client.Accounts.CreateAccountLinkAsync(account.Id, new CreateAccountLinkRequest
{
    Type = "onboarding",
    ReturnUrl = "https://marketplace.com/onboarding/complete",
    RefreshUrl = "https://marketplace.com/onboarding/refresh"
});
if (!response.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Account onboarding link creation failed: {response.Error?.Detail ?? "Unknown error"}");
}

var link = response.Data;

Console.WriteLine($"Direct merchant to: {link.Url}");
Console.WriteLine($"Link expires at: {link.ExpiresAt}");
```

---

### 3. Retrieve a Connected Account

```csharp
var response = await client.Accounts.GetAsync("acc_100200300");
if (!response.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Connected account lookup failed: {response.Error?.Detail ?? "Unknown error"}");
}

var details = response.Data;

Console.WriteLine($"Business: {details.BusinessName}");
Console.WriteLine($"Type: {details.Type}");
Console.WriteLine($"Country: {details.Country}");
```

---

### 4. Acting on Behalf of a Connected Account

Once onboarded, execute operations (such as creating checkout sessions or requesting payouts) on their behalf using `RequestOptions.AccountId`:

```csharp
using Bachs.Net.Common;
using Bachs.Net.Models.Checkout;
using Bachs.Net.Models.Checkout.Request;
using Bachs.Net.Models.Common;

var response = await client.CheckoutSessions.CreateAsync(new CreateCheckoutSessionRequest
{
    Pricing = new CheckoutPricingRequest { Currency = BachsCurrency.USD, Amount = 100.00m }, // Currency defaults to BachsCurrency.USD
    SuccessUrl = "https://shop.com/success",
    CancelUrl = "https://shop.com/cancel"
}, new RequestOptions
{
    AccountId = account.Id // Routes request to this sub-account
});
if (!response.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Merchant checkout session creation failed: {response.Error?.Detail ?? "Unknown error"}");
}

var merchantSession = response.Data;
```

---

### 5. Inspect Self / Platform Profile

```csharp
var response = await client.Accounts.GetMeAsync();
if (!response.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Current account lookup failed: {response.Error?.Detail ?? "Unknown error"}");
}

var me = response.Data;

Console.WriteLine($"Current platform account: {me.Id} ({me.BusinessName})");
```

---

## Models

### [`Account`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Accounts/Account.cs)
- `Id` (`string`): Starts with `acc_`.
- `Type` (`string`): `standard`, `custom`, `express`.
- `Country` (`string`): Two-letter ISO country code.
- `Email` (`string?`): Merchant contact email.
- `BusinessName` (`string?`): Commercial or legal name.
- `CreatedAt` (`DateTimeOffset`): Timestamp of creation.

### [`AccountLink`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Accounts/Account.cs)
- `Url` (`string`): Hosted Bachs onboarding / identity verification web page.
- `ExpiresAt` (`DateTimeOffset`): Expiration of the one-time link.
