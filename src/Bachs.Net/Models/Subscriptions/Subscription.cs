using System.Text.Json.Serialization;
using Bachs.Net.Common;

namespace Bachs.Net.Models.Subscriptions;

/// <summary>
/// Represents a recurring subscription in Bachs.
/// </summary>
public sealed record Subscription
{
    /// <summary>
    /// Unique identifier for the subscription (starts with 'sub_').
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Alias for <see cref="Id"/>.
    /// </summary>
    [JsonIgnore]
    public string SubscriptionId => Id;

    /// <summary>
    /// ID of the customer associated with this subscription.
    /// </summary>
    [JsonPropertyName("customer_id")]
    public string CustomerId { get; init; } = string.Empty;

    /// <summary>
    /// ID of the product this subscription is billing for.
    /// </summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; init; } = string.Empty;

    /// <summary>
    /// Status of the subscription ('active', 'trialing', 'past_due', 'canceled').
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Start of the current billing cycle.
    /// </summary>
    [JsonPropertyName("current_period_start")]
    public DateTimeOffset? CurrentPeriodStart { get; init; }

    /// <summary>
    /// End of the current billing cycle.
    /// </summary>
    [JsonPropertyName("current_period_end")]
    public DateTimeOffset? CurrentPeriodEnd { get; init; }

    /// <summary>
    /// Whether the subscription will cancel at the end of the current period.
    /// </summary>
    [JsonPropertyName("cancel_at_period_end")]
    public bool CancelAtPeriodEnd { get; init; }

    /// <summary>
    /// Timestamp when the subscription was canceled, if applicable.
    /// </summary>
    [JsonPropertyName("canceled_at")]
    public DateTimeOffset? CanceledAt { get; init; }

    /// <summary>
    /// Custom key-value pairs stored on the subscription.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// When the subscription was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }
}

/// <summary>
/// Parameters for updating a subscription.
/// </summary>
public sealed class UpdateSubscriptionRequest
{
    /// <summary>
    /// New product ID to change the subscription's plan.
    /// </summary>
    [JsonPropertyName("product_id")]
    public string? ProductId { get; set; }

    /// <summary>
    /// Whether to cancel the subscription at the end of the current billing cycle.
    /// </summary>
    [JsonPropertyName("cancel_at_period_end")]
    public bool? CancelAtPeriodEnd { get; set; }

    /// <summary>
    /// Updated custom key-value pairs.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IDictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Options for listing subscriptions.
/// </summary>
public sealed class ListSubscriptionsOptions : ListRequestOptions
{
    /// <summary>
    /// Filter by customer ID.
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// Filter by product ID.
    /// </summary>
    public string? ProductId { get; set; }

    /// <summary>
    /// Filter by status ('active', 'trialing', 'past_due', 'canceled').
    /// </summary>
    public string? Status { get; set; }

    public override IDictionary<string, string> ToQueryParameters()
    {
        var parameters = base.ToQueryParameters();
        if (!string.IsNullOrWhiteSpace(CustomerId))
        {
            parameters["customer_id"] = CustomerId;
        }
        if (!string.IsNullOrWhiteSpace(ProductId))
        {
            parameters["product_id"] = ProductId;
        }
        if (!string.IsNullOrWhiteSpace(Status))
        {
            parameters["status"] = Status;
        }
        return parameters;
    }
}
