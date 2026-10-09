using System.Text.Json;
using System.Text.Json.Serialization;
using Bachs.Net.Common;

namespace Bachs.Net.Models.Payouts;

/// <summary>
/// Represents a payout / withdrawal in Bachs.
/// </summary>
public sealed record Payout
{
    /// <summary>
    /// Unique identifier for the payout (withdrawal ID).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Alias for <see cref="Id"/>.
    /// </summary>
    [JsonIgnore]
    public string WithdrawalId => Id;

    /// <summary>
    /// The payout amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }

    /// <summary>
    /// The currency of the payout.
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; init; } = string.Empty;

    /// <summary>
    /// ID of the destination bank account, mobile money wallet, or crypto wallet.
    /// </summary>
    [JsonPropertyName("destination_id")]
    public string DestinationId { get; init; } = string.Empty;

    /// <summary>
    /// Status of the payout ('paid', 'processing', 'failed').
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// External reference for this payout.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    /// When the payout was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }
}

/// <summary>
/// Parameters for creating a payout / withdrawal.
/// </summary>
public sealed class CreatePayoutRequest
{
    /// <summary>
    /// The payout destination ID.
    /// </summary>
    [JsonPropertyName("destination_id")]
    public string DestinationId { get; set; } = string.Empty;

    /// <summary>
    /// The amount to pay out.
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// The currency of the payout.
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// Unique reference for this payout.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    /// <summary>
    /// Optional quote ID if this payout involves currency conversion.
    /// </summary>
    [JsonPropertyName("quote_id")]
    public string? QuoteId { get; set; }
}

/// <summary>
/// Quote for an international or cross-currency payout.
/// </summary>
public sealed record PayoutQuote
{
    [JsonPropertyName("quote_id")]
    public string QuoteId { get; init; } = string.Empty;

    [JsonPropertyName("source_currency")]
    public string SourceCurrency { get; init; } = string.Empty;

    [JsonPropertyName("source_amount")]
    public decimal SourceAmount { get; init; }

    [JsonPropertyName("destination_currency")]
    public string DestinationCurrency { get; init; } = string.Empty;

    [JsonPropertyName("destination_amount")]
    public decimal DestinationAmount { get; init; }

    [JsonPropertyName("rate")]
    public decimal Rate { get; init; }

    [JsonPropertyName("expires_at")]
    public DateTimeOffset ExpiresAt { get; init; }
}

/// <summary>
/// Parameters for requesting a payout quote.
/// </summary>
public sealed class CreatePayoutQuoteRequest
{
    [JsonPropertyName("destination_id")]
    public string DestinationId { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;
}

/// <summary>
/// Represents a saved destination (bank account, mobile money, or crypto wallet).
/// </summary>
public sealed record PayoutDestination
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonIgnore]
    public string DestinationId => Id;

    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("currency")]
    public string Currency { get; init; } = string.Empty;

    [JsonPropertyName("is_usable")]
    public bool IsUsable { get; init; }

    [JsonPropertyName("details")]
    public JsonElement? Details { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }
}

/// <summary>
/// Parameters for creating a payout destination.
/// </summary>
public sealed class CreatePayoutDestinationRequest
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    [JsonPropertyName("details")]
    public IDictionary<string, object?> Details { get; set; } = new Dictionary<string, object?>();
}

/// <summary>
/// Parameters for updating a payout destination.
/// </summary>
public sealed class UpdatePayoutDestinationRequest
{
    [JsonPropertyName("details")]
    public IDictionary<string, object?> Details { get; set; } = new Dictionary<string, object?>();
}

/// <summary>
/// Options for listing payouts.
/// </summary>
public sealed class ListPayoutsOptions : ListRequestOptions
{
    public string? DestinationId { get; set; }
    public string? Status { get; set; }

    public override IDictionary<string, string> ToQueryParameters()
    {
        var parameters = base.ToQueryParameters();
        if (!string.IsNullOrWhiteSpace(DestinationId)) parameters["destination_id"] = DestinationId;
        if (!string.IsNullOrWhiteSpace(Status)) parameters["status"] = Status;
        return parameters;
    }
}
