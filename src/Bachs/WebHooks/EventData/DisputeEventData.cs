using System.Text.Json.Serialization;
using Bachs.Models.Common;

namespace Bachs.WebHooks.EventData;

/// <summary>
/// Event data for dispute webhook events (<c>dispute.created</c> and <c>dispute.updated</c>).
/// </summary>
public sealed record DisputeEventData
{
    private readonly string? _id;
    private readonly string? _disputeId;

    /// <summary>
    /// Unique identifier for the dispute (starts with 'dsp_').
    /// </summary>
    [JsonPropertyName("id")]
    public string Id
    {
        get => !string.IsNullOrEmpty(_id) ? _id : _disputeId ?? string.Empty;
        init => _id = value;
    }

    /// <summary>
    /// Dispute identifier in webhook events.
    /// </summary>
    [JsonPropertyName("dispute_id")]
    public string DisputeId
    {
        get => !string.IsNullOrEmpty(_disputeId) ? _disputeId : _id ?? string.Empty;
        init => _disputeId = value;
    }

    /// <summary>
    /// Associated charge ID being disputed.
    /// </summary>
    [JsonPropertyName("charge_id")]
    public string ChargeId { get; init; } = string.Empty;

    /// <summary>
    /// Disputed amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public BachsPrice Amount { get; init; }

    /// <summary>
    /// Disputed amount currency code.
    /// </summary>
    [JsonPropertyName("currency")]
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;

    /// <summary>
    /// Lifecycle status of the dispute ('needs_response', 'under_review', 'won', 'lost').
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Whether evidence response is currently editable.
    /// </summary>
    [JsonPropertyName("is_response_editable")]
    public bool IsResponseEditable { get; init; }

    /// <summary>
    /// Reason code for the dispute (e.g. 'fraudulent').
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    /// <summary>
    /// Deadline timestamp to submit response and evidence.
    /// </summary>
    [JsonPropertyName("response_deadline_at")]
    public DateTimeOffset? ResponseDeadlineAt { get; init; }

    /// <summary>
    /// Charge amount originally collected.
    /// </summary>
    [JsonPropertyName("charge_amount")]
    public BachsPrice? ChargeAmount { get; init; }

    /// <summary>
    /// Original charge currency code.
    /// </summary>
    [JsonPropertyName("charge_currency")]
    public BachsCurrency? ChargeCurrency { get; init; }

    /// <summary>
    /// When the dispute was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// When the dispute was last updated.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }
}
