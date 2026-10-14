using System.Text.Json.Serialization;
using Bachs.Net.Common;
using Bachs.Net.Models.Common;

namespace Bachs.Net.Models.Transfers;

/// <summary>
/// Represents an internal balance transfer between a platform and a connected account.
/// </summary>
public sealed record Transfer
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonIgnore]
    public string TransferId => Id;

    [JsonPropertyName("amount")]
    public BachsPrice Amount { get; init; }

    [JsonPropertyName("currency")]
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;

    [JsonPropertyName("destination")]
    public string Destination { get; init; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }
}

public sealed record CreateTransferRequest
{
    [JsonPropertyName("amount")]
    public BachsPrice Amount { get; init; }

    [JsonPropertyName("currency")]
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;

    [JsonPropertyName("destination")]
    public string Destination { get; init; } = string.Empty;
}

public sealed class ListTransfersOptions : BachsListRequestOptions { }
