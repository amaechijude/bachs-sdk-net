using System.Text.Json.Serialization;
using Bachs.Models.Common;

namespace Bachs.WebHooks.EventData;

/// <summary>
/// Event data for payout webhook events (<c>payout.created</c>, <c>payout.paid</c>, and <c>payout.failed</c>).
/// </summary>
public sealed record PayoutEventData
{
    private readonly string? _id;
    private readonly string? _withdrawalId;

    /// <summary>
    /// Unique identifier for the payout/withdrawal (starts with 'pay_').
    /// </summary>
    [JsonPropertyName("id")]
    public string Id
    {
        get => !string.IsNullOrEmpty(_id) ? _id : _withdrawalId ?? string.Empty;
        init => _id = value;
    }

    /// <summary>
    /// Withdrawal identifier in webhook events.
    /// </summary>
    [JsonPropertyName("withdrawal_id")]
    public string WithdrawalId
    {
        get => !string.IsNullOrEmpty(_withdrawalId) ? _withdrawalId : _id ?? string.Empty;
        init => _withdrawalId = value;
    }

    /// <summary>
    /// External reference for the payout.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    /// Status of the payout ('pending', 'completed', 'failed').
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Payout amount in base currency.
    /// </summary>
    [JsonPropertyName("amount")]
    public BachsPrice Amount { get; init; }

    /// <summary>
    /// Currency of the payout.
    /// </summary>
    [JsonPropertyName("currency")]
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;

    /// <summary>
    /// Source currency for FX conversions.
    /// </summary>
    [JsonPropertyName("from_currency")]
    public BachsCurrency? FromCurrency { get; init; }

    /// <summary>
    /// Target destination currency for FX payouts.
    /// </summary>
    [JsonPropertyName("to_currency")]
    public BachsCurrency? ToCurrency { get; init; }

    /// <summary>
    /// Applied FX exchange rate.
    /// </summary>
    [JsonPropertyName("exchange_rate")]
    public decimal? ExchangeRate { get; init; }

    /// <summary>
    /// Target amount paid in destination currency.
    /// </summary>
    [JsonPropertyName("to_amount")]
    public BachsPrice? ToAmount { get; init; }

    /// <summary>
    /// Withdrawal fee deducted.
    /// </summary>
    [JsonPropertyName("withdrawal_fee")]
    public BachsPrice? WithdrawalFee { get; init; }

    /// <summary>
    /// Net debit amount from source currency.
    /// </summary>
    [JsonPropertyName("net_from_amount")]
    public BachsPrice? NetFromAmount { get; init; }

    /// <summary>
    /// Destination ID if provided.
    /// </summary>
    [JsonPropertyName("destination_id")]
    public string? DestinationId { get; init; }

    /// <summary>
    /// Custom key-value pairs stored on the payout.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// When the payout was initiated.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }
}
