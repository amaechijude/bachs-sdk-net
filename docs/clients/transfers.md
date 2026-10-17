# Internal Transfers Client

The Transfers client enables platforms to move funds internally between the platform balance and connected accounts (or between connected accounts) in Bachs.

- **Interface**: [`ITransfersClient`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs.Net/Clients/ITransfersClient.cs)
- **Implementation**: [`TransfersClient`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs.Net/Clients/TransfersClient.cs)
- **Property**: `client.Transfers`
- **DI Service**: `services.AddBachsClient(...)` registers `ITransfersClient` as a transient service.

> **Handling responses:** Every client method returns `BachsResponse<T>` and returns (rather than throws) on API errors (4xx/5xx). Always check `response.IsSuccessStatusCode` first: on **failure**, inspect `response.Error`/`response.StatusCode` and handle it (log it, retry, or surface it to the caller); on **success**, read the parsed payload from `response.Data`. Transport, timeout, circuit-breaker, and serialization failures still throw typed `Bachs.Net.Exceptions` — see [Response Handling](../README.md#response-handling).

---

## Methods Overview

| Method | HTTP | Path | Description |
|---|---|---|---|
| [`CreateAsync`](#1-create-a-transfer) | `POST` | `/v1/transfers` | Transfers funds from the platform balance to a connected account. |
| [`GetAsync`](#2-retrieve-a-transfer) | `GET` | `/v1/transfers/{transferId}` | Retrieves a transfer record by ID. |
| [`ListAsync`](#3-list-transfers) | `GET` | `/v1/transfers` | Lists transfer transactions with pagination. |

---

## Usage Examples

### 1. Create a Transfer

Transfer funds from your platform treasury to a merchant's connected account:

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Common;
using Bachs.Net.Models.Common;
using Bachs.Net.Models.Transfers;

var response = await client.Transfers.CreateAsync(new CreateTransferRequest
{
    Amount = 250.00m,
    Currency = BachsCurrency.USD, // Defaults to BachsCurrency.USD
    Destination = "acc_merchant_12345" // Target connected account ID
}, new BachsRequestOptions
{
    IdempotencyKey = "transfer_batch_item_99"
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Transfer creation failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var transfer = response.Data;

Console.WriteLine($"Transfer ID: {transfer.Id}");
Console.WriteLine($"Status: {transfer.Status}"); // 'succeeded', 'pending', 'failed'
```

---

### 2. Retrieve a Transfer

```csharp
var response = await client.Transfers.GetAsync("tr_100200300");
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Transfer lookup failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var record = response.Data;

Console.WriteLine($"Transferred: {record.Amount} {record.Currency} to {record.Destination}");
Console.WriteLine($"Executed at: {record.CreatedAt:u}");
```

---

### 3. List Transfers

```csharp
var response = await client.Transfers.ListAsync(new ListTransfersOptions
{
    Limit = 25
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Transfer list request failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var page = response.Data;

foreach (var item in page.Items)
{
    Console.WriteLine($"Transfer {item.Id}: {item.Amount} {item.Currency} => {item.Destination} ({item.Status})");
}
```

---

## Models

### [`CreateTransferRequest`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs.Net/Models/Transfers/Transfer.cs)
- `Amount` (`BachsPrice`): Monetary value to transfer.
- `Currency` (`BachsCurrency`): Currency code (defaults to `BachsCurrency.USD`; must match source and destination account currency capability).
- `Destination` (`string`): Connected account identifier starting with `acc_`.

### [`Transfer`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs.Net/Models/Transfers/Transfer.cs)
- `Id` (`string`): Starts with `tr_`.
- `Amount` (`BachsPrice`): Transferred amount.
- `Currency` (`BachsCurrency`): Three-letter currency code (e.g. `BachsCurrency.USD`).
- `Destination` (`string`): Recipient connected account ID.
- `Status` (`string`): `succeeded`, `pending`, `failed`.
- `CreatedAt` (`DateTimeOffset`): Timestamp of creation.
