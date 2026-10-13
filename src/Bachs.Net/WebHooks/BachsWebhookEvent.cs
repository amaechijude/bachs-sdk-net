using System.Text.Json;
using System.Text.Json.Serialization;
using Bachs.Net.Serialization;

namespace Bachs.Net.WebHooks;

/// <summary>
/// Represents a webhook event payload sent by Bachs.
/// </summary>
public sealed record BachsWebhookEvent
{
    /// <summary>
    /// Unique identifier for the event (starts with 'evt_').
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Event type (e.g., 'collection.succeeded', 'checkout.completed', 'customer.subscription.created').
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// When the event occurred.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// The organization ID the event belongs to.
    /// </summary>
    [JsonPropertyName("organization_id")]
    public string? OrganizationId { get; init; }

    /// <summary>
    /// Raw JSON element payload for the event data.
    /// </summary>
    [JsonPropertyName("data")]
    public JsonElement Data { get; init; }

    /// <summary>
    /// Deserializes the event data element to a strongly typed object.
    /// </summary>
    /// <typeparam name="T">Target model type.</typeparam>
    /// <returns>Deserialized event data object.</returns>
    public T? GetData<T>() => BachsJsonSerializer.Deserialize<T>(Data.GetRawText());
}
