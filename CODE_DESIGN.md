# Bachs.Net — Code Design & Contributor Reading Guide

This document explains the **design patterns** used in the Bachs.Net SDK and gives
contributors a **reading map** for finding their way around the code.

- For the public API surface and usage examples, see [README.md](./README.md).
- For per-client endpoint references, see [docs/README.md](./docs/README.md).

If you only read one section, read [§1 How to read this codebase](#1-how-to-read-this-codebase)
and [§2 The request pipeline](#2-the-request-pipeline-the-one-thing-to-understand).

---

## 1. How to read this codebase

### 1.1 The mental model

Bachs.Net is a thin, strongly-typed wrapper around the Bachs REST API. Almost every
public method is a one- or two-line delegation to the root client:

```csharp
// Clients/CheckoutSessionsClient.cs
public Task<BachsResponse<CreateCheckoutSessionResponse>> CreateAsync(
    CreateCheckoutSessionRequest request,
    BachsRequestOptions? requestOptions = null,
    CancellationToken cancellationToken = default
) =>
    BachsClient.PostAsync<CreateCheckoutSessionRequest, CreateCheckoutSessionResponse>(
        "v1/checkout-sessions", request, requestOptions, cancellationToken);
```

There is no business logic in the domain clients. Everything interesting happens in
`BachsClient.SendAsync` (URI building, headers, serialization, response/error
mapping) plus the resilience pipeline registered by `AddBachsClient`. **If you
understand those two pieces, you understand the whole library.**

### 1.2 Suggested reading order

| Step | File | What you'll learn |
|---|---|---|
| 1 | `README.md` | Public surface and intended usage |
| 2 | `src/Bachs.Net/Clients/IBachsClient.cs` | The HTTP verb contract and the 16 resource properties every client shares |
| 3 | `src/Bachs.Net/Clients/BachsClient.cs` | `SendAsync`: URI building, header application, content handling, success/failure mapping |
| 4 | `src/Bachs.Net/Clients/BaseClient.cs` + one domain client (e.g. `CheckoutSessionsClient.cs`) | How sub-clients delegate to the root client |
| 5 | `src/Bachs.Net/Common/BachsResponse.cs` + `BachsError.cs` + `BachsErrorCode.cs` | The result type returned to callers and the error contract |
| 6 | `src/Bachs.Net/Common/BachsClientOptions.cs` | Configuration, validation, environment inference |
| 7 | `src/Bachs.Net/Extensions/BachsServiceExtensions.cs` | Where retries / circuit breaker / timeouts actually live |
| 8 | `src/Bachs.Net/Serialization/BachsJsonSerializer.cs` + `BachsPriceJsonConverter.cs` | Wire-format conventions |
| 9 | `src/Bachs.Net/WebHooks/BachsWebhook.cs` | Signature verification (V1/V2), anti-replay |
| 10 | `test/Bachs.Test/BachsClientTests.cs` | How tests intercept HTTP without a network |

### 1.3 Following a request end-to-end

```
caller
  → <Domain>Client.Method(...)                      // Clients/<Domain>Client.cs
    → IBachsClient.GetAsync / PostAsync / ...       // Clients/BachsClient.cs
      → BuildRequestUri(path)                       // Options.GetEffectiveBaseUrl() + path
      → ApplyRequestHeaders(...)                    // Bearer, Idempotency-Key, X-Account-Id, custom
      → HttpClient.SendAsync(...)                   // transport
          • standalone: a shared static HttpClient (no retry/circuit breaker)
          • DI (AddBachsClient): Polly standard resilience pipeline
               – total & per-attempt timeouts
               – retries with exponential backoff + jitter (429/408/5xx/network)
               – circuit breaker              → BrokenCircuitException on open
               – attempt timeout              → TimeoutRejectedException
      → map outcome
          • 2xx  → BachsResponse<T>.Success(data, status)
          • 4xx/5xx → BachsResponse<T>.Failure(BachsError, status)
          • transport failure → throw BachsException subtype
  → BachsResponse<T>                                 // returned to caller
```

---

## 2. The request pipeline (the one thing to understand)

### 2.1 Result pattern instead of throw-on-API-error

Every public verb returns `BachsResponse<T>`, not `T`:

```csharp
public sealed record BachsResponse<T>
{
    public T Data { get; }              // parsed payload (default! on failure)
    public BachsError? Error { get; }   // parsed error body on failure
    public HttpStatusCode StatusCode { get; }
    public bool IsSuccessStatusCode => (int)StatusCode is >= 200 and <= 299;

    public static BachsResponse<T> Success<TResponse>(T response, HttpStatusCode status);
    public static BachsResponse<T> Failure(BachsError? errorBody, HttpStatusCode status);
}
```

`Success`/`Failure` are the only ways to construct a response (the constructors are
private), so the "either data or error" invariant holds by construction.

**Why:** callers can inspect status, headers, and error details without wrapping every
call in `try/catch`. API errors are *data*; only genuine transport/programming
failures throw.

**Where:** `Common/BachsResponse.cs`, populated in `BachsClient.SendAsync`.

### 2.2 What throws vs. what returns

| Situation | Behavior |
|---|---|
| 2xx response | `BachsResponse<T>.Success` |
| 4xx / 5xx response | `BachsResponse<T>.Failure` (an `Error` is attached) |
| JSON encode/decode error | throws `BachsSerializationException` |
| Per-attempt timeout (DI pipeline) | throws `BachsTimeoutException` |
| Circuit open (DI pipeline) | throws `BachsServiceUnavailableException` |
| `HttpRequestException` | throws `BachsNetworkException` |
| Bad configuration | throws `BachsInvalidOperationException` (`Options.Validate()`) |
| Bad argument | throws `ArgumentNullException` / `ArgumentException` / `BachsArgumentNullException` |

Note that retries and the circuit breaker are **only active when the client is
registered through `AddBachsClient`** (see §3.6). A standalone `new BachsClient(...)`
uses a plain shared `HttpClient`.

### 2.3 Header application

`ApplyRequestHeaders` is the single place headers are set, in priority order:

1. `Authorization: Bearer <key>` — a per-request `ScopedApiKey` overrides the option.
2. `Idempotency-Key` — only when supplied in `BachsRequestOptions`.
3. `X-Account-Id` — per-request value overrides `Options.AccountId` (Connect).
4. Arbitrary `CustomHeaders`.

`Accept: application/json` and a `Bachs.Net/<version>` `User-Agent` are applied once
when the `HttpClient` is configured.

**Where:** `BachsClient.ApplyRequestHeaders`, `BachsClient.ConfigureHttpClientHeaders`.

---

## 3. Design patterns

### 3.1 Facade — `BachsClient`

`BachsClient` is the single entry point. It owns the `HttpClient` and exposes the 16
domain clients as properties (`client.Products`, `client.Webhooks`, …).

**Why:** consumers get one object to configure, dispose, and inject; the SDK gets one
place to centralize headers, serialization, and transport handling.

**Where:** `Clients/BachsClient.cs` (and the interface `Clients/IBachsClient.cs`).

### 3.2 Thin sub-clients over a shared contract (Interface Segregation + Delegation)

Each domain client is a small `sealed` class built on a primary constructor, deriving
from `BaseClient`, implementing its own interface, and delegating straight back to the
root client:

```csharp
public sealed class CheckoutSessionsClient(IBachsClient bachsClient)
    : BaseClient(bachsClient), ICheckoutSessionsClient { ... }

public abstract class BaseClient(IBachsClient bachsClient)
{
    protected readonly IBachsClient BachsClient =
        bachsClient ?? throw new ArgumentNullException(nameof(bachsClient));
}
```

**Why:**

- Each interface (`ICheckoutSessionsClient`, …) can be injected on its own — consumers
  don't need the whole facade.
- Depending on `IBachsClient` (not the concrete class) makes sub-clients trivially
  mockable and keeps them decoupled from each other.
- Adding an endpoint touches only its own domain file (see §5.1).

**Where:** `Clients/*Client.cs`, `Clients/BaseClient.cs`.

### 3.3 Options pattern + environment inference

`BachsClientOptions` centralizes configuration. The environment is resolved lazily:

```
GetEffectiveEnvironment():  explicit Environment
                         → Custom if BaseUrl is set
                         → Live if key starts with "sk_live_"
                         → Sandbox otherwise (fail-safe fallback)

GetEffectiveBaseUrl():      explicit BaseUrl wins
                         → otherwise the standard URL for the resolved environment
```

`Validate()` enforces the invariants (key present, absolute `BaseUrl` when custom,
key prefix matches the selected environment, timeouts ordered, retry values sane,
circuit-breaker values sane).

**Why:** safe defaults for beginners (an unrecognized key can never hit production),
escape hatches for advanced users (proxies, mock servers, tests via `BaseUrl`), and
fail-fast configuration errors at construction time.

**Where:** `Common/BachsClientOptions.cs`.

### 3.4 Two construction modes (DI vs. standalone)

```csharp
// Standalone: shared static HttpClient (no socket exhaustion)
var client = new BachsClient("sk_sandbox_...", "whsec_...");
var client = new BachsClient(new BachsClientOptions { ApiKey = "sk_sandbox_..." });

// DI: HttpClient from IHttpClientFactory + IOptions<BachsClientOptions>
services.AddBachsClient("sk_sandbox_...", "whsec_...");
```

The `[ActivatorUtilitiesConstructor]` attribute tells DI which constructor to use.

**Why:** a single static `HttpClient` is the right pattern for long-lived standalone
apps; `IHttpClientFactory` (with a resilience handler) is the right pattern inside
ASP.NET Core. The class supports both without forcing `Microsoft.Extensions.*` on
standalone users. `Dispose()` is deliberately a no-op: the client never owns the
`HttpClient` it was given or the shared static one.

**Where:** `Clients/BachsClient.cs` constructors, `Extensions/BachsServiceExtensions.cs`.

### 3.5 JSON conventions and custom converters

`BachsJsonSerializer.Options` is the single shared `JsonSerializerOptions`:

- `SnakeCaseLower` property naming (models also carry explicit
  `[JsonPropertyName("snake_case")]` where the field name is irregular),
- `WhenWritingNull` (omit nulls),
- case-insensitive deserialization,
- `AllowReadingFromString` (numbers may arrive as strings),
- converters: `BachsErrorCodeJsonConverter`, `JsonStringEnumConverter`, `BachsPriceJsonConverter`.

**Where:** `Serialization/BachsJsonSerializer.cs`.

#### `BachsPrice` — a money type with a custom converter

The API sends money as 2-decimal strings (`"29.00"`). `BachsPrice` is a
`readonly record struct` that:

- writes as `"29.00"` (`ToApiString()`, invariant culture, rounded 2 dp),
- reads from both string **and** number tokens,
- has implicit conversions to/from `decimal`,
- exposes `FromMinorUnits(long)` for cent inputs.

**Why:** one type makes the wire convention invisible and eliminates float/decimal
precision bugs at the boundary.

**Where:** `Models/Common/BachsPrice.cs`, `Serialization/BachsPriceJsonConverter.cs`.

#### `BachsErrorCode` — enum with a forward-compatible converter

Error codes are modelled as an enum (`BachsErrorCode`), but the converter maps any
unrecognized code to `BachsErrorCode.UNKNOWN` instead of throwing.

**Why:** new server error codes never break deserialization of an otherwise valid
error response.

**Where:** `Common/BachsErrorCode.cs`, used by `Common/BachsError.cs`.

### 3.6 Resilience as a decorator pipeline (not hand-rolled retry)

When registered with `AddBachsClient`, the typed `HttpClient` is wrapped with
`AddStandardResilienceHandler()` and configured from `BachsClientOptions`:

```
TotalRequestTimeout ─┐
AttemptTimeout ──────┤  timeouts
MaxRetries/RetryDelay/MaxRetryDelay ── retries (exponential + jitter)
EnableCircuitBreaker/CircuitBreaker* ─ circuit breaker
```

`ShouldRetry` decides transience: network errors and `TimeoutRejectedException`, or
HTTP `408 / 429 / 500 / 502 / 503 / 504`, and it refuses to wait past `MaxRetryDelay`
when the server sends a `Retry-After`.

**Why:** retries, backoff, jitter, and circuit breaking are subtle; delegating to the
Polly-based standard handler avoids re-implementing them and keeps the SDK's own code
focused on serialization and mapping. "Retries disabled" is expressed through
`ShouldRetry` returning false rather than `MaxRetryAttempts = 0`, because Polly
requires a minimum of one attempt.

**Where:** `Extensions/BachsServiceExtensions.cs`, tuned by `Common/BachsClientOptions.cs`.

### 3.7 Adapter — translating Polly failures into SDK exceptions

Polly throws its own types; `BachsClient.SendAsync` adapts them at the boundary so
callers only ever see `Bachs.Net.Exceptions` types:

| Thrown by pipeline | Surfaced as |
|---|---|
| `TimeoutRejectedException` | `BachsTimeoutException` |
| `BrokenCircuitException` | `BachsServiceUnavailableException` |
| `HttpRequestException` | `BachsNetworkException` |
| `JsonException` | `BachsSerializationException` |

**Where:** `Clients/BachsClient.cs` (`SendAsync` catch blocks).

### 3.8 Factory method — typed exceptions

`BachsApiException.Create(statusCode, error, rateLimit, rawResponse)` is a `switch`
expression that maps a status code + `BachsError` to the most specific subtype:

```
BachsException
├── BachsApiException
│   ├── BachsValidationException     (400 with VALIDATION_ERROR/errors, or 422)
│   ├── BachsAuthenticationException (401)
│   ├── BachsForbiddenException      (403)
│   ├── BachsNotFoundException       (404)
│   ├── BachsConflictException       (409)
│   └── BachsRateLimitException      (429)
├── BachsInvalidOperationException   (local configuration errors)
├── BachsArgumentNullException
├── BachsTimeoutException            (transport)
├── BachsNetworkException            (transport)
├── BachsServiceUnavailableException (transport / circuit open)
└── BachsSerializationException      (wire format)
```

`BachsValidationException` additionally exposes `ValidationErrors` (field-level
details). This factory is the single place mapping is defined, so exception-style
consumers and the response-style pipeline stay consistent.

**Where:** `Exceptions/BachsApiException.cs`, `Exceptions/*.cs`.

### 3.9 Fluent builder with generic self-typing (CRTP)

Checkout session construction uses a builder hierarchy with the curiously recurring
template pattern:

```csharp
public abstract class CheckoutSessionRequestBuilder<TBuilder>
    where TBuilder : CheckoutSessionRequestBuilder<TBuilder>
{
    protected TBuilder Self => (TBuilder)this;
    public TBuilder WithCustomer(...) { ...; return Self; }
    public abstract CreateCheckoutSessionRequest Build();
}
```

`CreateCheckoutSessionRequestBuilder` is the static entry point that routes to either
`ProductCartCheckoutRequestBuilder` (catalog cart) or `RawPricingCheckoutRequestBuilder`
(ad-hoc pricing). Builders accumulate state and validate on `Build()`.

**Why:** checkout requests have many optional fields and two mutually exclusive
shapes; `return Self` keeps IntelliSense fluent across the hierarchy, and the static
entry point makes the invalid combinations hard to construct.

**Where:** `Models/Checkout/CheckoutSessionRequestBuilder.cs`,
`Models/Checkout/CreateCheckoutSessionRequestBuilder.cs`, and the two concrete builders.

### 3.10 Response envelope + cursor pagination

- `BachsList<T>` wraps `Items` + `BachsPagination` and exposes convenience members
  (`HasMore`, `NextCursor`, `PrevCursor`, `Total`, `Count`).
- `BachsListRequestOptions : BachsRequestOptions` adds `Limit`, `Cursor`, `Offset`
  and implements `ToQueryParameters()`. Each domain defines a specific subtype (e.g.
  `ListProductsOptions`) that overrides `ToQueryParameters()` to add its own filters.
- List methods build the URL with `BachsQueryBuilder.BuildUrl(path, options?.ToQueryParameters())`.

**Why:** pagination cursor/offset logic lives in one place; domain-specific filters
are expressed by overriding one virtual method.

**Where:** `Common/BachsList.cs`, `Common/BachsPagination.cs`,
`Common/BachsListRequestOptions.cs`, `Common/BachsQueryBuilder.cs`.

### 3.11 Records everywhere (immutability + value equality)

Request/response models are `sealed record` / `record struct` types. Immutability
makes DTOs safe to share across async boundaries; value equality simplifies tests and
makes `BachsResponse<T>` a clean value carrier.

### 3.12 Dual webhook API (static helper + DI-friendly instance)

- `BachsWebhook` — static helper: `VerifySignature`, `ConstructEvent`, `ValidateEvent`,
  `ValidateTypedEvent`, and non-throwing `TryValidate*` variants.
- `WebhookEventValidator` — instance class implementing `IWebhookEventValidator` and
  alias `IWebhookValidator`; can pull a default secret from `IOptions<BachsClientOptions>`
  and resolves the secret per call.

**Why:** the static form is convenient in minimal APIs / cloud functions; the instance
form fits DI and testability. Both share the same core verification logic.

**Where:** `WebHooks/BachsWebhook.cs`, `WebHooks/WebhookEventValidator.cs`,
`WebHooks/IWebhookEventValidator.cs`.

### 3.13 Constant-time comparison + anti-replay

Signatures are HMAC-SHA256 over `"<timestamp>.<payload>"`, compared with
`CryptographicOperations.FixedTimeEquals`. Deliveries older than
`DefaultToleranceSeconds` (300) are rejected.

**Why:** timing-safe comparison prevents signature-oracle attacks; the timestamp
window prevents replay attacks.

**Where:** `WebHooks/BachsWebhook.cs`.

### 3.14 V2 signature format with secret rotation

V2 headers look like `t=<ts>,v1=<hmac>,v1=<hmac2>`. Verification accepts the payload
if **any** `v1` signature matches — enabling zero-downtime webhook secret rotation
(both old and new secrets work during the transition). V1 headers are a raw HMAC hex
digest plus a separate `X-Bachs-Timestamp` header.

**Where:** `WebHooks/BachsWebhook.cs` (`VerifyV2Signature` / `VerifyV1Signature`).

### 3.15 Typed event hierarchy + parser (union via records + switch)

Raw events (`BachsWebhookEvent`) carry a `JsonElement` payload and a generic
`GetData<T>()` / `ToTypedEvent()`. `BachsTypedEventParser.Parse` is a switch
expression that maps `type` strings to one of ~25 sealed records
(`CheckoutCompletedEvent`, `CollectionSucceededEvent`, …), each deriving from
`BachsTypedEvent<TData>`. Unknown types — or known types whose data fails to
deserialize — fall back to `UnknownBachsEvent`, which retains the raw `JsonElement`.

**Why:** consumers get compile-time safety and C# pattern matching, plus forward
compatibility:

```csharp
switch (typedEvent)
{
    case CheckoutCompletedEvent e: /* e.Data is GetCheckoutSessionResponse */ break;
    case CollectionSucceededEvent e: /* e.Data is Payment */ break;
    case UnknownBachsEvent u: /* u.GetData<T>() for new server event types */ break;
}
```

**Where:** `WebHooks/BachsTypedEvent.cs`, `WebHooks/BachsEventTypes.cs`,
`WebHooks/BachsWebhookEvent.cs`.

### 3.16 Small focused helpers

- `BachsQueryBuilder.BuildUrl(path, parameters)` — URL-encodes query strings.
- `Uri.EscapeDataString(...)` — used inline for path parameters in every client
  (never string-concatenate user input into URLs).

**Where:** `Common/BachsQueryBuilder.cs`; path-escaping appears in each domain client.

---

## 4. Layering & dependencies

```
Extensions ──▶ Clients ──▶ Common ──▶ Exceptions
                  │            │
                  ├──▶ Serialization ◀──▶ Models
                  └──▶ Models
WebHooks ──▶ Common, Serialization, Exceptions, Models
```

Rules that keep the code decoupled:

- Domain clients never call each other; they only talk to `IBachsClient`.
- Models have no dependency on clients or on `HttpClient`.
- The exception layer is a leaf that only `Common`, `Clients`, and `Extensions`
  depend on.
- `Serialization` and `Models` are coupled only through the `[JsonConverter]`
  attribute on `BachsPrice` (Models → Serialization) and the converter's use of the
  `BachsPrice` type (Serialization → Models); both live in the same assembly.
- Resilience depends on Polly types, but only in `Extensions` (DI) and in the small
  catch blocks in `BachsClient` — no Polly types leak into the public surface.

```
src/Bachs.Net/
├── Clients/        Facade (BachsClient) + 16 thin domain clients + interfaces + BaseClient
├── Common/         Options, response/error/list wrappers, query builder, error codes
├── Exceptions/     Typed exception hierarchy
├── Models/         sealed record DTOs per domain (request/response/event payloads)
├── Serialization/  Shared JsonSerializerOptions + BachsPrice / error-code converters
├── WebHooks/       Signature verification, raw + typed events + parser
└── Extensions/     ASP.NET Core DI registration (AddBachsClient)
```

---

## 5. Contributor recipes

### 5.1 Add a new API endpoint

1. **Models** — add request/response `sealed record`s under `Models/<Domain>/`
   (follow snake_case naming; use `BachsPrice` for money).
2. **Interface** — add the method signature to `Clients/I<Domain>Client.cs`.
3. **Client** — implement it in `Clients/<Domain>Client.cs`, one delegation line to
   `BachsClient.{Get,Post,Patch,Put,Delete}Async`, with `Uri.EscapeDataString` on any
   path parameter and `BachsQueryBuilder.BuildUrl` for query parameters. Return
   `Task<BachsResponse<T>>`.
4. **Tests** — add cases in `test/Bachs.Test/` using the `MockHttpMessageHandler`
   pattern (see §5.6).

No changes to `BachsClient` are needed — that's the point of the design.

### 5.2 Add a new list endpoint with filters

Derive an options class from `BachsListRequestOptions`, override
`ToQueryParameters()` to add filters, then pass
`BachsQueryBuilder.BuildUrl(path, options?.ToQueryParameters())` as the path. List
methods conventionally accept `BachsRequestOptions? requestOptions = null` and pass
`requestOptions ?? options` down, so the same object can carry idempotency/account
context.

### 5.3 Add a new webhook event type

1. Add the type string constant to `WebHooks/BachsEventTypes.cs`.
2. Add a `sealed record ... : BachsTypedEvent<TData>` to `WebHooks/BachsTypedEvent.cs`.
3. Add a `case` to the `switch` in `BachsTypedEventParser.Parse` (copy an existing
   case; keep the `CreateUnknown(evt)` fallback when data fails to deserialize).

### 5.4 Add a new exception subtype

Subclass `BachsApiException` in `Exceptions/` and extend the `Create` switch
expression with the relevant status code(s). Keep the constructor parameter order
consistent with the existing subtypes.

### 5.5 Change configuration or resilience behavior

- New setting → add a property to `BachsClientOptions`, document its default, and add
  validation for it in `Validate()`.
- Apply it → wire it into `Extensions/BachsServiceExtensions.cs` inside the
  `Configure` callback of the resilience handler.

### 5.6 Testing conventions

- xUnit, `[Fact]` / `[Theory]`; test files live in `test/Bachs.Test/` and mirror the
  area under test (`BachsClientTests`, `BachsClientOptionsTests`,
  `BachsSerializationTests`, `BachsServiceExtensionsTests`, `BachsWebhookTests`,
  `CheckoutSessionTests`).
- HTTP is never hit for real: tests construct a `MockHttpMessageHandler` (captures the
  outgoing `HttpRequestMessage`, returns a canned `HttpResponseMessage`) and pass it
  via `new BachsClient(httpClient, options)`. A `CountingHttpMessageHandler` variant
  returns a queue of status codes to observe request counts.
- Webhook tests build real HMAC signatures with `HMACSHA256` so the verification logic
  is exercised end-to-end.
- DI behavior is tested against a real `ServiceCollection` + `BuildServiceProvider()`
  (see `BachsServiceExtensionsTests`).

---

## 6. Conventions checklist

- **JSON:** snake_case via `BachsJsonSerializer.Options`; `null` properties omitted;
  enums as strings; money as `BachsPrice`; unknown error codes → `UNKNOWN`.
- **Async:** every I/O method is `*Async`, takes a defaulted `CancellationToken`, and
  awaits with `ConfigureAwait(false)` inside the library. Verb wrappers are
  intentionally **not** `async` — they return the `Task` from `SendAsync` directly to
  avoid an extra state-machine allocation per call.
- **Returns:** public methods return `BachsResponse<T>`; API errors are data, not
  exceptions.
- **Validation:** `ArgumentNullException.ThrowIfNull` /
  `ArgumentException.ThrowIfNullOrWhiteSpace` guards at public boundaries;
  `BachsClientOptions.Validate()` on construction.
- **Errors:** typed `Bachs.Net.Exceptions` types only — never leak Polly or
  `System.Text.Json` exceptions.
- **URLs:** always `Uri.EscapeDataString` for path segments and `BachsQueryBuilder`
  for query strings.
- **Style:** file-scoped namespaces, primary constructors, `sealed` records, top-level
  XML docs on public members (the library ships IntelliSense docs;
  `GenerateDocumentationFile=true`).
- **Targets:** multi-targeted `net8.0;net10.0`, nullable enabled.

---

## 7. Quick file index

| If you want to… | Read… |
|---|---|
| understand request flow | `Clients/BachsClient.cs` (`SendAsync`) |
| add an endpoint | `Clients/I<Domain>Client.cs`, `Clients/<Domain>Client.cs`, `Models/<Domain>/` |
| change the return/error contract | `Common/BachsResponse.cs`, `Common/BachsError.cs` |
| change headers / auth | `BachsClient.ApplyRequestHeaders` |
| change retry / timeout / circuit breaker | `Extensions/BachsServiceExtensions.cs` + `Common/BachsClientOptions.cs` |
| change error mapping to exceptions | `Exceptions/BachsApiException.Create` |
| change JSON conventions | `Serialization/BachsJsonSerializer.cs`, `Serialization/BachsPriceJsonConverter.cs` |
| work on webhooks | `WebHooks/BachsWebhook.cs`, `WebHooks/BachsTypedEvent.cs` |
| build a checkout request | `Models/Checkout/CreateCheckoutSessionRequestBuilder.cs` |
| change DI registration | `Extensions/BachsServiceExtensions.cs` |
| write a test | `test/Bachs.Test/BachsClientTests.cs` (mock handler pattern) |
