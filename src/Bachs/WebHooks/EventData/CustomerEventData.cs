using System.Text.Json.Serialization;
using Bachs.Models.Common;

namespace Bachs.WebHooks.EventData;

/// <summary>
/// Event data for customer webhook events (<c>customer.created</c> and <c>customer.updated</c>).
/// </summary>
public sealed record CustomerEventData
{
    private readonly string? _id;
    private readonly string? _customerId;

    /// <summary>
    /// Unique identifier for the customer (starts with 'cust_').
    /// </summary>
    [JsonPropertyName("id")]
    public string Id
    {
        get => !string.IsNullOrEmpty(_id) ? _id : _customerId ?? string.Empty;
        init => _id = value;
    }

    /// <summary>
    /// Customer identifier in webhook events.
    /// </summary>
    [JsonPropertyName("customer_id")]
    public string CustomerId
    {
        get => !string.IsNullOrEmpty(_customerId) ? _customerId : _id ?? string.Empty;
        init => _customerId = value;
    }

    /// <summary>
    /// Customer email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Customer full name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Customer phone number in E.164 format.
    /// </summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Customer billing address.
    /// </summary>
    [JsonPropertyName("billing_address")]
    public Address? BillingAddress { get; init; }

    /// <summary>
    /// Custom key-value pairs stored with this customer.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// When the customer record was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// When the customer was last updated.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }
}
