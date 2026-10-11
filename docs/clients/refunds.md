# Refunds Client

The Refunds client handles issuing full and partial refunds against previously captured payments, inspecting refund statuses, and looking up all refunds tied to a specific transaction.

- **Interface**: [`IRefundsClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/IRefundsClient.cs)
- **Implementation**: [`RefundsClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/RefundsClient.cs)
- **Property**: `client.Refunds`
- **DI Service**: `services.AddBachsClient(...)` registers `IRefundsClient` as a transient service.

---

## Methods Overview

| Method | HTTP | Path | Description |
|---|---|---|---|
| [`CreateAsync`](#1-create-a-refund) | `POST` | `/v1/refunds` | Initiates a full or partial refund against a captured charge. |
| [`GetAsync`](#2-retrieve-a-refund) | `GET` | `/v1/refunds/{refundId}` | Retrieves a refund record by ID. |
| [`ListAsync`](#3-list-refunds) | `GET` | `/v1/refunds` | Lists refunds with filtering and pagination. |
| [`GetByPaymentIdAsync`](#4-get-refunds-for-a-payment) | `GET` | `/v1/refunds?charge_id={paymentId}` | Retrieves all refunds issued against a specific charge ID. |

---

## Usage Examples

### 1. Create a Refund

#### Example A: Full Refund

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Common;
using Bachs.Net.Models.Refunds;

var refund = await client.Refunds.CreateAsync(new CreateRefundRequest
{
    PaymentId = "ch_100200300",
    Reason = "Customer requested cancellation before delivery"
}, new RequestOptions
{
    IdempotencyKey = "ref_order_100200300"
});

Console.WriteLine($"Refund ID: {refund.Id}");
Console.WriteLine($"Status: {refund.Status}"); // 'pending', 'succeeded', 'failed'
```

#### Example B: Partial Refund with Explicit Amount

```csharp
var partialRefund = await client.Refunds.CreateAsync(new CreateRefundRequest
{
    PaymentId = "ch_100200300",
    Amount = 15.00m, // Partial amount in same currency as charge
    Reason = "Partial courtesy discount"
});
```

---

### 2. Retrieve a Refund

```csharp
var refund = await client.Refunds.GetAsync("ref_555666777");

Console.WriteLine($"Refund Amount: {refund.Amount} {refund.Currency}");
Console.WriteLine($"Charge ID: {refund.PaymentId}");
Console.WriteLine($"Status: {refund.Status}");
```

---

### 3. List Refunds

```csharp
var page = await client.Refunds.ListAsync(new ListRefundsOptions
{
    PaymentId = "ch_100200300",
    Limit = 10
});

foreach (var refItem in page.Data)
{
    Console.WriteLine($"Refund {refItem.Id}: {refItem.Amount} {refItem.Currency} ({refItem.Status})");
}
```

---

### 4. Get All Refunds for a Payment

A dedicated helper to retrieve all refunds attached to a charge:

```csharp
var refunds = await client.Refunds.GetByPaymentIdAsync("ch_100200300");

Console.WriteLine($"Total refunds issued for charge: {refunds.Data.Count}");
```

---

## Models

### [`CreateRefundRequest`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Refunds/Refund.cs)
- `PaymentId` (`string`): The payment identifier starting with `ch_`.
- `Amount` (`Price?`): Amount to refund. If omitted, the entire remaining charge balance is refunded.
- `Reason` (`string?`): Explanation for the refund for audit trails and customer receipts.
- `Metadata` (`IDictionary<string, string>?`): Optional metadata key-value pairs.

### [`Refund`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Refunds/Refund.cs)
- `Id` (`string`): Starts with `ref_`.
- `PaymentId` (`string`): Target charge identifier.
- `Amount` (`Price`): Monetary value refunded.
- `Currency` (`Currency`): Currency code matching the original payment (e.g. `Currency.USD`).
- `Status` (`string`): `pending`, `succeeded`, or `failed`.
- `Reason` (`string?`): Reason provided during initiation.
- `CreatedAt` (`DateTimeOffset`): Timestamp of refund creation.

---

## Associated Webhooks

- `refund.succeeded`: Fired when a refund successfully credits back to the customer.
- `refund.failed`: Fired when a refund could not be processed by the issuing bank.
