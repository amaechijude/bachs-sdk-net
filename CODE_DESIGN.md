# Bachs.Net — Code Design

This document explains the **design patterns** used in the Bachs.Net SDK and gives
contributors a **reading map** for navigating the code. For a component-by-component
inventory (options tables, client lists, test counts), see
[CODE_ANALYSIS.md](./CODE_ANALYSIS.md). For API usage, see [README.md](./README.md).

---

## 1. How to read this codebase

### 1.1 Mental model

Bachs.Net is a thin, strongly-typed wrapper around the Bachs REST API. Almost every
public method follows the same one-line shape:

```csharp
// A domain client method (e.g. CheckoutSessionsClient)
public Task<CreateCheckoutSessionResponse> CreateAsync(
    CreateCheckoutSessionRequest request,
    RequestOptions? requestOptions = null,
    CancellationToken cancellationToken = default) =>
    BachsClient.PostAsync<CreateCheckoutSessionRequest, CreateCheckoutSessionResponse>(
        "v1/checkout-sessions", request, requestOptions, cancellationToken);
```

Everything interesting happens inside `BachsClient.SendAsync` (retries, headers,
deserialization, error mapping). If you understand that one method, you understand
80% of the library.

### 1.2 Suggested reading order

| Step | File | What you'll learn |
|---|---|---|
| 1 | `README.md` | Public surface and intended usage |
| 2 | `src/Bachs.Net/Clients/IBachsClient.cs` | The HTTP verb contract every client shares |
| 3 | `src/Bachs.Net/Clients/BachsClient.cs` | Request pipeline: URI building, headers, retry loop, streaming deserialization, error mapping |
| 4 | `src/Bachs.Net/Clients/BaseClient.cs` + one domain client (e.g. `CheckoutSessionsClient.cs`) | How sub-clients delegate to the root client |
| 5 | `src/Bachs.Net/Common/BachsClientOptions.cs` | Configuration + environment (sandbox/live) inference |
| 6 | `src/Bachs.Net/Serialization/BachsJsonSerializer.cs` + `PriceJsonConverter.cs` | Wire format conventions (snake_case, `Price` as string) |
| 7 | `src/Bachs.Net/WebHooks/BachsWebhook.cs` | Signature verification (V1/V2), anti-replay |
| 8 | `src/Bachs.Net/Extensions/BachsServiceExtensions.cs` | DI registration story |
| 9 | `test/Bachs.Test/BachsClientTests.cs` | How tests intercept HTTP without a network |

### 1.3 Following a request end-to-end

```
caller
  → <Domain>Client.Method(...)                 // Clients/<Domain>Client.cs
    → IBachsClient.GetAsync / PostAsync / ...  // Clients/BachsClient.cs
      → BuildRequestUri(path)                  // BaseUrl (from options) + path
      → SendAsync<TResponse>()                 // retry loop
          • ApplyRequestHeaders (Bearer, Idempotency-Key, X-Account-Id, custom)
          • retry on 429 / 5xx / HttpRequestException (exponential backoff + jitter)
          • success → stream-deserialize with BachsJsonSerializer.Options
          • failure → BachsApiException.Create(status, error, ...)
  → BachsResponse<T> / T                       // returned to caller
```

---

## 2. Architecture at a glance

```
src/Bachs.Net/
├── Clients/        Facade (BachsClient) + 16 thin domain clients + interfaces
├── Common/         Options, exceptions, response/pagination wrappers, query builder
├── Models/         sealed record DTOs per domain (request/response/event payloads)
├── Serialization/  Shared JsonSerializerOptions + PriceJsonConverter
├── WebHooks/       Signature verification, raw events, typed event hierarchy + parser
└── Extensions/     ASP.NET Core DI registration (AddBachsClient)
```

The layering rule is strict and one-directional:

```
Extensions → Clients → (Common, Serialization) → Models
WebHooks   → (Common, Serialization) → Models
```

Domain clients never call each other; they only talk to `IBachsClient`.
Models have no dependencies on clients. This keeps every layer independently
testable and prevents cyclic coupling.

---

## 3. Design patterns

### 3.1 Facade — `BachsClient`

`BachsClient` is the single entry point. It owns the `HttpClient`, the options,
and exposes 16 sub-clients as properties (`client.Products`, `client.Webhooks`, …).

**Why:** consumers get one object to configure, dispose, and inject; the SDK gets
one place to centralize headers, retries, and error handling.

**Where:** `Clients/BachsClient.cs`.

### 3.2 Thin sub-clients over a shared contract (Strategy + Interface Segregation)

Every domain client is a ~10-line class that implements an interface and delegates
straight back to the root client:

```csharp
public sealed class CheckoutSessionsClient(IBachsClient bachsClient)
    : BaseClient(bachsClient), ICheckoutSessionsClient { ... }
```

**Why:**
- Each interface (`ICheckoutSessionsClient`, …) can be injected on its own —
  consumers don't need the whole facade.
- The `IBachsClient` contract makes sub-clients trivially mockable.
- Adding an endpoint touches only its own domain file (see §4).

**Where:** `Clients/*.cs`, `Clients/BaseClient.cs` (primary constructor holding the
`IBachsClient` reference).

### 3.3 Options pattern + environment inference

`BachsClientOptions` centralizes configuration. `GetEffectiveBaseUrl()` resolves the
base URL in priority order: explicit `BaseUrl` → key-prefix inference
(`sk_live_` → production, `sk_sandbox_` → sandbox) → sandbox fallback (fail-safe).

**Why:** safe defaults for beginners, escape hatches (proxies, mocks) for advanced
users, and no surprising production calls from a misconfigured sandbox key.

**Where:** `Common/BachsClientOptions.cs`.

### 3.4 Two construction modes (DI vs. standalone)

```csharp
// Standalone: shared static HttpClient (no socket exhaustion)
var client = new BachsClient("sk_sandbox_...");

// DI: HttpClient from IHttpClientFactory + IOptions<BachsClientOptions>
services.AddBachsClient("sk_sandbox_...");
```

The `[ActivatorUtilitiesConstructor]` attribute tells DI which constructor to use.

**Why:** a static shared `HttpClient` is the recommended pattern for standalone apps;
`IHttpClientFactory` is the recommended pattern inside ASP.NET Core. The class
supports both without forcing a dependency on `Microsoft.Extensions.*` for
standalone users.

**Where:** `Clients/BachsClient.cs` constructors, `Extensions/BachsServiceExtensions.cs`.

### 3.5 Retry-with-clone (buffer + clone, not mutate)

`HttpRequestMessage` can only be sent once. When `MaxRetries > 0`, the pipeline:

1. buffers the request body into `byte[]` **once**,
2. clones the request per attempt (`CloneRequest`),
3. applies exponential backoff with jitter, honoring `Retry-After` and `MaxRetryDelay`,
4. retries on 429, 5xx, and `HttpRequestException`.

**Why:** correct retry semantics without request-object reuse bugs; jitter avoids
thundering-herd retries across many clients.

**Note:** with DI + Polly (`AddStandardResilienceHandler()`), leave `MaxRetries = 0`
(the default) so resilience is owned by exactly one layer.

**Where:** `BachsClient.SendAsync` and helpers.

### 3.6 Factory method — typed exceptions

`BachsApiException.Create(statusCode, error, rateLimit, rawResponse)` is a switch
expression that maps HTTP status codes to the most specific exception subtype:

```
BachsException
├── BachsApiException
│   ├── BachsValidationException   (400/422)
│   ├── BachsAuthenticationException (401)
│   ├── BachsForbiddenException    (403)
│   ├── BachsNotFoundException     (404)
│   ├── BachsConflictException     (409)
│   └── BachsRateLimitException    (429)
├── BachsInvalidOperationException    (local config errors)
```

**Why:** callers can `catch (BachsNotFoundException)` for precise handling, or
`catch (BachsException)` for anything API-related.

**Where:** `Common/BachsApiException.cs` and sibling exception files.

### 3.7 Custom `JsonConverter` — `Price`

The Bachs API sends monetary values as 2-decimal strings (`"29.00"`). `Price` is a
`readonly record struct` that:

- writes as `"29.00"` (`ToApiString()`),
- reads from both string **and** number JSON tokens,
- supports implicit conversions to/from `decimal`,
- exposes `FromMinorUnits(long)` for cent-based inputs.

**Why:** one type makes the wire convention invisible to consumers and eliminates
float/decimal precision bugs at the boundary.

**Where:** `Serialization/PriceJsonConverter.cs`, registered globally in
`Serialization/BachsJsonSerializer.cs` (alongside snake_case naming,
`WhenWritingNull`, case-insensitive matching, and `JsonStringEnumConverter`).

### 3.8 Fluent builder with generic self-typing

Checkout session construction uses a builder hierarchy with the curiously recurring
template pattern (CRTP):

```csharp
public abstract class CheckoutSessionRequestBuilder<TBuilder>
    where TBuilder : CheckoutSessionRequestBuilder<TBuilder>
```

`CreateCheckoutSessionRequestBuilder` is the static entry point that routes to
`ProductCartCheckoutRequestBuilder` (catalog cart) or `RawPricingCheckoutRequestBuilder`
(ad-hoc pricing). Builders validate along the way and produce a
`CreateCheckoutSessionRequest` via `Build()`.

**Why:** checkout requests have many optional fields and two mutually exclusive
shapes; the builder makes valid states easy and invalid states hard to construct.

**Where:** `Models/Checkout/CreateCheckoutSessionRequestBuilder.cs`.

### 3.9 Dual webhook validators (static helper + DI-friendly instance)

- `BachsWebhook` — static helper: `VerifySignature(payload, header, secret, …)`,
  event construction/validation.
- `WebhookEventValidator` — instance class implementing `IWebhookEventValidator` /
  `IWebhookValidator`, able to pull a default secret from `IOptions<BachsClientOptions>`.

**Why:** the static form is convenient in minimal APIs / functions; the instance form
fits DI and testability. Both share the same core verification logic.

**Where:** `WebHooks/BachsWebhook.cs`, `WebHooks/WebhookEventValidator.cs`.

### 3.10 Constant-time comparison + anti-replay

Signatures use HMAC-SHA256 over `"<timestamp>.<payload>"`, compared with
`CryptographicOperations.FixedTimeEquals`. Deliveries older than `toleranceSeconds`
(default 300) are rejected.

**Why:** timing-safe comparison prevents signature oracle attacks; the timestamp
window prevents replay attacks.

**Where:** `WebHooks/BachsWebhook.cs`.

### 3.11 V2 signature format with secret rotation

V2 headers look like `t=<ts>,v1=<hmac>,v1=<hmac2>`. Verification accepts the
payload if **any** `v1` signature matches — enabling zero-downtime webhook secret
rotation (both old and new secrets work during the transition). V1 headers are a raw
HMAC hex digest plus a separate timestamp header.

**Where:** `WebHooks/BachsWebhook.cs` (`VerifyV2Signature` / `VerifyV1Signature`).

### 3.12 Typed event hierarchy + parser (union via records + switch)

Raw events (`BachsWebhookEvent`) carry a `JsonElement` payload and a generic
`GetData<T>()`. `BachsTypedEventParser.Parse` is a switch expression that maps
`type` strings to one of 25+ sealed records (`CheckoutCompletedEvent`,
`CollectionSucceededEvent`, …), each deriving from `BachsTypedEvent<TData>`. Unknown
types fall back to `UnknownBachsEvent`.

**Why:** consumers get compile-time safety and C# pattern matching:

```csharp
switch (typedEvent)
{
    case CheckoutCompletedEvent e: /* e.Data is GetCheckoutSessionResponse */ break;
    case CollectionSucceededEvent e: /* e.Data is Payment */ break;
    case UnknownBachsEvent u: /* forward compatibility */ break;
}
```

**Where:** `WebHooks/BachsTypedEvent.cs`, `WebHooks/BachsEventTypes.cs`.

### 3.13 Records everywhere (immutability + value equality)

Request/response models are `sealed record` / `record struct` types. Immutability
makes DTOs safe to share across async boundaries; value equality simplifies tests.

### 3.14 Response envelope + cursor pagination

- `BachsResponse<T>` wraps `Data`, `StatusCode`, `Headers`, `RateLimit`, `RawJson` —
  so callers can inspect rate-limit headers without exceptions.
- `BachsList<T>` + `BachsPagination` model the API's cursor pagination
  (`Items`, `HasMore`, `NextCursor`, `PrevCursor`); `ListRequestOptions`
  (`Limit`, `Cursor`, `Offset`) is the input.

**Where:** `Common/BachsResponse.cs`, `Common/BachsList.cs`, `Common/BachsPagination.cs`,
`Common/ListRequestOptions.cs`, `Common/RateLimitInfo.cs`.

### 3.15 Small focused helpers

- `QueryBuilder.BuildUrl(path, parameters)` — URL-encodes query strings.
- `Uri.EscapeDataString(...)` — used inline for path parameters in every client
  (never string-concatenate user input into URLs).

**Where:** `Common/QueryBuilder.cs`; path-escaping appears in each domain client.

---

## 4. Contributor recipes

### 4.1 Add a new API endpoint

1. **Models** — add request/response `sealed record`s under
   `Models/<Domain>/` (follow snake_case property naming; use `Price` for money).
2. **Interface** — add the method signature to `Clients/I<Domain>Client.cs`.
3. **Client** — implement it in `Clients/<Domain>Client.cs`, one delegation line
   to `BachsClient.{Get,Post,Patch,Put,Delete}Async`, with `Uri.EscapeDataString`
   on any path parameter and `QueryBuilder.BuildUrl` for query parameters.
4. **Tests** — add cases in `test/Bachs.Test/` using the `MockHttpMessageHandler`
   pattern (see `BachsClientTests.cs`).

No changes to `BachsClient` itself are needed — that's the point of the design.

### 4.2 Add a new webhook event type

1. Add the type string constant to `WebHooks/BachsEventTypes.cs`.
2. Add a `sealed record ... : BachsTypedEvent<TData>` to `WebHooks/BachsTypedEvent.cs`.
3. Add a `case` to the switch in `BachsTypedEventParser.Parse` (copy an existing
   case; remember the `CreateUnknown(evt)` fallback when data fails to deserialize).

### 4.3 Add a new exception subtype

Subclass `BachsApiException` in `Common/` and extend the `Create` switch expression
with the relevant status code(s).

### 4.4 Testing conventions

- xUnit, `[Fact]` / `[Theory]`.
- HTTP is never hit for real: tests construct a `MockHttpMessageHandler`
  (captures the outgoing `HttpRequestMessage`, returns a canned
  `HttpResponseMessage`) and pass it via `new BachsClient(httpClient, options)`.
- Webhook tests build real HMAC signatures so verification logic is exercised
  end-to-end.

---

## 5. Conventions checklist

- **JSON:** snake_case via `BachsJsonSerializer.Options` (and explicit
  `[JsonPropertyName("snake_case")]` attributes on model properties);
  `null` properties omitted; enums as strings; money as `Price`.
- **Async:** every I/O method is `*Async` and takes a `CancellationToken`
  (defaulted) — always `ConfigureAwait(false)` inside the library.
- **Validation:** `ArgumentNullException.ThrowIfNull` / `ArgumentException.ThrowIfNullOrWhiteSpace`
  guards at public boundaries; `BachsClientOptions.Validate()` on construction.
- **Errors:** throw typed exceptions when `ThrowOnApiErrors` is true (default);
  otherwise return `BachsResponse<T>` with the error status.
- **Style:** file-scoped namespaces, primary constructors, top-level XML docs on
  public members (the library ships with IntelliSense documentation).
- **Retry ownership:** exactly one layer retries — built-in (`MaxRetries > 0`) **or**
  Polly/DI resilience, never both.

---

## 6. Quick file index

| If you want to… | Read… |
|---|---|
| understand request flow | `Clients/BachsClient.cs` (`SendAsync`) |
| add an endpoint | `Clients/I<Domain>Client.cs`, `Clients/<Domain>Client.cs`, `Models/<Domain>/` |
| change headers / auth | `BachsClient.ApplyRequestHeaders` |
| change retry behavior | `BachsClient.SendAsync` + `Common/BachsClientOptions.MaxRetries` |
| change error mapping | `Common/BachsApiException.Create` |
| change JSON conventions | `Serialization/BachsJsonSerializer.cs`, `Serialization/PriceJsonConverter.cs` |
| work on webhooks | `WebHooks/BachsWebhook.cs`, `WebHooks/BachsTypedEvent.cs` |
| change DI registration | `Extensions/BachsServiceExtensions.cs` |
| write a test | `test/Bachs.Test/BachsClientTests.cs` (mock handler pattern) |
