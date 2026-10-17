# Refunds Client

The Refunds client handles issuing full and partial refunds against previously captured payments, inspecting refund statuses, and looking up all refunds tied to a specific transaction.

- **Interface**: [`IRefundsClient`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs/Clients/IRefundsClient.cs)
- **Implementation**: [`RefundsClient`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs/Clients/RefundsClient.cs)
- **Property**: `client.Refunds`
- **DI Service**: `services.AddBachsClient(...)` registers `IRefundsClient` as a transient service.

> **Handling responses:** Every client method returns `BachsResponse<T>` and returns (rather than throws) on API errors (4xx/5xx). Always check `response.IsSuccessStatusCode` first: on **failure**, inspect `response.Error`/`response.StatusCode` and handle it (log it, retry, or surface it to the caller); on **success**, read the parsed payload from `response.Data`. Transport, timeout, circuit-breaker, and serialization failures still throw typed `Bachs.Exceptions` — see [Response Handling](../README.md#response-handling).

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
using Bachs.Clients;
using Bachs.Common;
using Bachs.Models.Refunds;

var response = await client.Refunds.CreateAsync(new CreateRefundRequest
{
    ChargeId = "ch_100200300",
    Reason = "Customer requested cancellation before delivery"
}, new BachsRequestOptions
{
    IdempotencyKey = "ref_order_100200300"
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Refund creation failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var refund = response.Data;

Console.WriteLine($"Refund ID: {refund.Id}");
Console.WriteLine($"Status: {refund.Status}"); // 'succeeded', 'processing', 'failed'
```

#### Example B: Partial Refund with Explicit Amount

```csharp
var response = await client.Refunds.CreateAsync(new CreateRefundRequest
{
    ChargeId = "ch_100200300",
    Amount = 15.00m, // Partial amount in same currency as charge
    Reason = "Partial courtesy discount"
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Partial refund creation failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var partialRefund = response.Data;
```

---

### 2. Retrieve a Refund

```csharp
var response = await client.Refunds.GetAsync("ref_555666777");
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Refund lookup failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var refund = response.Data;

Console.WriteLine($"Refund Amount: {refund.Amount} {refund.Currency}");
Console.WriteLine($"Charge ID: {refund.ChargeId}");
Console.WriteLine($"Status: {refund.Status}");
```

---

### 3. List Refunds

```csharp
var response = await client.Refunds.ListAsync(new ListRefundsOptions
{
    ChargeId = "ch_100200300",
    Limit = 10
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Refund list request failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var page = response.Data;

foreach (var refItem in page.Items)
{
    Console.WriteLine($"Refund {refItem.Id}: {refItem.Amount} {refItem.Currency} ({refItem.Status})");
}
```

---

### 4. Get All Refunds for a Payment

A dedicated helper to retrieve all refunds attached to a charge:

```csharp
var response = await client.Refunds.GetByPaymentIdAsync("ch_100200300");
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Refund query failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var refunds = response.Data;

Console.WriteLine($"Total refunds issued for charge: {refunds.Items.Count}");
```

---

## Models

### [`CreateRefundRequest`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs/Models/Refunds/Refund.cs)
- `ChargeId` (`string`): The payment identifier starting with `ch_`.
- `Amount` (`BachsPrice?`): Amount to refund. If omitted, the entire remaining charge balance is refunded.
- `Reason` (`string?`): Explanation for the refund for audit trails and customer receipts.
- `Reference` (`string`): Unique external reference for the refund.
- `RefundAddress` (`string?`): Crypto wallet address to receive the refund, for crypto payments.

### [`Refund`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs/Models/Refunds/Refund.cs)
- `Id` (`string`): Starts with `ref_`. Also exposed as `RefundId` (the `refund_id` webhook alias).
- `ChargeId` (`string`): Target charge identifier.
- `Amount` (`BachsPrice`): Monetary value refunded.
- `RequestedAmount` / `RefundedAmount` (`BachsPrice?`): Requested and total refunded amounts (webhook events).
- `RefundFeeAmount` (`BachsPrice?`) and `FeeBearer` (`string?`): Refund fee and who bears it.
- `Currency` (`BachsCurrency`): Currency code matching the original payment (e.g. `BachsCurrency.USD`).
- `Status` (`string`): `succeeded`, `processing`, or `failed`.
- `Reason` (`string?`): Reason provided during initiation.
- `Reference` (`string?`): External reference.
- `CreatedAt` (`DateTimeOffset`): Timestamp of refund creation.

---

## Associated Webhooks

- `refund.succeeded`: Fired when a refund successfully credits back to the customer.
- `refund.failed`: Fired when a refund could not be processed by the issuing bank.
