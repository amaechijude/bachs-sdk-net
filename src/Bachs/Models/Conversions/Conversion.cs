using System.Text.Json.Serialization;
using Bachs.Common;
using Bachs.Models.Common;

namespace Bachs.Models.Conversions;

/// <summary>
/// Represents a currency conversion between balances.
/// </summary>
public sealed record Conversion
{
    private readonly string? _id;
    private readonly string? _conversionId;

    [JsonPropertyName("id")]
    public string Id
    {
        get => !string.IsNullOrEmpty(_id) ? _id : _conversionId ?? string.Empty;
        init => _id = value;
    }

    [JsonPropertyName("conversion_id")]
    public string ConversionId
    {
        get => !string.IsNullOrEmpty(_conversionId) ? _conversionId : _id ?? string.Empty;
        init => _conversionId = value;
    }

    [JsonPropertyName("quote_id")]
    public string QuoteId { get; init; } = string.Empty;

    [JsonPropertyName("source_currency")]
    public BachsCurrency SourceCurrency { get; init; } = BachsCurrency.USD;

    [JsonPropertyName("source_amount")]
    public BachsPrice SourceAmount { get; init; }

    [JsonPropertyName("target_currency")]
    public BachsCurrency TargetCurrency { get; init; } = BachsCurrency.USD;

    [JsonPropertyName("target_amount")]
    public BachsPrice TargetAmount { get; init; }

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
    public BachsCurrency SourceCurrency { get; init; } = BachsCurrency.USD;

    [JsonPropertyName("source_amount")]
    public BachsPrice SourceAmount { get; init; }

    [JsonPropertyName("target_currency")]
    public BachsCurrency TargetCurrency { get; init; } = BachsCurrency.USD;

    [JsonPropertyName("target_amount")]
    public BachsPrice TargetAmount { get; init; }

    [JsonPropertyName("rate")]
    public decimal Rate { get; init; }

    [JsonPropertyName("expires_at")]
    public DateTimeOffset ExpiresAt { get; init; }
}

public sealed record CreateConversionQuoteRequest
{
    [JsonPropertyName("source_currency")]
    public BachsCurrency SourceCurrency { get; init; } = BachsCurrency.USD;

    [JsonPropertyName("target_currency")]
    public BachsCurrency TargetCurrency { get; init; } = BachsCurrency.USD;

    [JsonPropertyName("amount")]
    public BachsPrice Amount { get; init; }
}

public sealed record ExecuteConversionRequest
{
    [JsonPropertyName("quote_id")]
    public string QuoteId { get; init; } = string.Empty;
}

public sealed class ListConversionsOptions : BachsListRequestOptions
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
