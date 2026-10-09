using System.Text.Json;
using System.Text.Json.Serialization;
using Bachs.Net.Common;

namespace Bachs.Net.Models.Webhooks;

/// <summary>
/// Represents a configured webhook destination endpoint in Bachs.
/// </summary>
public sealed record WebhookEndpoint
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonIgnore]
    public string EndpointId => Id;

    [JsonPropertyName("url")]
    public string Url { get; init; } = string.Empty;

    [JsonPropertyName("events")]
    public IReadOnlyList<string> Events { get; init; } = Array.Empty<string>();

    [JsonPropertyName("secret")]
    public string? Secret { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }
}

public sealed class CreateWebhookEndpointRequest
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;

    [JsonPropertyName("events")]
    public IList<string> Events { get; set; } = new List<string>();
}

public sealed class UpdateWebhookEndpointRequest
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("events")]
    public IList<string>? Events { get; set; }
}

public sealed record SecretRotationResult
{
    [JsonPropertyName("secret")]
    public string Secret { get; init; } = string.Empty;
}

public sealed record WebhookEventRecord
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("organization_id")]
    public string? OrganizationId { get; init; }

    [JsonPropertyName("data")]
    public JsonElement Data { get; init; }
}

public sealed class ListWebhookEndpointsOptions : ListRequestOptions
{
}
