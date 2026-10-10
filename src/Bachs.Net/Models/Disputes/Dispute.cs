using System.Text.Json.Serialization;
using Bachs.Net.Common;
using Bachs.Net.Serialization;

namespace Bachs.Net.Models.Disputes;

/// <summary>
/// Represents a payment dispute / chargeback in Bachs.
/// </summary>
public sealed record Dispute
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonIgnore]
    public string DisputeId => Id;

    [JsonPropertyName("charge_id")]
    public string ChargeId { get; init; } = string.Empty;

    [JsonPropertyName("amount")]
    public Price Amount { get; init; }

    [JsonPropertyName("currency")]
    public string Currency { get; init; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    [JsonPropertyName("evidence_due_date")]
    public DateTimeOffset? EvidenceDueDate { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }
}

public sealed record UpdateDisputeEvidenceRequest
{
    [JsonPropertyName("customer_name")]
    public string? CustomerName { get; init; }

    [JsonPropertyName("customer_email")]
    public string? CustomerEmail { get; init; }

    [JsonPropertyName("explanation")]
    public string? Explanation { get; init; }

    [JsonPropertyName("document_ids")]
    public IList<string>? DocumentIds { get; init; }
}

public sealed class ListDisputesOptions : ListRequestOptions
{
    public string? Status { get; set; }
    public string? ChargeId { get; set; }

    public override IDictionary<string, string> ToQueryParameters()
    {
        var parameters = base.ToQueryParameters();
        if (!string.IsNullOrWhiteSpace(Status)) parameters["status"] = Status;
        if (!string.IsNullOrWhiteSpace(ChargeId)) parameters["charge_id"] = ChargeId;
        return parameters;
    }
}
