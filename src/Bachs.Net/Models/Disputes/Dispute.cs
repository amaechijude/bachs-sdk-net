using System.Text.Json.Serialization;
using Bachs.Net.Common;
using Bachs.Net.Models.Common;
using Bachs.Net.Serialization;

namespace Bachs.Net.Models.Disputes;

/// <summary>
/// Represents a payment dispute / chargeback in Bachs.
/// </summary>
public sealed record Dispute
{
    private readonly string? _id;
    private readonly string? _disputeId;

    [JsonPropertyName("id")]
    public string Id
    {
        get => !string.IsNullOrEmpty(_id) ? _id : _disputeId ?? string.Empty;
        init => _id = value;
    }

    [JsonPropertyName("dispute_id")]
    public string DisputeId
    {
        get => !string.IsNullOrEmpty(_disputeId) ? _disputeId : _id ?? string.Empty;
        init => _disputeId = value;
    }

    [JsonPropertyName("charge_id")]
    public string ChargeId { get; init; } = string.Empty;

    [JsonPropertyName("amount")]
    public Price Amount { get; init; }

    [JsonPropertyName("currency")]
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;

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

    [JsonPropertyName("service_documentation")]
    public string? ServiceDocumentation { get; init; }

    [JsonPropertyName("shipping_documentation")]
    public string? ShippingDocumentation { get; init; }

    [JsonPropertyName("uncategorized_text")]
    public string? UncategorizedText { get; init; }
}

public sealed record DisputeEvidenceResponse
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;
}

public sealed class ListDisputesOptions : ListRequestOptions
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
