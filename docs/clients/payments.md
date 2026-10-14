# Payments & Charges Client

The Payments client provides access to captured transactions, settled charges, customer payment methods, and processing fees across all rails supported by Bachs.

- **Interface**: [`IPaymentsClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/IPaymentsClient.cs)
- **Implementation**: [`PaymentsClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/PaymentsClient.cs)
- **Property**: `client.Payments`
- **DI Service**: `services.AddBachsClient(...)` registers `IPaymentsClient` as a transient service.

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
    throw new InvalidOperationException($"Payment lookup failed: {response.Error?.Detail ?? "Unknown error"}");
}

var payment = response.Data;

Console.WriteLine($"Payment ID: {payment.Id}");
Console.WriteLine($"Status: {payment.Status}");             // 'succeeded', 'failed', 'pending'
Console.WriteLine($"Amount: {payment.Amount} {payment.Currency}");
Console.WriteLine($"Payment Method: {payment.PaymentMethod}"); // 'card', 'bank_transfer', etc.
Console.WriteLine($"Gross Fee: {payment.Fees?.BachsFee} {payment.Currency}");
Console.WriteLine($"Settled Net: {payment.Amount - (payment.Fees?.BachsFee ?? 0m)}");
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
    throw new InvalidOperationException($"Payment list request failed: {response.Error?.Detail ?? "Unknown error"}");
}

var payments = response.Data;

foreach (var tx in payments.Data)
{
    Console.WriteLine($"Charge {tx.Id}: {tx.Amount:C2} {tx.Currency} ({tx.Status}) at {tx.CreatedAt:g}");
}
```

---

## Models

### [`Payment`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Payments/Payment.cs)
- `Id` (`string`): Starts with `ch_`.
- `Amount` (`BachsPrice`): Transaction monetary amount parsed with high precision.
- `Currency` (`BachsCurrency`): Three-letter ISO currency code (e.g., `BachsCurrency.USD`, `BachsCurrency.NGN`).
- `Status` (`string`): `succeeded`, `pending`, `failed`, or `refunded`.
- `CustomerId` (`string?`): Associated customer identifier (`cus_...`).
- `PaymentMethod` (`string?`): Type of instrument used (`card`, `bank_transfer`, `ussd`, `mobile_money`).
- `Fees` (`PaymentFees?`): Breakdown of processing fees.
- `CreatedAt` (`DateTimeOffset`): Timestamp of transaction execution.

### [`PaymentFees`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Payments/Payment.cs)
- `Amount` (`BachsPrice`): Processing fee deduction.
- `Currency` (`BachsCurrency`): Fee currency code.

---

## Associated Webhooks

- `collection.succeeded`: Fired when a payment is captured and cleared.
- `collection.failed`: Fired when an attempted payment charge fails or declines.
