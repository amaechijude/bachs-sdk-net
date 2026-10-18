using System.Text.Json.Serialization;
using Bachs.Models.Common;

namespace Bachs.WebHooks.EventData;

/// <summary>
/// Event data for subscription webhook events (<c>customer.subscription.created</c>, <c>customer.subscription.updated</c>, and <c>customer.subscription.deleted</c>).
/// </summary>
public sealed record SubscriptionEventData
{
    private readonly string? _id;
    private readonly string? _subscriptionId;

    /// <summary>
    /// Unique identifier for the subscription (starts with 'sub_').
    /// </summary>
    [JsonPropertyName("id")]
    public string Id
    {
        get => !string.IsNullOrEmpty(_id) ? _id : _subscriptionId ?? string.Empty;
        init => _id = value;
    }

    /// <summary>
    /// Subscription identifier.
    /// </summary>
    [JsonPropertyName("subscription_id")]
    public string SubscriptionId
    {
        get => !string.IsNullOrEmpty(_subscriptionId) ? _subscriptionId : _id ?? string.Empty;
        init => _subscriptionId = value;
    }

    /// <summary>
    /// Customer object associated with this subscription.
    /// </summary>
    [JsonPropertyName("customer")]
    public SubscriptionEventCustomer? Customer { get; init; }

    /// <summary>
    /// Associated customer ID.
    /// </summary>
    [JsonPropertyName("customer_id")]
    public string? CustomerId => Customer?.CustomerId;

    /// <summary>
    /// Product ID this subscription is billing for.
    /// </summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; init; } = string.Empty;

    /// <summary>
    /// Status of the subscription (e.g. 'active', 'canceled').
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Collection method ('charge_automatically', etc.).
    /// </summary>
    [JsonPropertyName("collection_method")]
    public string? CollectionMethod { get; init; }

    /// <summary>
    /// Currency code.
    /// </summary>
    [JsonPropertyName("currency")]
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;

    /// <summary>
    /// Recurring subscription amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public BachsPrice? Amount { get; init; }

    /// <summary>
    /// Billing cycle cadence.
    /// </summary>
    [JsonPropertyName("billing_cycle")]
    public SubscriptionBillingCycle? BillingCycle { get; init; }

    /// <summary>
    /// Number of subscription units.
    /// </summary>
    [JsonPropertyName("quantity")]
    public int? Quantity { get; init; }

    /// <summary>
    /// Start of current period.
    /// </summary>
    [JsonPropertyName("current_period_start")]
    public DateTimeOffset? CurrentPeriodStart { get; init; }

    /// <summary>
    /// End of current period.
    /// </summary>
    [JsonPropertyName("current_period_end")]
    public DateTimeOffset? CurrentPeriodEnd { get; init; }

    /// <summary>
    /// Next scheduled billing timestamp.
    /// </summary>
    [JsonPropertyName("next_billed_at")]
    public DateTimeOffset? NextBilledAt { get; init; }

    /// <summary>
    /// Free trial end timestamp.
    /// </summary>
    [JsonPropertyName("trial_end")]
    public DateTimeOffset? TrialEnd { get; init; }

    /// <summary>
    /// Whether the subscription will cancel at the period end.
    /// </summary>
    [JsonPropertyName("cancel_at_period_end")]
    public bool CancelAtPeriodEnd { get; init; }

    /// <summary>
    /// When the subscription was canceled.
    /// </summary>
    [JsonPropertyName("canceled_at")]
    public DateTimeOffset? CanceledAt { get; init; }

    /// <summary>
    /// Additional line items.
    /// </summary>
    [JsonPropertyName("items")]
    public IReadOnlyList<object>? Items { get; init; }

    /// <summary>
    /// When the subscription was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    /// <summary>
    /// Custom key-value pairs stored on the subscription.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Customer information attached to a subscription webhook event.
/// </summary>
public sealed record SubscriptionEventCustomer
{
    private readonly string? _id;
    private readonly string? _customerId;

    [JsonPropertyName("id")]
    public string Id
    {
        get => !string.IsNullOrEmpty(_id) ? _id : _customerId ?? string.Empty;
        init => _id = value;
    }

    [JsonPropertyName("customer_id")]
    public string CustomerId
    {
        get => !string.IsNullOrEmpty(_customerId) ? _customerId : _id ?? string.Empty;
        init => _customerId = value;
    }

    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; init; }

    [JsonPropertyName("billing_address")]
    public Address? BillingAddress { get; init; }

    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }
}

/// <summary>
/// Billing cycle configuration for a subscription event.
/// </summary>
public sealed record SubscriptionBillingCycle
{
    [JsonPropertyName("interval")]
    public string Interval { get; init; } = string.Empty;

    [JsonPropertyName("frequency")]
    public int Frequency { get; init; } = 1;
}
