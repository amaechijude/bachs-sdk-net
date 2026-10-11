using System.Text.Json.Serialization;
using Bachs.Net.Common;
using Bachs.Net.Models.Common;
using Bachs.Net.Serialization;

namespace Bachs.Net.Models.Refunds;

/// <summary>
/// Represents a refund in Bachs.
/// </summary>
public sealed record Refund
{
    /// <summary>
    /// Unique identifier for the refund (starts with 'ref_').
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Alias for <see cref="Id"/>.
    /// </summary>
    [JsonIgnore]
    public string RefundId => Id;

    /// <summary>
    /// ID of the charge/payment being refunded (starts with 'ch_').
    /// </summary>
    [JsonPropertyName("charge_id")]
    public string ChargeId { get; init; } = string.Empty;

    /// <summary>
    /// The refunded amount as a price.
    /// </summary>
    [JsonPropertyName("amount")]
    public Price Amount { get; init; }

    /// <summary>
    /// Currency code (e.g., Currency.USD).
    /// </summary>
    [JsonPropertyName("currency")]
    public Currency Currency { get; init; } = Currency.USD;

    /// <summary>
    /// Status of the refund ('succeeded', 'processing', 'failed').
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Unique external reference for this refund.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    /// Explanation of why the refund was issued.
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    /// <summary>
    /// When the refund was requested.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }
}

/// <summary>
/// Parameters for creating a refund.
/// </summary>
public sealed record CreateRefundRequest
{
    /// <summary>
    /// ID of the payment to refund (starts with 'ch_').
    /// </summary>
    [JsonPropertyName("charge_id")]
    public string ChargeId { get; init; } = string.Empty;

    /// <summary>
    /// Unique external reference for this refund request.
    /// </summary>
    [JsonPropertyName("reference")]
    public string Reference { get; init; } = string.Empty;

    /// <summary>
    /// Amount to refund. Omit for a full refund, or supply a price for a partial refund.
    /// </summary>
    [JsonPropertyName("amount")]
    public Price? Amount { get; init; }

    /// <summary>
    /// Reason for the refund.
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    /// <summary>
    /// Crypto wallet address to receive the refund (for crypto payments).
    /// </summary>
    [JsonPropertyName("refund_address")]
    public string? RefundAddress { get; init; }
}

/// <summary>
/// Options for listing refunds.
/// </summary>
public sealed class ListRefundsOptions : ListRequestOptions
{
    /// <summary>
    /// Filter by charge ID.
    /// </summary>
    public string? ChargeId { get; set; }

    public override IDictionary<string, string> ToQueryParameters()
    {
        var parameters = base.ToQueryParameters();
        if (!string.IsNullOrWhiteSpace(ChargeId))
        {
            parameters["charge_id"] = ChargeId;
        }
        return parameters;
    }
}
