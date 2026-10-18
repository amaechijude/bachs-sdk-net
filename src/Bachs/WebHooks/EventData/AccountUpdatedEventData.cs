using System.Text.Json.Serialization;

namespace Bachs.WebHooks.EventData;

/// <summary>
/// Payload data for 'account.updated' webhook event.
/// </summary>
public sealed record AccountUpdatedEventData
{
    [JsonPropertyName("account")]
    public string Account { get; init; } = string.Empty;

    [JsonPropertyName("outstanding")]
    public IReadOnlyList<string> Outstanding { get; init; } = Array.Empty<string>();
}
