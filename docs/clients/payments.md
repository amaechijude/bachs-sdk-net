# Payments & Charges Client

The Payments client provides access to captured transactions, settled charges, customer payment methods, and processing fees across all rails supported by Bachs.

- **Interface**: [`IPaymentsClient`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs.Net/Clients/IPaymentsClient.cs)
- **Implementation**: [`PaymentsClient`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs.Net/Clients/PaymentsClient.cs)
- **Property**: `client.Payments`
- **DI Service**: `services.AddBachsClient(...)` registers `IPaymentsClient` as a transient service.

> **Handling responses:** Every client method returns `BachsResponse<T>` and returns (rather than throws) on API errors (4xx/5xx). Always check `response.IsSuccessStatusCode` first: on **failure**, inspect `response.Error`/`response.StatusCode` and handle it (log it, retry, or surface it to the caller); on **success**, read the parsed payload from `response.Data`. Transport, timeout, circuit-breaker, and serialization failures still throw typed `Bachs.Net.Exceptions` — see [Response Handling](../README.md#response-handling).

---

## Methods Overview

| Method | HTTP | Path | Description |
|---|---|---|---|
| [`GetAsync`](#1-retrieve-a-payment) | `GET` | `/v1/payments/{paymentId}` | Retrieves a payment record by its charge ID. |
| [`ListAsync`](#2-list-payments) | `GET` | `/v1/payments` | Lists payments with filtering by status and customer with pagination. |

---

## Usage Examples

### 1. Retrieve a Payment

```csharp
using Bachs.Net.Clients;

var response = await client.Payments.GetAsync("ch_100200300");
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Payment lookup failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var payment = response.Data;

Console.WriteLine($"Payment ID: {payment.Id}");
Console.WriteLine($"Status: {payment.Status}");             // 'succeeded', 'failed', 'pending'
Console.WriteLine($"Amount: {payment.Amount} {payment.Currency}");
Console.WriteLine($"Payment Method: {payment.PaymentMethod}"); // 'card', 'bank_transfer', etc.
Console.WriteLine($"Gross Fee: {payment.Fees?.Amount} {payment.Currency}");
Console.WriteLine($"Settled Net: {payment.Amount - (payment.Fees?.Amount ?? 0m)}");
```

---

### 2. List Payments with Filtering

```csharp
using Bachs.Net.Models.Payments;

var response = await client.Payments.ListAsync(new ListPaymentsOptions
{
    Status = "succeeded",
    CustomerId = "cus_123456789",
    Limit = 25
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Payment list request failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var payments = response.Data;

foreach (var tx in payments.Items)
{
    Console.WriteLine($"Charge {tx.Id}: {tx.Amount:C2} {tx.Currency} ({tx.Status}) at {tx.CreatedAt:g}");
}
```

---

## Models

### [`Payment`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs.Net/Models/Payments/Payment.cs)
- `Id` (`string`): Starts with `ch_`.
- `Amount` (`BachsPrice`): Transaction monetary amount parsed with high precision.
- `Currency` (`BachsCurrency`): Three-letter ISO currency code (e.g., `BachsCurrency.USD`, `BachsCurrency.NGN`).
- `Status` (`string`): `succeeded`, `pending`, `failed`, or `refunded`.
- `CustomerId` (`string?`): Associated customer identifier (`cus_...`).
- `PaymentMethod` (`string?`): Type of instrument used (`card`, `bank_transfer`, `ussd`, `mobile_money`).
- `Fees` (`PaymentFees?`): Breakdown of processing fees.
- `CreatedAt` (`DateTimeOffset`): Timestamp of transaction execution.

### [`PaymentFees`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs.Net/Models/Payments/Payment.cs)
- `Amount` (`BachsPrice`): Processing fee deduction.
- `Currency` (`BachsCurrency`): Fee currency code.

---

## Associated Webhooks

- `collection.succeeded`: Fired when a payment is captured and cleared.
- `collection.failed`: Fired when an attempted payment charge fails or declines.
