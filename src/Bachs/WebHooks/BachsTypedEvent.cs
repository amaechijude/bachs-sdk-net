using System.Text.Json;
using Bachs.Models.Checkout;
using Bachs.Models.Conversions;
using Bachs.Models.Customers;
using Bachs.Models.Disputes;
using Bachs.Models.Payments;
using Bachs.Models.Payouts;
using Bachs.Models.Refunds;
using Bachs.Models.Subscriptions;
using Bachs.Serialization;
using Bachs.WebHooks.EventData;

namespace Bachs.WebHooks;

/// <summary>
/// Abstract base record for strongly-typed Bachs webhook events.
/// </summary>
public abstract record BachsTypedEvent
{
    /// <summary>
    /// Unique identifier for the event (starts with 'evt_').
    /// </summary>
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Event type string in 'resource.action' format (e.g. 'collection.succeeded', 'checkout.completed').
    /// </summary>
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// Timestamp when the event was recorded in UTC.
    /// </summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// The organization ID the event belongs to.
    /// </summary>
    public string? OrganizationId { get; init; }

    /// <summary>
    /// Origin account ID when the event originated from a connected account (Connect events).
    /// </summary>
    public string? Account { get; init; }
}

/// <summary>
/// Strongly-typed Bachs webhook event carrying typed payload data.
/// </summary>
/// <typeparam name="TData">The model type representing the event's data payload.</typeparam>
public abstract record BachsTypedEvent<TData> : BachsTypedEvent
{
    /// <summary>
    /// Strongly-typed payload data for this event.
    /// </summary>
    public TData Data { get; init; } = default!;
}

// ---------------------------------------------------------
// Checkout Events
// ---------------------------------------------------------

/// <summary>Fired when a checkout session finishes successfully.</summary>
public sealed record CheckoutCompletedEvent : BachsTypedEvent<CheckoutEventData>;

/// <summary>Fired when a checkout session lapses past its expiry.</summary>
public sealed record CheckoutExpiredEvent : BachsTypedEvent<CheckoutEventData>;

// ---------------------------------------------------------
// Payment / Collection Events
// ---------------------------------------------------------

/// <summary>Fired when a payment collection successfully completes.</summary>
public sealed record CollectionSucceededEvent : BachsTypedEvent<CollectionEventData>;

/// <summary>Fired when a payment collection attempt fails.</summary>
public sealed record CollectionFailedEvent : BachsTypedEvent<CollectionEventData>;

/// <summary>Fired when a payment collection is received for less than requested.</summary>
public sealed record CollectionUnderpaidEvent : BachsTypedEvent<CollectionEventData>;

// ---------------------------------------------------------
// Payment Method Events
// ---------------------------------------------------------

/// <summary>Fired when a customer payment method is saved or tokenized.</summary>
public sealed record PaymentMethodSavedEvent : BachsTypedEvent<JsonElement>;

// ---------------------------------------------------------
// Subscription Events
// ---------------------------------------------------------

/// <summary>Fired when a new customer subscription is created.</summary>
public sealed record SubscriptionCreatedEvent : BachsTypedEvent<SubscriptionEventData>;

/// <summary>Fired when an existing subscription is modified or updated.</summary>
public sealed record SubscriptionUpdatedEvent : BachsTypedEvent<SubscriptionEventData>;

/// <summary>Fired when a subscription is canceled or expired.</summary>
public sealed record SubscriptionDeletedEvent : BachsTypedEvent<SubscriptionEventData>;

// ---------------------------------------------------------
// Invoice Events
// ---------------------------------------------------------

/// <summary>Fired when an invoice is generated for a billing cycle.</summary>
public sealed record InvoiceCreatedEvent : BachsTypedEvent<InvoiceEventData>;

/// <summary>Fired when an invoice is paid in full.</summary>
public sealed record InvoicePaidEvent : BachsTypedEvent<InvoiceEventData>;

/// <summary>Fired when an invoice payment attempt fails.</summary>
public sealed record InvoicePaymentFailedEvent : BachsTypedEvent<InvoiceEventData>;

// ---------------------------------------------------------
// Payout / Withdrawal Events
// ---------------------------------------------------------

/// <summary>Fired when a payout/withdrawal is initiated.</summary>
public sealed record PayoutCreatedEvent : BachsTypedEvent<PayoutEventData>;

/// <summary>Fired when a payout/withdrawal successfully clears.</summary>
public sealed record PayoutPaidEvent : BachsTypedEvent<PayoutEventData>;

/// <summary>Fired when a payout/withdrawal attempt fails.</summary>
public sealed record PayoutFailedEvent : BachsTypedEvent<PayoutEventData>;

// ---------------------------------------------------------
// Refund Events
// ---------------------------------------------------------

/// <summary>Fired when a refund is requested.</summary>
public sealed record RefundCreatedEvent : BachsTypedEvent<RefundEventData>;

/// <summary>Fired when a refund is successfully processed.</summary>
public sealed record RefundPaidEvent : BachsTypedEvent<RefundEventData>;

/// <summary>Fired when a refund attempt fails.</summary>
public sealed record RefundFailedEvent : BachsTypedEvent<RefundEventData>;

// ---------------------------------------------------------
// Dispute Events
// ---------------------------------------------------------

/// <summary>Fired when a chargeback or payment dispute is opened.</summary>
public sealed record DisputeCreatedEvent : BachsTypedEvent<DisputeEventData>;

/// <summary>Fired when dispute status or evidence changes.</summary>
public sealed record DisputeUpdatedEvent : BachsTypedEvent<DisputeEventData>;

// ---------------------------------------------------------
// Currency Conversion Events
// ---------------------------------------------------------

/// <summary>Fired when an FX currency conversion completes.</summary>
public sealed record ConversionCompletedEvent : BachsTypedEvent<ConversionEventData>;

/// <summary>Fired when an FX currency conversion fails.</summary>
public sealed record ConversionFailedEvent : BachsTypedEvent<ConversionEventData>;

// ---------------------------------------------------------
// Customer Events
// ---------------------------------------------------------

/// <summary>Fired when a customer record is created.</summary>
public sealed record CustomerCreatedEvent : BachsTypedEvent<CustomerEventData>;

/// <summary>Fired when a customer record is updated.</summary>
public sealed record CustomerUpdatedEvent : BachsTypedEvent<CustomerEventData>;

// ---------------------------------------------------------
// Connect & Platform Events
// ---------------------------------------------------------

/// <summary>Fired when a connected account's onboarding/requirements status changes.</summary>
public sealed record AccountUpdatedEvent : BachsTypedEvent<AccountUpdatedEventData>;

/// <summary>Fired when a connected account's capability status changes.</summary>
public sealed record CapabilityUpdatedEvent : BachsTypedEvent<CapabilityUpdatedEventData>;

/// <summary>Fired when internal funds transfer completes between platform and account.</summary>
public sealed record TransferCreatedEvent : BachsTypedEvent<TransferEventData>;

// ---------------------------------------------------------
// Unknown / Forward-Compatibility Fallback Event
// ---------------------------------------------------------

/// <summary>
/// Fallback for new or unrecognized event types sent by Bachs, ensuring forward compatibility.
/// </summary>
public sealed record UnknownBachsEvent : BachsTypedEvent
{
    /// <summary>
    /// Raw JSON element payload for the event data.
    /// </summary>
    public JsonElement RawData { get; init; }

    /// <summary>
    /// Deserializes the raw event data element to a strongly-typed object.
    /// </summary>
    public T? GetData<T>() => BachsJsonSerializer.Deserialize<T>(RawData.GetRawText());
}
