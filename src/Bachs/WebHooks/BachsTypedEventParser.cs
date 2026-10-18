using Bachs.Models.Checkout;
using Bachs.Models.Conversions;
using Bachs.Models.Customers;
using Bachs.Models.Disputes;
using Bachs.Models.Payments;
using Bachs.Models.Payouts;
using Bachs.Models.Refunds;
using Bachs.Models.Subscriptions;
using Bachs.WebHooks.EventData;

namespace Bachs.WebHooks;

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
                var data = evt.GetData<CheckoutEventData>();
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
                var data = evt.GetData<CheckoutEventData>();
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
                var data = evt.GetData<CollectionEventData>();
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
                var data = evt.GetData<CollectionEventData>();
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
                var data = evt.GetData<CollectionEventData>();
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
                var data = evt.GetData<SubscriptionEventData>();
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
                var data = evt.GetData<SubscriptionEventData>();
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
                var data = evt.GetData<SubscriptionEventData>();
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
                var data = evt.GetData<PayoutEventData>();
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
                var data = evt.GetData<PayoutEventData>();
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
                var data = evt.GetData<PayoutEventData>();
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
                var data = evt.GetData<RefundEventData>();
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
                var data = evt.GetData<RefundEventData>();
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
                var data = evt.GetData<RefundEventData>();
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
                var data = evt.GetData<DisputeEventData>();
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
                var data = evt.GetData<DisputeEventData>();
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
                var data = evt.GetData<ConversionEventData>();
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
                var data = evt.GetData<ConversionEventData>();
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
                var data = evt.GetData<CustomerEventData>();
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
                var data = evt.GetData<CustomerEventData>();
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
