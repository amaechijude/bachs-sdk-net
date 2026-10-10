# Disputes & Chargebacks Client

The Disputes client allows you to manage payment disputes, query chargebacks, submit defense evidence before deadlines, and formally challenge claims.

- **Interface**: [`IDisputesClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/IDisputesClient.cs)
- **Implementation**: [`DisputesClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/DisputesClient.cs)
- **Property**: `client.Disputes`
- **DI Service**: `services.AddBachsClient(...)` registers `IDisputesClient` as a transient service.

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
using Bachs.Net.Clients;

var dispute = await client.Disputes.GetAsync("dp_100200300");

Console.WriteLine($"Dispute ID: {dispute.Id}");
Console.WriteLine($"Charge ID: {dispute.ChargeId}");
Console.WriteLine($"Amount: {dispute.Amount} {dispute.Currency}");
Console.WriteLine($"Status: {dispute.Status}"); // 'needs_response', 'under_review', 'won', 'lost'
Console.WriteLine($"Evidence Due: {dispute.EvidenceDueDate:yyyy-MM-dd}");
```

---

### 2. List Disputes

```csharp
using Bachs.Net.Models.Disputes;

var page = await client.Disputes.ListAsync(new ListDisputesOptions
{
    Status = "needs_response",
    Limit = 15
});

foreach (var d in page.Data)
{
    Console.WriteLine($"Action Required: {d.Id} for {d.Amount} {d.Currency} (Due {d.EvidenceDueDate:d})");
}
```

---

### 3. Update Evidence

Save customer communication logs, order fulfillment proof, and explanation before submitting:

```csharp
var updated = await client.Disputes.UpdateEvidenceAsync("dp_100200300", new UpdateDisputeEvidenceRequest
{
    CustomerName = "John Doe",
    CustomerEmail = "john.doe@example.com",
    Explanation = "Order was successfully delivered via FedEx with tracking number 99882233.",
    DocumentIds = new List<string> { "doc_proof_of_delivery_1" }
});

Console.WriteLine($"Evidence saved for dispute: {updated.Id}");
```

---

### 4. Formally Submit Evidence

Once evidence is attached, submit the claim to the issuing bank:

```csharp
var submitted = await client.Disputes.SubmitAsync("dp_100200300");

Console.WriteLine($"Dispute submitted! Status: {submitted.Status}"); // 'under_review'
```

---

## Models

### [`Dispute`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Disputes/Dispute.cs)
- `Id` (`string`): Starts with `dp_`.
- `ChargeId` (`string`): Disputed transaction identifier (`ch_...`).
- `Amount` (`decimal`): Monetary value under dispute.
- `Currency` (`string`): Currency code.
- `Status` (`string`): `needs_response`, `under_review`, `won`, `lost`.
- `Reason` (`string?`): Dispute code (e.g., `fraudulent`, `unrecognized`, `product_not_received`).
- `EvidenceDueDate` (`DateTimeOffset?`): Final deadline to provide defense evidence.
- `CreatedAt` (`DateTimeOffset`): Timestamp when dispute was opened.

### [`UpdateDisputeEvidenceRequest`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Disputes/Dispute.cs)
- `CustomerName` (`string?`): Name on customer file.
- `CustomerEmail` (`string?`): Contact email.
- `Explanation` (`string?`): Detailed written narrative refuting the dispute.
- `DocumentIds` (`IList<string>?`): IDs of supporting receipt, delivery, or communication files.

---

## Associated Webhooks

- `dispute.created`: Fired when a customer files a chargeback with their card issuer.
- `dispute.updated`: Fired when dispute state updates or evidence is acknowledged.
- `dispute.closed`: Fired when the issuer issues a final determination (`won` or `lost`).
