# Webhooks Client & Signature Verification

The Webhooks client allows programmatic registration and management of webhook destination endpoints, subscribing to specific event topics, performing zero-downtime secret rotations, and inspecting past delivered webhook events.

In addition to the client, the SDK provides the static [`BachsWebhook`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/WebHooks/BachsWebhook.cs) class for cryptographic signature verification.

- **Interface**: [`IWebhooksClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/IWebhooksClient.cs)
- **Implementation**: [`WebhooksClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/WebhooksClient.cs)
- **Property**: `client.Webhooks`
- **DI Service**: `services.AddBachsClient(...)` registers `IWebhooksClient` as a transient service.

---

## Methods Overview

| Method | HTTP | Path | Description |
|---|---|---|---|
| [`CreateEndpointAsync`](#1-create-a-webhook-endpoint) | `POST` | `/v1/webhooks/endpoints` | Registers a new HTTPS destination endpoint and generates a signing secret. |
| [`GetEndpointAsync`](#2-retrieve-an-endpoint) | `GET` | `/v1/webhooks/endpoints/{endpointId}` | Retrieves endpoint details and subscribed topics. |
| [`ListEndpointsAsync`](#3-list-endpoints) | `GET` | `/v1/webhooks/endpoints` | Lists all configured webhook destinations. |
| [`UpdateEndpointAsync`](#4-update-an-endpoint) | `PATCH` | `/v1/webhooks/endpoints/{endpointId}` | Updates destination URL or subscribed event list. |
| [`DeleteEndpointAsync`](#5-delete-an-endpoint) | `DELETE` | `/v1/webhooks/endpoints/{endpointId}` | Deletes a webhook destination. |
| [`RotateSecretAsync`](#6-rotate-signing-secret) | `POST` | `/v1/webhooks/endpoints/{endpointId}/rotate-secret` | Rotates the HMAC signing secret with zero downtime. |
| [`ListEventsAsync`](#7-list-webhook-events) | `GET` | `/v1/webhooks/events` | Retrieves history of fired events and payloads. |
| [`GetEventAsync`](#8-retrieve-an-event) | `GET` | `/v1/webhooks/events/{eventId}` | Retrieves a single webhook event record by ID. |

---

## Usage Examples

### 1. Create a Webhook Endpoint

Register your server's endpoint to listen for specific notifications:

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Models.Webhooks;

var endpoint = await client.Webhooks.CreateEndpointAsync(new CreateWebhookEndpointRequest
{
    Url = "https://api.myplatform.com/webhooks/bachs",
    Events = new List<string>
    {
        "collection.succeeded",
        "checkout.completed",
        "customer.subscription.created",
        "customer.subscription.updated",
        "refund.succeeded"
    }
});

Console.WriteLine($"Endpoint ID: {endpoint.Id}");
Console.WriteLine($"Webhook Secret (save securely): {endpoint.Secret}");
```

---

### 2. Retrieve & Update an Endpoint

```csharp
// Retrieve
var ep = await client.Webhooks.GetEndpointAsync("we_100200300");
Console.WriteLine($"URL: {ep.Url} ({string.Join(", ", ep.Events)})");

// Update URL or events
var updated = await client.Webhooks.UpdateEndpointAsync("we_100200300", new UpdateWebhookEndpointRequest
{
    Url = "https://api.myplatform.com/v2/webhooks/bachs",
    Events = new List<string> { "collection.succeeded", "checkout.completed" }
});
```

---

### 3. Rotate Webhook Signing Secret

Generate a new secret while keeping the previous secret valid temporarily during the grace window:

```csharp
var rotation = await client.Webhooks.RotateSecretAsync("we_100200300");

Console.WriteLine($"New Signing Secret: {rotation.Secret}");
// Deploy the new secret to your application configuration
```

---

### 4. Delete an Endpoint

```csharp
await client.Webhooks.DeleteEndpointAsync("we_100200300");
Console.WriteLine("Webhook endpoint unregistered.");
```

---

### 5. Inspect Event History

```csharp
var events = await client.Webhooks.ListEventsAsync();

foreach (var evt in events.Data)
{
    Console.WriteLine($"Event {evt.Id}: {evt.Type} at {evt.CreatedAt}");
}
```

---

## Cryptographic Signature Verification

When receiving webhooks in your ASP.NET Core application, use [`BachsWebhook.ConstructEvent`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/WebHooks/BachsWebhook.cs#L22):

```csharp
using Bachs.Net.WebHooks;

app.MapPost("/webhooks/bachs", async (HttpRequest request, IConfiguration config) =>
{
    using var reader = new StreamReader(request.Body);
    var rawPayload = await reader.ReadToEndAsync();

    // Prefer V2 signature, fall back to V1
    var signature = request.Headers["X-Bachs-Signature-V2"].ToString();
    if (string.IsNullOrEmpty(signature))
    {
        signature = request.Headers["X-Bachs-Signature"].ToString();
    }

    var timestamp = request.Headers["X-Bachs-Timestamp"].ToString();
    var secret = config["Bachs:WebhookSecret"]!;

    try
    {
        // Validates HMAC SHA-256 signature, replay tolerance (5 minutes), and returns strongly-typed event
        var webhookEvent = BachsWebhook.ConstructEvent(
            rawPayload,
            signature,
            secret,
            toleranceSeconds: 300,
            timestampHeader: timestamp
        );

        if (webhookEvent.Type == "collection.succeeded")
        {
            // Deserialize event payload data
            var paymentData = webhookEvent.GetData<Dictionary<string, object>>();
            // Fulfill purchase
        }

        return Results.Ok();
    }
    catch (BachsException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});
```

---

## Models

### [`WebhookEndpoint`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Webhooks/WebhookEndpoint.cs)
- `Id` (`string`): Starts with `we_`.
- `Url` (`string`): Destination HTTPS URL.
- `Events` (`IReadOnlyList<string>`): Subscribed event patterns.
- `Secret` (`string?`): HMAC secret (returned upon creation).
- `Status` (`string`): `active`, `disabled`.
- `CreatedAt` (`DateTimeOffset`): Creation timestamp.

### [`WebhookEventRecord`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Webhooks/WebhookEndpoint.cs)
- `Id` (`string`): Starts with `evt_`.
- `Type` (`string`): Event topic code (e.g. `collection.succeeded`).
- `Data` (`JsonElement`): Raw event payload dictionary.
- `CreatedAt` (`DateTimeOffset`): Timestamp when event occurred.
