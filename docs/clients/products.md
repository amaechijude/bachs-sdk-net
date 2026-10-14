# Products & Billing Catalog Client

The Products client allows you to manage catalog items in Bachs, defining one-time pricing, recurring subscriptions (with custom frequencies and trial periods), and lifecycle status.

- **Interface**: [`IProductsClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/IProductsClient.cs)
- **Implementation**: [`ProductsClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/ProductsClient.cs)
- **Property**: `client.Products`
- **DI Service**: `services.AddBachsClient(...)` registers `IProductsClient` as a transient service.

---

## Methods Overview

| Method | HTTP | Path | Description |
|---|---|---|---|
| [`CreateAsync`](#1-create-a-product) | `POST` | `/v1/products` | Creates a new catalog product with pricing and billing rules. |
| [`GetAsync`](#2-retrieve-a-product) | `GET` | `/v1/products/{productId}` | Retrieves product details by ID. |
| [`ListAsync`](#3-list-products) | `GET` | `/v1/products` | Lists products with optional pagination and archive inclusion. |
| [`UpdateAsync`](#4-update-a-product) | `PATCH` | `/v1/products/{productId}` | Updates product name, description, or metadata. |
| [`ArchiveAsync`](#5-archive-a-product) | `POST` | `/v1/products/{productId}/archive` | Marks a product as inactive/archived to prevent new purchases. |
| [`UnarchiveAsync`](#6-unarchive-a-product) | `POST` | `/v1/products/{productId}/unarchive` | Restores an archived product to active status. |

---

## Usage Examples

### 1. Create a Product

#### Example A: One-Time Payment Product

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Models.Common;
using Bachs.Net.Models.Products;

var response = await client.Products.CreateAsync(new CreateProductRequest
{
    Name = "E-Book: Cloud Architecture",
    Description = "Comprehensive PDF guide and source code examples.",
    Price = new ProductPriceRequest
    {
        PriceType = "fixed",
        Amount = 19.99m,
        Currency = BachsCurrency.USD // Defaults to BachsCurrency.USD
    }
});
if (!response.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Product creation failed: {response.Error?.Detail ?? "Unknown error"}");
}

var product = response.Data;

Console.WriteLine($"Created Product ID: {product.Id}");
```

#### Example B: Recurring Subscription Plan with Trial

```csharp
var response = await client.Products.CreateAsync(new CreateProductRequest
{
    Name = "Pro Membership",
    Description = "Access to full analytics suite and unlimited storage.",
    Price = new ProductPriceRequest
    {
        PriceType = "fixed",
        Amount = 49.00m,
        Currency = BachsCurrency.USD // Defaults to BachsCurrency.USD
    },
    BillingCycle = new ProductBillingCycleRequest
    {
        Interval = "month",
        Frequency = 1
    },
    TrialPeriod = new ProductTrialPeriodRequest
    {
        Interval = "day",
        Frequency = 14 // 14-day free trial
    }
});
if (!response.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Subscription plan creation failed: {response.Error?.Detail ?? "Unknown error"}");
}

var subscriptionPlan = response.Data;
```

---

### 2. Retrieve a Product

```csharp
var response = await client.Products.GetAsync("prod_pro_plan");
if (!response.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Product lookup failed: {response.Error?.Detail ?? "Unknown error"}");
}

var product = response.Data;

Console.WriteLine($"Name: {product.Name}");
Console.WriteLine($"Price: {product.Price?.Amount} {product.Price?.Currency}");
Console.WriteLine($"Archived: {product.Archived}");
```

---

### 3. List Products

```csharp
var response = await client.Products.ListAsync(new ListProductsOptions
{
    Limit = 25,
    IncludeArchived = false
});
if (!response.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Product list request failed: {response.Error?.Detail ?? "Unknown error"}");
}

var page = response.Data;

foreach (var item in page.Data)
{
    Console.WriteLine($"Product: {item.Id} - {item.Name} ({item.Price?.Amount} {item.Price?.Currency})");
}
```

---

### 4. Update a Product

```csharp
var response = await client.Products.UpdateAsync("prod_pro_plan", new UpdateProductRequest
{
    Name = "Pro Membership (2026 Edition)",
    Description = "Updated feature set with new team collaboration tools."
});
if (!response.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Product update failed: {response.Error?.Detail ?? "Unknown error"}");
}

var updated = response.Data;
```

---

### 5. Archive and Unarchive Products

```csharp
// Archive to retire from checkout
var archiveResponse = await client.Products.ArchiveAsync("prod_legacy_tier");
if (!archiveResponse.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Archive request failed: {archiveResponse.Error?.Detail ?? "Unknown error"}");
}

var archived = archiveResponse.Data;
Console.WriteLine($"Is Archived: {archived.Archived}"); // true

// Reactivate when needed
var restoreResponse = await client.Products.UnarchiveAsync("prod_legacy_tier");
if (!restoreResponse.IsSuccessStatusCode)
{
    throw new InvalidOperationException($"Unarchive request failed: {restoreResponse.Error?.Detail ?? "Unknown error"}");
}

var restored = restoreResponse.Data;
Console.WriteLine($"Is Archived: {restored.Archived}"); // false
```

---

## Models

### [`CreateProductRequest`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Products/CreateProductRequest.cs)
- `Name` (`string`): The name shown at checkout.
- `Description` (`string?`): Product details or marketing summary.
- `Price` (`ProductPriceRequest`): Amount, currency, and pricing model (`fixed`, `free`, `custom`).
- `BillingCycle` (`ProductBillingCycleRequest?`): Recurring interval (`day`, `week`, `month`, `year`) and frequency.
- `TrialPeriod` (`ProductTrialPeriodRequest?`): Trial duration cadence and frequency.
- `Images` (`IList<string>?`): Image URLs for hosted checkout rendering.

### [`Product`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Products/Product.cs)
- `Id` (`string`): Starts with `prod_`.
- `Name` (`string`): Display name.
- `Price` (`ProductPrice?`): Configured pricing breakdown.
- `BillingCycle` (`ProductBillingCycle?`): Recurring billing rules.
- `TrialPeriod` (`ProductTrialPeriod?`): Trial duration configuration.
- `Archived` (`bool`): Active vs retired flag.
