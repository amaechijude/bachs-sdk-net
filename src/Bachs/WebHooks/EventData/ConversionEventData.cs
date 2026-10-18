using System.Text.Json.Serialization;
using Bachs.Models.Common;

namespace Bachs.WebHooks.EventData;

/// <summary>
/// Event data for currency conversion webhook events (<c>conversion.completed</c> and <c>conversion.failed</c>).
/// </summary>
public sealed record ConversionEventData
{
    private readonly string? _id;
    private readonly string? _conversionId;

    /// <summary>
    /// Unique identifier for the conversion (starts with 'cnv_').
    /// </summary>
    [JsonPropertyName("id")]
    public string Id
    {
        get => !string.IsNullOrEmpty(_id) ? _id : _conversionId ?? string.Empty;
        init => _id = value;
    }

    /// <summary>
    /// Conversion identifier in webhook events.
    /// </summary>
    [JsonPropertyName("conversion_id")]
    public string ConversionId
    {
        get => !string.IsNullOrEmpty(_conversionId) ? _conversionId : _id ?? string.Empty;
        init => _conversionId = value;
    }

    /// <summary>
    /// ID of the quote used for this conversion (starts with 'cq_').
    /// </summary>
    [JsonPropertyName("quote_id")]
    public string? QuoteId { get; init; }

    /// <summary>
    /// Source currency converted from.
    /// </summary>
    [JsonPropertyName("from_currency")]
    public BachsCurrency FromCurrency { get; init; } = BachsCurrency.USD;

    /// <summary>
    /// Target currency converted to.
    /// </summary>
    [JsonPropertyName("to_currency")]
    public BachsCurrency ToCurrency { get; init; } = BachsCurrency.USD;

    /// <summary>
    /// Source amount debited.
    /// </summary>
    [JsonPropertyName("from_amount")]
    public BachsPrice FromAmount { get; init; }

    /// <summary>
    /// Destination amount credited.
    /// </summary>
    [JsonPropertyName("to_amount")]
    public BachsPrice ToAmount { get; init; }

    /// <summary>
    /// Conversion exchange rate.
    /// </summary>
    [JsonPropertyName("exchange_rate")]
    public decimal ExchangeRate { get; init; }

    /// <summary>
    /// Status of the conversion ('completed', 'failed').
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// When the conversion was initiated/created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }
}
