using System.Text.Json.Serialization;
using Bachs.Models.Common;

namespace Bachs.Models.Balances;

/// <summary>
/// Snapshot of balances across currencies held by an account.
/// </summary>
public sealed record Balance
{
    /// <summary>
    /// Balance buckets by currency code (e.g. "USD", "NGN").
    /// </summary>
    [JsonPropertyName("currencies")]
    public IReadOnlyDictionary<string, BalanceBucket> Currencies { get; init; } =
        new Dictionary<string, BalanceBucket>();

    /// <summary>
    /// Consolidated available balance in USD.
    /// </summary>
    [JsonPropertyName("total_usd")]
    public BachsPrice? TotalUsd { get; init; }
}

/// <summary>
/// Balance bucket for an individual currency.
/// </summary>
public sealed record BalanceBucket
{
    /// <summary>
    /// Funds currently available for payouts or conversions.
    /// </summary>
    [JsonPropertyName("available")]
    public BachsPrice Available { get; init; }

    /// <summary>
    /// Funds held in reserve or locked for open disputes.
    /// </summary>
    [JsonPropertyName("locked")]
    public BachsPrice Locked { get; init; }

    /// <summary>
    /// Funds still settling from incoming payments.
    /// </summary>
    [JsonPropertyName("pending")]
    public BachsPrice Pending { get; init; }

    /// <summary>
    /// Currency code (e.g. USD, NGN).
    /// </summary>
    [JsonPropertyName("currency")]
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;
}

/// <summary>
/// Automatic payout schedule settings.
/// </summary>
public sealed record BalanceSettings
{
    [JsonPropertyName("payout_schedule")]
    public string? PayoutSchedule { get; init; }

    [JsonPropertyName("default_destination_id")]
    public string? DefaultDestinationId { get; init; }
}
