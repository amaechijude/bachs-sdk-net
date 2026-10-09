using System.Text.Json.Serialization;
using Bachs.Net.Common;

namespace Bachs.Net.Models.Conversions;

/// <summary>
/// Represents a currency conversion between balances.
/// </summary>
public record Conversion
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonIgnore]
    public string ConversionId => Id;

    [JsonPropertyName("quote_id")]
    public string QuoteId { get; init; } = string.Empty;

    [JsonPropertyName("source_currency")]
    public string SourceCurrency { get; init; } = string.Empty;

    [JsonPropertyName("source_amount")]
    public decimal SourceAmount { get; init; }

    [JsonPropertyName("target_currency")]
    public string TargetCurrency { get; init; } = string.Empty;

    [JsonPropertyName("target_amount")]
    public decimal TargetAmount { get; init; }

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
public record ConversionQuote
{
    [JsonPropertyName("quote_id")]
    public string QuoteId { get; init; } = string.Empty;

    [JsonPropertyName("source_currency")]
    public string SourceCurrency { get; init; } = string.Empty;

    [JsonPropertyName("source_amount")]
    public decimal SourceAmount { get; init; }

    [JsonPropertyName("target_currency")]
    public string TargetCurrency { get; init; } = string.Empty;

    [JsonPropertyName("target_amount")]
    public decimal TargetAmount { get; init; }

    [JsonPropertyName("rate")]
    public decimal Rate { get; init; }

    [JsonPropertyName("expires_at")]
    public DateTimeOffset ExpiresAt { get; init; }
}

public class CreateConversionQuoteRequest
{
    [JsonPropertyName("source_currency")]
    public string SourceCurrency { get; set; } = string.Empty;

    [JsonPropertyName("target_currency")]
    public string TargetCurrency { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }
}

public class ExecuteConversionRequest
{
    [JsonPropertyName("quote_id")]
    public string QuoteId { get; set; } = string.Empty;
}

public class ListConversionsOptions : ListRequestOptions
{
    public string? Status { get; set; }

    public override IDictionary<string, string> ToQueryParameters()
    {
        var parameters = base.ToQueryParameters();
        if (!string.IsNullOrWhiteSpace(Status)) parameters["status"] = Status;
        return parameters;
    }
}
