using System.Text.Json;
using System.Text.Json.Serialization;
using Bachs.Net.Common;
using Bachs.Net.Models.Common;
using Bachs.Net.Serialization;

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
    public Price Amount { get; init; }

    /// <summary>
    /// The currency of the payout.
    /// </summary>
    [JsonPropertyName("currency")]
    public Currency Currency { get; init; } = Currency.USD;

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
public sealed record CreatePayoutRequest
{
    /// <summary>
    /// The payout destination ID.
    /// </summary>
    [JsonPropertyName("destination_id")]
    public string DestinationId { get; init; } = string.Empty;

    /// <summary>
    /// The amount to pay out.
    /// </summary>
    [JsonPropertyName("amount")]
    public Price Amount { get; init; }

    /// <summary>
    /// The currency of the payout. Defaults to <see cref="Currency.USD"/>.
    /// </summary>
    [JsonPropertyName("currency")]
    public Currency Currency { get; init; } = Currency.USD;

    /// <summary>
    /// Unique reference for this payout.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    /// Optional quote ID if this payout involves currency conversion.
    /// </summary>
    [JsonPropertyName("quote_id")]
    public string? QuoteId { get; init; }
}

/// <summary>
/// Quote for an international or cross-currency payout.
/// </summary>
public sealed record PayoutQuote
{
    [JsonPropertyName("quote_id")]
    public string QuoteId { get; init; } = string.Empty;

    [JsonPropertyName("source_currency")]
    public Currency SourceCurrency { get; init; } = Currency.USD;

    [JsonPropertyName("source_amount")]
    public Price SourceAmount { get; init; }

    [JsonPropertyName("destination_currency")]
    public Currency DestinationCurrency { get; init; } = Currency.USD;

    [JsonPropertyName("destination_amount")]
    public Price DestinationAmount { get; init; }

    [JsonPropertyName("rate")]
    public decimal Rate { get; init; }

    [JsonPropertyName("expires_at")]
    public DateTimeOffset ExpiresAt { get; init; }
}

/// <summary>
/// Parameters for requesting a payout quote.
/// </summary>
public sealed record CreatePayoutQuoteRequest
{
    [JsonPropertyName("destination_id")]
    public string DestinationId { get; init; } = string.Empty;

    [JsonPropertyName("amount")]
    public Price Amount { get; init; }

    [JsonPropertyName("currency")]
    public Currency Currency { get; init; } = Currency.USD;
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
    public Currency Currency { get; init; } = Currency.USD;

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
public sealed record CreatePayoutDestinationRequest
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("currency")]
    public Currency Currency { get; init; } = Currency.USD;

    [JsonPropertyName("details")]
    public IDictionary<string, object?> Details { get; init; } = new Dictionary<string, object?>();
}

/// <summary>
/// Parameters for updating a payout destination.
/// </summary>
public sealed record UpdatePayoutDestinationRequest
{
    [JsonPropertyName("details")]
    public IDictionary<string, object?> Details { get; init; } = new Dictionary<string, object?>();
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
        if (!string.IsNullOrWhiteSpace(DestinationId))
            parameters["destination_id"] = DestinationId;
        if (!string.IsNullOrWhiteSpace(Status))
            parameters["status"] = Status;
        return parameters;
    }
}
