using System.Text.Json;
using Bachs.Net.Models.Checkout;
using Bachs.Net.Models.Conversions;
using Bachs.Net.Models.Customers;
using Bachs.Net.Models.Disputes;
using Bachs.Net.Models.Payments;
using Bachs.Net.Models.Payouts;
using Bachs.Net.Models.Refunds;
using Bachs.Net.Models.Subscriptions;
using Bachs.Net.Serialization;

namespace Bachs.Net.WebHooks;

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
public sealed record CheckoutCompletedEvent : BachsTypedEvent<GetCheckoutSessionResponse>;

/// <summary>Fired when a checkout session lapses past its expiry.</summary>
public sealed record CheckoutExpiredEvent : BachsTypedEvent<GetCheckoutSessionResponse>;

// ---------------------------------------------------------
// Payment / Collection Events
// ---------------------------------------------------------

/// <summary>Fired when a payment collection successfully completes.</summary>
public sealed record CollectionSucceededEvent : BachsTypedEvent<Payment>;

/// <summary>Fired when a payment collection attempt fails.</summary>
public sealed record CollectionFailedEvent : BachsTypedEvent<Payment>;

/// <summary>Fired when a payment collection is received for less than requested.</summary>
public sealed record CollectionUnderpaidEvent : BachsTypedEvent<Payment>;

// ---------------------------------------------------------
// Payment Method Events
// ---------------------------------------------------------

/// <summary>Fired when a customer payment method is saved or tokenized.</summary>
public sealed record PaymentMethodSavedEvent : BachsTypedEvent<JsonElement>;

// ---------------------------------------------------------
// Subscription Events
// ---------------------------------------------------------

/// <summary>Fired when a new customer subscription is created.</summary>
public sealed record SubscriptionCreatedEvent : BachsTypedEvent<Subscription>;

/// <summary>Fired when an existing subscription is modified or updated.</summary>
public sealed record SubscriptionUpdatedEvent : BachsTypedEvent<Subscription>;

/// <summary>Fired when a subscription is canceled or expired.</summary>
public sealed record SubscriptionDeletedEvent : BachsTypedEvent<Subscription>;

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
public sealed record PayoutCreatedEvent : BachsTypedEvent<Payout>;

/// <summary>Fired when a payout/withdrawal successfully clears.</summary>
public sealed record PayoutPaidEvent : BachsTypedEvent<Payout>;

/// <summary>Fired when a payout/withdrawal attempt fails.</summary>
public sealed record PayoutFailedEvent : BachsTypedEvent<Payout>;

// ---------------------------------------------------------
// Refund Events
// ---------------------------------------------------------

/// <summary>Fired when a refund is requested.</summary>
public sealed record RefundCreatedEvent : BachsTypedEvent<Refund>;

/// <summary>Fired when a refund is successfully processed.</summary>
public sealed record RefundPaidEvent : BachsTypedEvent<Refund>;

/// <summary>Fired when a refund attempt fails.</summary>
public sealed record RefundFailedEvent : BachsTypedEvent<Refund>;

// ---------------------------------------------------------
// Dispute Events
// ---------------------------------------------------------

/// <summary>Fired when a chargeback or payment dispute is opened.</summary>
public sealed record DisputeCreatedEvent : BachsTypedEvent<Dispute>;

/// <summary>Fired when dispute status or evidence changes.</summary>
public sealed record DisputeUpdatedEvent : BachsTypedEvent<Dispute>;

// ---------------------------------------------------------
// Currency Conversion Events
// ---------------------------------------------------------

/// <summary>Fired when an FX currency conversion completes.</summary>
public sealed record ConversionCompletedEvent : BachsTypedEvent<Conversion>;

/// <summary>Fired when an FX currency conversion fails.</summary>
public sealed record ConversionFailedEvent : BachsTypedEvent<Conversion>;

// ---------------------------------------------------------
// Customer Events
// ---------------------------------------------------------

/// <summary>Fired when a customer record is created.</summary>
public sealed record CustomerCreatedEvent : BachsTypedEvent<Customer>;

/// <summary>Fired when a customer record is updated.</summary>
public sealed record CustomerUpdatedEvent : BachsTypedEvent<Customer>;

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

/// <summary>
/// Parser helper that converts a <see cref="BachsWebhookEvent"/> into its strongly-typed <see cref="BachsTypedEvent"/> representation.
/// </summary>
public static class BachsTypedEventParser
{
    /// <summary>
    /// Parses a raw <see cref="BachsWebhookEvent"/> into a strongly-typed <see cref="BachsTypedEvent"/>.
    /// </summary>
    public static BachsTypedEvent Parse(BachsWebhookEvent evt)
    {
        ArgumentNullException.ThrowIfNull(evt);

        var id = evt.Id;
        var type = evt.Type;
        var createdAt = evt.CreatedAt;
        var organizationId = evt.OrganizationId;
        var account = evt.Account;

        switch (type.ToLowerInvariant())
        {
            case BachsEventTypes.CheckoutCompleted:
            {
                var data = evt.GetData<GetCheckoutSessionResponse>();
                return data is not null
                    ? new CheckoutCompletedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.CheckoutExpired:
            {
                var data = evt.GetData<GetCheckoutSessionResponse>();
                return data is not null
                    ? new CheckoutExpiredEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.CollectionSucceeded:
            {
                var data = evt.GetData<Payment>();
                return data is not null
                    ? new CollectionSucceededEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.CollectionFailed:
            {
                var data = evt.GetData<Payment>();
                return data is not null
                    ? new CollectionFailedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.CollectionUnderpaid:
            {
                var data = evt.GetData<Payment>();
                return data is not null
                    ? new CollectionUnderpaidEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.PaymentMethodSaved:
            {
                return new PaymentMethodSavedEvent
                {
                    Id = id,
                    Type = type,
                    CreatedAt = createdAt,
                    OrganizationId = organizationId,
                    Account = account,
                    Data = evt.Data,
                };
            }

            case BachsEventTypes.CustomerSubscriptionCreated:
            {
                var data = evt.GetData<Subscription>();
                return data is not null
                    ? new SubscriptionCreatedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.CustomerSubscriptionUpdated:
            {
                var data = evt.GetData<Subscription>();
                return data is not null
                    ? new SubscriptionUpdatedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.CustomerSubscriptionDeleted:
            {
                var data = evt.GetData<Subscription>();
                return data is not null
                    ? new SubscriptionDeletedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.InvoiceCreated:
            {
                var data = evt.GetData<InvoiceEventData>();
                return data is not null
                    ? new InvoiceCreatedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.InvoicePaid:
            {
                var data = evt.GetData<InvoiceEventData>();
                return data is not null
                    ? new InvoicePaidEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.InvoicePaymentFailed:
            {
                var data = evt.GetData<InvoiceEventData>();
                return data is not null
                    ? new InvoicePaymentFailedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.PayoutCreated:
            {
                var data = evt.GetData<Payout>();
                return data is not null
                    ? new PayoutCreatedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.PayoutPaid:
            {
                var data = evt.GetData<Payout>();
                return data is not null
                    ? new PayoutPaidEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.PayoutFailed:
            {
                var data = evt.GetData<Payout>();
                return data is not null
                    ? new PayoutFailedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.RefundCreated:
            {
                var data = evt.GetData<Refund>();
                return data is not null
                    ? new RefundCreatedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.RefundPaid:
            {
                var data = evt.GetData<Refund>();
                return data is not null
                    ? new RefundPaidEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.RefundFailed:
            {
                var data = evt.GetData<Refund>();
                return data is not null
                    ? new RefundFailedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.DisputeCreated:
            {
                var data = evt.GetData<Dispute>();
                return data is not null
                    ? new DisputeCreatedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.DisputeUpdated:
            {
                var data = evt.GetData<Dispute>();
                return data is not null
                    ? new DisputeUpdatedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.ConversionCompleted:
            {
                var data = evt.GetData<Conversion>();
                return data is not null
                    ? new ConversionCompletedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.ConversionFailed:
            {
                var data = evt.GetData<Conversion>();
                return data is not null
                    ? new ConversionFailedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.CustomerCreated:
            {
                var data = evt.GetData<Customer>();
                return data is not null
                    ? new CustomerCreatedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.CustomerUpdated:
            {
                var data = evt.GetData<Customer>();
                return data is not null
                    ? new CustomerUpdatedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.AccountUpdated:
            {
                var data = evt.GetData<AccountUpdatedEventData>();
                return data is not null
                    ? new AccountUpdatedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.CapabilityUpdated:
            {
                var data = evt.GetData<CapabilityUpdatedEventData>();
                return data is not null
                    ? new CapabilityUpdatedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            case BachsEventTypes.TransferCreated:
            {
                var data = evt.GetData<TransferEventData>();
                return data is not null
                    ? new TransferCreatedEvent
                    {
                        Id = id,
                        Type = type,
                        CreatedAt = createdAt,
                        OrganizationId = organizationId,
                        Account = account,
                        Data = data,
                    }
                    : CreateUnknown(evt);
            }

            default:
                return CreateUnknown(evt);
        }
    }

    private static UnknownBachsEvent CreateUnknown(BachsWebhookEvent evt) =>
        new()
        {
            Id = evt.Id,
            Type = evt.Type,
            CreatedAt = evt.CreatedAt,
            OrganizationId = evt.OrganizationId,
            Account = evt.Account,
            RawData = evt.Data,
        };
}
