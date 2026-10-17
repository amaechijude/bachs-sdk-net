# Exception Reference

The Bachs .NET SDK uses a single exception hierarchy rooted at `BachsException`. API errors (4xx/5xx) are **returned** in `BachsResponse<T>` rather than thrown; the exceptions below are thrown only for transport, serialization, configuration, and argument failures, or when you explicitly convert an unsuccessful response with `BachsApiException.Create(...)`.

## Hierarchy

```
BachsException
├── BachsApiException                 (HTTP 4xx/5xx, only via BachsApiException.Create)
│   ├── BachsValidationException      (HTTP 400 with validation errors, or 422)
│   ├── BachsAuthenticationException  (HTTP 401)
│   ├── BachsForbiddenException       (HTTP 403)
│   ├── BachsNotFoundException        (HTTP 404)
│   ├── BachsConflictException        (HTTP 409)
│   ├── BachsRateLimitException       (HTTP 429)
│   └── BachsApiException             (other 4xx/5xx)
├── BachsInvalidOperationException    (invalid configuration or builder usage)
├── BachsArgumentNullException       (null constructor argument)
├── BachsTimeoutException             (per-attempt or total timeout exceeded)
├── BachsNetworkException             (DNS, socket, or connection failure)
├── BachsServiceUnavailableException  (circuit breaker open)
└── BachsSerializationException      (request/response JSON failure)
```

Standard `ArgumentNullException` / `ArgumentException` may also be thrown for `null`, empty, or whitespace method arguments.

## What throws each exception

| Exception | Thrown by | When |
|---|---|---|
| `BachsTimeoutException` | `BachsClient.SendAsync` | The DI resilience pipeline rejects an attempt/total timeout, or a transport-level timeout occurs (`HttpClient.Timeout`). Caller cancellation propagates unchanged. |
| `BachsServiceUnavailableException` | `BachsClient.SendAsync` | The Polly circuit breaker is open (`BrokenCircuitException`). |
| `BachsNetworkException` | `BachsClient.SendAsync` | An `HttpRequestException` occurs during the HTTP call. |
| `BachsSerializationException` | `BachsClient.SendAsync` | Request serialization or response deserialization fails with `JsonException`. |
| `BachsInvalidOperationException` | `BachsClientOptions.Validate`, builders, webhook validator, `WebhooksClient` | Invalid configuration, missing webhook secret, invalid builder state, or an invalid argument such as a missing account id. |
| `BachsArgumentNullException` | `BachsClient` constructors | A required constructor argument (`HttpClient`, `BachsClientOptions`) is `null`. |
| `ArgumentNullException` / `ArgumentException` | Public methods | A required method argument (request body or identifier) is `null`, empty, or whitespace. |
| `BachsApiException` (and subclasses) | `BachsApiException.Create(...)` only | When you explicitly convert an unsuccessful `BachsResponse<T>` into an exception. The SDK does not throw these automatically. |
| `BachsValidationException` | `BachsApiException.Create(...)` | HTTP 400 with `VALIDATION_ERROR` or field-level errors, or HTTP 422. Exposes `ValidationErrors`. |
| `BachsAuthenticationException` | `BachsApiException.Create(...)` | HTTP 401 Unauthorized. |
| `BachsForbiddenException` | `BachsApiException.Create(...)` | HTTP 403 Forbidden. |
| `BachsNotFoundException` | `BachsApiException.Create(...)` | HTTP 404 Not Found. |
| `BachsConflictException` | `BachsApiException.Create(...)` | HTTP 409 Conflict. Exposes `IsIdempotencyInProgress` and `IsIdempotencyConflict`. |
| `BachsRateLimitException` | `BachsApiException.Create(...)` | HTTP 429 Too Many Requests. Exposes `RetryAfter`. |

## Usage

Transport/config failures should be handled with `try/catch` around SDK calls:

```csharp
using Bachs.Exceptions;

try
{
    var response = await client.Products.CreateAsync(request);
    if (!response.IsSuccessStatusCode)
    {
        // API error returned as data; handle or convert:
        if (response.Error is { } apiError)
        {
            throw BachsApiException.Create(response.StatusCode, apiError, rateLimit: null, rawResponse: null);
        }
        return;
    }
}
catch (BachsTimeoutException)
{
    // retry or return 504
}
catch (BachsServiceUnavailableException)
{
    // circuit breaker open
}
catch (BachsNetworkException)
{
    // upstream unreachable
}
catch (BachsSerializationException)
{
    // malformed JSON
}
catch (BachsApiException apiException)
{
    // converted API failure
}
```

## Notes

- API errors (4xx/5xx) are returned in `BachsResponse<T>.Error` and are not thrown unless you call `BachsApiException.Create(...)`.
- Webhook verification methods throw `BachsInvalidOperationException` when no secret is configured or supplied; their `Try*` variants return `false` instead.
- Checkout builders throw `BachsInvalidOperationException` on `Build()` when validation constraints are violated.
- All SDK exceptions derive from `BachsException`, so a single `catch (BachsException)` covers every SDK failure.