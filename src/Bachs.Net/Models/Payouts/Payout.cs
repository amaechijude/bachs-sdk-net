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
    private readonly string? _id;
    private readonly string? _withdrawalId;

    /// <summary>
    /// Unique identifier for the payout (withdrawal ID).
    /// </summary>
    [JsonPropertyName("id")]
    public string Id
    {
        get => !string.IsNullOrEmpty(_id) ? _id : _withdrawalId ?? string.Empty;
        init => _id = value;
    }

    /// <summary>
    /// Alias for <see cref="Id"/>, mapped to 'withdrawal_id' in webhook events.
    /// </summary>
    [JsonPropertyName("withdrawal_id")]
    public string WithdrawalId
    {
        get => !string.IsNullOrEmpty(_withdrawalId) ? _withdrawalId : _id ?? string.Empty;
        init => _withdrawalId = value;
    }

    /// <summary>
    /// The payout amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public Price Amount { get; init; }

    /// <summary>
    /// The currency of the payout.
    /// </summary>
    [JsonPropertyName("currency")]
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;

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
    /// Source currency for FX payouts.
    /// </summary>
    [JsonPropertyName("from_currency")]
    public BachsCurrency? FromCurrency { get; init; }

    /// <summary>
    /// Destination currency for FX payouts.
    /// </summary>
    [JsonPropertyName("to_currency")]
    public BachsCurrency? ToCurrency { get; init; }

    /// <summary>
    /// Applied exchange rate.
    /// </summary>
    [JsonPropertyName("exchange_rate")]
    public decimal? ExchangeRate { get; init; }

    /// <summary>
    /// Converted payout amount at destination.
    /// </summary>
    [JsonPropertyName("to_amount")]
    public Price? ToAmount { get; init; }

    /// <summary>
    /// Withdrawal fee deducted.
    /// </summary>
    [JsonPropertyName("withdrawal_fee")]
    public Price? WithdrawalFee { get; init; }

    /// <summary>
    /// Net amount debited from source currency.
    /// </summary>
    [JsonPropertyName("net_from_amount")]
    public Price? NetFromAmount { get; init; }

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
    /// The currency of the payout. Defaults to <see cref="BachsCurrency.USD"/>.
    /// </summary>
    [JsonPropertyName("currency")]
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;

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
    public BachsCurrency SourceCurrency { get; init; } = BachsCurrency.USD;

    [JsonPropertyName("source_amount")]
    public Price SourceAmount { get; init; }

    [JsonPropertyName("destination_currency")]
    public BachsCurrency DestinationCurrency { get; init; } = BachsCurrency.USD;

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
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;
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
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;

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
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;

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
