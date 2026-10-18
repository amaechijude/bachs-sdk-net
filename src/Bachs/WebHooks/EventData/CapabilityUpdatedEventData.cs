using System.Text.Json.Serialization;

namespace Bachs.WebHooks.EventData;

/// <summary>
/// Payload data for 'capability.updated' webhook event.
/// </summary>
public sealed record CapabilityUpdatedEventData
{
    [JsonPropertyName("account")]
    public string Account { get; init; } = string.Empty;

    [JsonPropertyName("capability")]
    public string Capability { get; init; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("requested")]
    public bool Requested { get; init; }
}
