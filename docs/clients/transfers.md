# Internal Transfers Client

The Transfers client enables platforms to move funds internally between the platform balance and connected accounts (or between connected accounts) in Bachs.

- **Interface**: [`ITransfersClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/ITransfersClient.cs)
- **Implementation**: [`TransfersClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/TransfersClient.cs)
- **Property**: `client.Transfers`
- **DI Service**: `services.AddBachsClient(...)` registers `ITransfersClient` as a transient service.

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
using Bachs.Net.Models.Transfers;

var transfer = await client.Transfers.CreateAsync(new CreateTransferRequest
{
    Amount = 250.00m,
    Currency = "USD",
    Destination = "acc_merchant_12345" // Target connected account ID
}, new RequestOptions
{
    IdempotencyKey = "transfer_batch_item_99"
});

Console.WriteLine($"Transfer ID: {transfer.Id}");
Console.WriteLine($"Status: {transfer.Status}"); // 'succeeded', 'pending', 'failed'
```

---

### 2. Retrieve a Transfer

```csharp
var record = await client.Transfers.GetAsync("tr_100200300");

Console.WriteLine($"Transferred: {record.Amount} {record.Currency} to {record.Destination}");
Console.WriteLine($"Executed at: {record.CreatedAt:u}");
```

---

### 3. List Transfers

```csharp
var page = await client.Transfers.ListAsync(new ListTransfersOptions
{
    Limit = 25
});

foreach (var item in page.Data)
{
    Console.WriteLine($"Transfer {item.Id}: {item.Amount} {item.Currency} => {item.Destination} ({item.Status})");
}
```

---

## Models

### [`CreateTransferRequest`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Transfers/Transfer.cs)
- `Amount` (`Price`): Monetary value to transfer.
- `Currency` (`string`): Currency code (must match source and destination account currency capability).
- `Destination` (`string`): Connected account identifier starting with `acc_`.

### [`Transfer`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Transfers/Transfer.cs)
- `Id` (`string`): Starts with `tr_`.
- `Amount` (`Price`): Transferred amount.
- `Currency` (`string`): Three-letter currency code.
- `Destination` (`string`): Recipient connected account ID.
- `Status` (`string`): `succeeded`, `pending`, `failed`.
- `CreatedAt` (`DateTimeOffset`): Timestamp of creation.
