using System.Text.Json.Serialization;
using Bachs.Models.Common;

namespace Bachs.WebHooks.EventData;

/// <summary>
/// Payload data for invoice webhook events ('invoice.created', 'invoice.paid', 'invoice.payment_failed').
/// </summary>
public sealed record InvoiceEventData
{
    [JsonPropertyName("invoice_id")]
    public string InvoiceId { get; init; } = string.Empty;

    [JsonIgnore]
    public string Id => InvoiceId;

    [JsonPropertyName("subscription")]
    public InvoiceSubscriptionInfo? Subscription { get; init; }

    [JsonPropertyName("customer")]
    public InvoiceCustomerInfo? Customer { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("collection_method")]
    public string? CollectionMethod { get; init; }

    [JsonPropertyName("currency")]
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;

    [JsonPropertyName("subtotal")]
    public BachsPrice? Subtotal { get; init; }

    [JsonPropertyName("total")]
    public BachsPrice? Total { get; init; }

    [JsonPropertyName("amount_paid")]
    public BachsPrice? AmountPaid { get; init; }

    [JsonPropertyName("amount_remaining")]
    public BachsPrice? AmountRemaining { get; init; }

    [JsonPropertyName("period_start")]
    public DateTimeOffset? PeriodStart { get; init; }

    [JsonPropertyName("period_end")]
    public DateTimeOffset? PeriodEnd { get; init; }

    [JsonPropertyName("attempt_count")]
    public int? AttemptCount { get; init; }

    [JsonPropertyName("next_payment_attempt")]
    public DateTimeOffset? NextPaymentAttempt { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }

    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Subscription reference inside an invoice event.
/// </summary>
public sealed record InvoiceSubscriptionInfo
{
    [JsonPropertyName("subscription_id")]
    public string SubscriptionId { get; init; } = string.Empty;

    [JsonIgnore]
    public string Id => SubscriptionId;
}

/// <summary>
/// Customer reference inside an invoice event.
/// </summary>
public sealed record InvoiceCustomerInfo
{
    [JsonPropertyName("customer_id")]
    public string CustomerId { get; init; } = string.Empty;

    [JsonIgnore]
    public string Id => CustomerId;

    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}
