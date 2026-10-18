using System.Text.Json.Serialization;
using Bachs.Models.Common;

namespace Bachs.WebHooks.EventData;

/// <summary>
/// Payload data for 'transfer.created' webhook event.
/// </summary>
public sealed record TransferEventData
{
    private readonly string? _id;
    private readonly string? _transferId;

    [JsonPropertyName("id")]
    public string Id
    {
        get => !string.IsNullOrEmpty(_id) ? _id : _transferId ?? string.Empty;
        init => _id = value;
    }

    [JsonPropertyName("transfer_id")]
    public string TransferId
    {
        get => !string.IsNullOrEmpty(_transferId) ? _transferId : _id ?? string.Empty;
        init => _transferId = value;
    }

    [JsonPropertyName("source")]
    public string? Source { get; init; }

    [JsonPropertyName("destination")]
    public string Destination { get; init; } = string.Empty;

    [JsonPropertyName("amount")]
    public BachsPrice Amount { get; init; }

    [JsonPropertyName("currency")]
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("kind")]
    public string? Kind { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }
}
