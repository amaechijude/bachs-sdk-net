using System.Text.Json.Serialization;
using Bachs.Models.Common;

namespace Bachs.WebHooks.EventData;

/// <summary>
/// Event data for refund webhook events (<c>refund.created</c>, <c>refund.paid</c>, and <c>refund.failed</c>).
/// </summary>
public sealed record RefundEventData
{
    private readonly string? _id;
    private readonly string? _refundId;

    /// <summary>
    /// Unique identifier for the refund (starts with 'ref_').
    /// </summary>
    [JsonPropertyName("id")]
    public string Id
    {
        get => !string.IsNullOrEmpty(_id) ? _id : _refundId ?? string.Empty;
        init => _id = value;
    }

    /// <summary>
    /// Refund identifier in webhook events.
    /// </summary>
    [JsonPropertyName("refund_id")]
    public string RefundId
    {
        get => !string.IsNullOrEmpty(_refundId) ? _refundId : _id ?? string.Empty;
        init => _refundId = value;
    }

    /// <summary>
    /// ID of the charge/payment being refunded.
    /// </summary>
    [JsonPropertyName("charge_id")]
    public string ChargeId { get; init; } = string.Empty;

    /// <summary>
    /// Unique external reference for this refund.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    /// Status of the refund (e.g. 'processing', 'paid', 'failed').
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// The refund amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public BachsPrice? Amount { get; init; }

    /// <summary>
    /// Requested amount to be refunded.
    /// </summary>
    [JsonPropertyName("requested_amount")]
    public BachsPrice? RequestedAmount { get; init; }

    /// <summary>
    /// Amount successfully refunded.
    /// </summary>
    [JsonPropertyName("refunded_amount")]
    public BachsPrice? RefundedAmount { get; init; }

    /// <summary>
    /// Fee charged for processing the refund.
    /// </summary>
    [JsonPropertyName("refund_fee_amount")]
    public BachsPrice? RefundFeeAmount { get; init; }

    /// <summary>
    /// Who bears the refund fee ('merchant' or 'customer').
    /// </summary>
    [JsonPropertyName("fee_bearer")]
    public string? FeeBearer { get; init; }

    /// <summary>
    /// Currency code (e.g., Currency.USD).
    /// </summary>
    [JsonPropertyName("currency")]
    public BachsCurrency? Currency { get; init; }

    /// <summary>
    /// Reason why the refund was requested or issued.
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    /// <summary>
    /// When the refund was requested or created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }
}
