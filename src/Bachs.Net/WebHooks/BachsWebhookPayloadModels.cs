using System.Text.Json;
using System.Text.Json.Serialization;
using Bachs.Net.Models.Common;
using Bachs.Net.Serialization;

namespace Bachs.Net.WebHooks;

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
    public Price? Subtotal { get; init; }

    [JsonPropertyName("total")]
    public Price? Total { get; init; }

    [JsonPropertyName("amount_paid")]
    public Price? AmountPaid { get; init; }

    [JsonPropertyName("amount_remaining")]
    public Price? AmountRemaining { get; init; }

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
    public Price Amount { get; init; }

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
