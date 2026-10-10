# Subscriptions Client

The Subscriptions client manages recurring billing lifecycles, plan updates, status inquiries, and subscription cancellations in Bachs.

- **Interface**: [`ISubscriptionsClient`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Clients/ISubscriptionsClient.cs)
- **Implementation**: [`SubscriptionsClient`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Clients/SubscriptionsClient.cs)
- **Property**: `client.Subscriptions`
- **DI Service**: `services.AddBachsClient(...)` registers `ISubscriptionsClient` as a transient service.

---

## Methods Overview

| Method | HTTP | Path | Description |
|---|---|---|---|
| [`GetAsync`](#1-retrieve-a-subscription) | `GET` | `/v1/subscriptions/{subscriptionId}` | Retrieves a subscription by its ID. |
| [`ListAsync`](#2-list-subscriptions) | `GET` | `/v1/subscriptions` | Lists subscriptions filtered by customer or status with pagination. |
| [`UpdateAsync`](#3-update-a-subscription) | `PATCH` | `/v1/subscriptions/{subscriptionId}` | Upgrades or downgrades the plan product and modifies metadata. |
| [`CancelAsync`](#4-cancel-a-subscription) | `POST` | `/v1/subscriptions/{subscriptionId}/cancel` | Immediately cancels an active subscription. |

---

## Usage Examples

### 1. Retrieve a Subscription

```csharp
using Bachs.Net.Clients;

var subscription = await client.Subscriptions.GetAsync("sub_100200300");

Console.WriteLine($"Status: {subscription.Status}"); // 'active', 'past_due', 'canceled', 'trialing'
Console.WriteLine($"Customer ID: {subscription.CustomerId}");
Console.WriteLine($"Product ID: {subscription.ProductId}");
Console.WriteLine($"Current Period End: {subscription.CurrentPeriodEnd:yyyy-MM-dd}");
```

---

### 2. List Subscriptions

```csharp
using Bachs.Net.Models.Subscriptions;

var page = await client.Subscriptions.ListAsync(new ListSubscriptionsOptions
{
    CustomerId = "cus_123456789",
    Limit = 20
});

foreach (var sub in page.Data)
{
    Console.WriteLine($"Subscription {sub.Id} ({sub.Status}) renewal: {sub.CurrentPeriodEnd:d}");
}
```

---

### 3. Update / Switch Subscription Plan

Upgrades or downgrades a subscription to a different catalog product:

```csharp
var updated = await client.Subscriptions.UpdateAsync("sub_100200300", new UpdateSubscriptionRequest
{
    ProductId = "prod_enterprise_tier",
    Metadata = new Dictionary<string, string>
    {
        ["upgrade_reason"] = "Customer upgraded via in-app banner"
    }
});

Console.WriteLine($"Switched to product: {updated.ProductId}");
```

---

### 4. Cancel a Subscription

```csharp
var canceled = await client.Subscriptions.CancelAsync("sub_100200300");

Console.WriteLine($"Subscription {canceled.Id} is now {canceled.Status}");
Console.WriteLine($"Canceled at: {canceled.CanceledAt}");
```

---

## Models

### [`Subscription`](file:///home/amaechi/code/nuget/Bachs.Net/src/Bachs.Net/Models/Subscriptions/Subscription.cs)
- `Id` (`string`): Starts with `sub_`.
- `CustomerId` (`string`): Associated customer identifier (`cus_...`).
- `ProductId` (`string`): Current plan product identifier (`prod_...`).
- `Status` (`string`): `active`, `past_due`, `canceled`, `unpaid`, or `trialing`.
- `CurrentPeriodStart` (`DateTimeOffset?`): Start of the current billing cycle.
- `CurrentPeriodEnd` (`DateTimeOffset?`): Next renewal or expiration date.
- `CanceledAt` (`DateTimeOffset?`): Timestamp when the subscription was canceled.
- `CreatedAt` (`DateTimeOffset`): Timestamp when subscription started.

---

## Associated Webhooks

- `customer.subscription.created`: Triggered when a new subscription is initialized.
- `customer.subscription.updated`: Triggered upon plan changes, renewals, or invoice attempts.
- `customer.subscription.deleted`: Triggered upon cancellation or failure to collect recurring payments.
