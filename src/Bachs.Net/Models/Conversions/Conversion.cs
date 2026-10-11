using System.Text.Json.Serialization;
using Bachs.Net.Common;
using Bachs.Net.Models.Common;
using Bachs.Net.Serialization;

namespace Bachs.Net.Models.Conversions;

/// <summary>
/// Represents a currency conversion between balances.
/// </summary>
public sealed record Conversion
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonIgnore]
    public string ConversionId => Id;

    [JsonPropertyName("quote_id")]
    public string QuoteId { get; init; } = string.Empty;

    [JsonPropertyName("source_currency")]
    public Currency SourceCurrency { get; init; } = Currency.USD;

    [JsonPropertyName("source_amount")]
    public Price SourceAmount { get; init; }

    [JsonPropertyName("target_currency")]
    public Currency TargetCurrency { get; init; } = Currency.USD;

    [JsonPropertyName("target_amount")]
    public Price TargetAmount { get; init; }

    [JsonPropertyName("rate")]
    public decimal Rate { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }
}

/// <summary>
/// Locked rate quote for a conversion.
/// </summary>
public sealed record ConversionQuote
{
    [JsonPropertyName("quote_id")]
    public string QuoteId { get; init; } = string.Empty;

    [JsonPropertyName("source_currency")]
    public Currency SourceCurrency { get; init; } = Currency.USD;

    [JsonPropertyName("source_amount")]
    public Price SourceAmount { get; init; }

    [JsonPropertyName("target_currency")]
    public Currency TargetCurrency { get; init; } = Currency.USD;

    [JsonPropertyName("target_amount")]
    public Price TargetAmount { get; init; }

    [JsonPropertyName("rate")]
    public decimal Rate { get; init; }

    [JsonPropertyName("expires_at")]
    public DateTimeOffset ExpiresAt { get; init; }
}

public sealed record CreateConversionQuoteRequest
{
    [JsonPropertyName("source_currency")]
    public Currency SourceCurrency { get; init; } = Currency.USD;

    [JsonPropertyName("target_currency")]
    public Currency TargetCurrency { get; init; } = Currency.USD;

    [JsonPropertyName("amount")]
    public Price Amount { get; init; }
}

public sealed record ExecuteConversionRequest
{
    [JsonPropertyName("quote_id")]
    public string QuoteId { get; init; } = string.Empty;
}

public sealed class ListConversionsOptions : ListRequestOptions
{
    public string? Status { get; set; }

    public override IDictionary<string, string> ToQueryParameters()
    {
        var parameters = base.ToQueryParameters();
        if (!string.IsNullOrWhiteSpace(Status))
            parameters["status"] = Status;
        return parameters;
    }
}
