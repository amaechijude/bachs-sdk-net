using System.Text.Json;
using System.Text.Json.Serialization;
using Bachs.Serialization;

namespace Bachs.WebHooks;

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
    /// Origin account ID when the event came from a connected account (Connect events).
    /// </summary>
    [JsonPropertyName("account")]
    public string? Account { get; init; }

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
    public T? GetData<T>()
    {
        if (Data.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
            return default;

        try
        {
            return Data.Deserialize<T>(options: BachsJsonSerializer.Options);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    // BachsJsonSerializer.Deserialize<T>(Data.GetRawText());

    /// <summary>
    /// Converts this raw webhook event into a strongly-typed <see cref="BachsTypedEvent"/>
    /// for pattern matching and type-safe data handling.
    /// </summary>
    /// <returns>The parsed strongly-typed event record.</returns>
    public BachsTypedEvent ToTypedEvent() => BachsTypedEventParser.Parse(this);
}
