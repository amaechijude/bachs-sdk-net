# Currency Conversions Client

The Conversions client provides real-time foreign exchange (FX) quotes and balance conversion execution between supported currencies within your Bachs multi-currency account.

- **Interface**: [`IConversionsClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/IConversionsClient.cs)
- **Implementation**: [`ConversionsClient`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Clients/ConversionsClient.cs)
- **Property**: `client.Conversions`
- **DI Service**: `services.AddBachsClient(...)` registers `IConversionsClient` as a transient service.

---

## Methods Overview

| Method | HTTP | Path | Description |
|---|---|---|---|
| [`CreateQuoteAsync`](#1-request-a-conversion-quote) | `POST` | `/v1/conversions/quotes` | Requests a guaranteed, time-locked FX conversion rate quote. |
| [`ExecuteAsync`](#2-execute-a-conversion) | `POST` | `/v1/conversions` | Executes a currency conversion using a valid quote ID. |
| [`GetAsync`](#3-retrieve-a-conversion) | `GET` | `/v1/conversions/{conversionId}` | Retrieves a conversion transaction by its ID. |
| [`ListAsync`](#4-list-conversions) | `GET` | `/v1/conversions` | Lists historical balance conversions with pagination and status filters. |

---

## Usage Examples

### 1. Request a Conversion Quote

Request a locked exchange rate before committing to a currency swap:

```csharp
using Bachs.Net.Clients;
using Bachs.Net.Models.Common;
using Bachs.Net.Models.Conversions;

var quote = await client.Conversions.CreateQuoteAsync(new CreateConversionQuoteRequest
{
    SourceCurrency = Currency.USD, // Defaults to Currency.USD
    TargetCurrency = Currency.NGN, // Defaults to Currency.USD
    Amount = 1000.00m // Converting $1,000 USD to NGN
});

Console.WriteLine($"Quote ID: {quote.QuoteId}");
Console.WriteLine($"Rate: 1 {quote.SourceCurrency} = {quote.Rate} {quote.TargetCurrency}");
Console.WriteLine($"You will receive: {quote.TargetAmount:N2} {quote.TargetCurrency}");
Console.WriteLine($"Rate guaranteed until: {quote.ExpiresAt:T}");
```

---

### 2. Execute a Conversion

Submit the `QuoteId` before the quote expires to execute the ledger balance swap:

```csharp
using Bachs.Net.Common;

var conversion = await client.Conversions.ExecuteAsync(new ExecuteConversionRequest
{
    QuoteId = quote.QuoteId
}, new RequestOptions
{
    IdempotencyKey = $"conv_exec_{quote.QuoteId}"
});

Console.WriteLine($"Conversion ID: {conversion.Id}");
Console.WriteLine($"Status: {conversion.Status}"); // 'completed', 'pending'
Console.WriteLine($"Converted: {conversion.SourceAmount} {conversion.SourceCurrency} -> {conversion.TargetAmount} {conversion.TargetCurrency}");
```

---

### 3. Retrieve a Conversion

```csharp
var record = await client.Conversions.GetAsync("conv_100200300");

Console.WriteLine($"Conversion {record.Id} status: {record.Status} at {record.CreatedAt}");
```

---

### 4. List Conversions

```csharp
var history = await client.Conversions.ListAsync(new ListConversionsOptions
{
    Status = "completed",
    Limit = 20
});

foreach (var item in history.Data)
{
    Console.WriteLine($"{item.CreatedAt:d}: {item.SourceAmount} {item.SourceCurrency} => {item.TargetAmount} {item.TargetCurrency} (Rate: {item.Rate})");
}
```

---

## Models

### [`ConversionQuote`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Conversions/Conversion.cs)
- `QuoteId` (`string`): Unique quote identifier to pass to `ExecuteAsync`.
- `SourceCurrency` (`Currency`): Currency debited.
- `SourceAmount` (`Price`): Debit amount.
- `TargetCurrency` (`Currency`): Currency credited.
- `TargetAmount` (`Price`): Credit amount.
- `Rate` (`decimal`): Conversion multiplier.
- `ExpiresAt` (`DateTimeOffset`): Expiration timestamp of the guaranteed rate.

### [`Conversion`](https://github.com/amaechijude/Bachs.Net/blob/main/src/Bachs.Net/Models/Conversions/Conversion.cs)
- `Id` (`string`): Starts with `conv_`.
- `QuoteId` (`string`): Quote utilized for the conversion.
- `SourceCurrency` (`Currency`): Origin currency code.
- `SourceAmount` (`Price`): Debited amount.
- `TargetCurrency` (`Currency`): Destination currency code.
- `TargetAmount` (`Price`): Credited amount.
- `Rate` (`decimal`): Effective rate.
- `Status` (`string`): `completed`, `pending`, `failed`.
- `CreatedAt` (`DateTimeOffset`): Timestamp of execution.
