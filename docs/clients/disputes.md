# Disputes & Chargebacks Client

The Disputes client allows you to manage payment disputes, query chargebacks, submit defense evidence before deadlines, and formally challenge claims.

- **Interface**: [`IDisputesClient`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs/Clients/IDisputesClient.cs)
- **Implementation**: [`DisputesClient`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs/Clients/DisputesClient.cs)
- **Property**: `client.Disputes`
- **DI Service**: `services.AddBachsClient(...)` registers `IDisputesClient` as a transient service.

> **Handling responses:** Every client method returns `BachsResponse<T>` and returns (rather than throws) on API errors (4xx/5xx). Always check `response.IsSuccessStatusCode` first: on **failure**, inspect `response.Error`/`response.StatusCode` and handle it (log it, retry, or surface it to the caller); on **success**, read the parsed payload from `response.Data`. Transport, timeout, circuit-breaker, and serialization failures still throw typed `Bachs.Exceptions` — see [Response Handling](../README.md#response-handling).

---

## Methods Overview

| Method | HTTP | Path | Description |
|---|---|---|---|
| [`GetAsync`](#1-retrieve-a-dispute) | `GET` | `/v1/disputes/{disputeId}` | Retrieves dispute details and evidence due dates. |
| [`ListAsync`](#2-list-disputes) | `GET` | `/v1/disputes` | Lists disputes filtered by status or charge ID. |
| [`UpdateEvidenceAsync`](#3-update-evidence) | `PATCH` | `/v1/disputes/{disputeId}/evidence` | Uploads and saves evidence explanation and documents. |
| [`SubmitAsync`](#4-submit-dispute-defense) | `POST` | `/v1/disputes/{disputeId}/submit` | Formally submits evidence to card networks for adjudication. |

---

## Usage Examples

### 1. Retrieve a Dispute

```csharp
using Bachs.Clients;

var response = await client.Disputes.GetAsync("dp_100200300");
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Dispute lookup failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var dispute = response.Data;

Console.WriteLine($"Dispute ID: {dispute.Id}");
Console.WriteLine($"Charge ID: {dispute.ChargeId}");
Console.WriteLine($"Amount: {dispute.Amount} {dispute.Currency}");
Console.WriteLine($"Status: {dispute.Status}"); // 'needs_response', 'under_review', 'won', 'lost'
Console.WriteLine($"Evidence Due: {dispute.EvidenceDueDate:yyyy-MM-dd}");
```

---

### 2. List Disputes

```csharp
using Bachs.Models.Disputes;

var response = await client.Disputes.ListAsync(new ListDisputesOptions
{
    Status = "needs_response",
    Limit = 15
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Dispute list request failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var page = response.Data;

foreach (var d in page.Items)
{
    Console.WriteLine($"Action Required: {d.Id} for {d.Amount} {d.Currency} (Due {d.EvidenceDueDate:d})");
}
```

---

### 3. Update Evidence

Save customer communication logs, order fulfillment proof, and explanation before submitting:

```csharp
var response = await client.Disputes.UpdateEvidenceAsync("dp_100200300", new UpdateDisputeEvidenceRequest
{
    CustomerName = "John Doe",
    CustomerEmail = "john.doe@example.com",
    ShippingDocumentation = "Delivered via FedEx, tracking 99882233, signed for on 2026-04-20.",
    UncategorizedText = "Order was successfully delivered to the billing address on file."
});
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Dispute evidence update failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var updated = response.Data;

Console.WriteLine($"Evidence saved for dispute: {updated.Id}");
```

---

### 4. Formally Submit Evidence

Once evidence is attached, submit the claim to the issuing bank:

```csharp
var response = await client.Disputes.SubmitAsync("dp_100200300");
if (!response.IsSuccessStatusCode)
{
    // Handle the error: log it and take the appropriate action (return, retry, or surface it).
    // e.g. logger.LogError("Dispute submission failed ({StatusCode}): {Detail}", (int)response.StatusCode, response.Error?.Detail);
    return;
}

var submitted = response.Data;

Console.WriteLine($"Dispute submitted! Status: {submitted.Status}"); // 'under_review'
```

---

## Models

### [`Dispute`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs/Models/Disputes/Dispute.cs)
- `Id` (`string`): Starts with `dp_`.
- `ChargeId` (`string`): Disputed transaction identifier (`ch_...`).
- `Amount` (`BachsPrice`): Monetary value under dispute.
- `Currency` (`BachsCurrency`): Currency code (e.g. `BachsCurrency.USD`).
- `Status` (`string`): `needs_response`, `under_review`, `won`, `lost`.
- `Reason` (`string?`): Dispute code (e.g., `fraudulent`, `unrecognized`, `product_not_received`).
- `EvidenceDueDate` (`DateTimeOffset?`): Final deadline to provide defense evidence.
- `CreatedAt` (`DateTimeOffset`): Timestamp when dispute was opened.

### [`UpdateDisputeEvidenceRequest`](https://github.com/amaechijude/bachs-sdk-net/blob/main/src/Bachs/Models/Disputes/Dispute.cs)
- `CustomerName` (`string?`): Name on customer file.
- `CustomerEmail` (`string?`): Contact email.
- `ServiceDocumentation` (`string?`): Description of the service or product provided.
- `ShippingDocumentation` (`string?`): Proof of shipping/delivery details.
- `UncategorizedText` (`string?`): Free-form narrative refuting the dispute.

---

## Associated Webhooks

- `dispute.created`: Fired when a customer files a chargeback with their card issuer.
- `dispute.updated`: Fired when dispute state updates or evidence is acknowledged.
- `dispute.closed`: Fired when the issuer issues a final determination (`won` or `lost`).
