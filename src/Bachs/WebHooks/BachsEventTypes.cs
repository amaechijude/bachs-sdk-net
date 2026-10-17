namespace Bachs.WebHooks;

/// <summary>
/// Known Bachs webhook event type constants in 'resource.action' format.
/// </summary>
public static class BachsEventTypes
{
    // Checkout
    public const string CheckoutCompleted = "checkout.completed";
    public const string CheckoutExpired = "checkout.expired";

    // Payments (Collections)
    public const string CollectionSucceeded = "collection.succeeded";
    public const string CollectionFailed = "collection.failed";
    public const string CollectionUnderpaid = "collection.underpaid";

    // Payment Methods
    public const string PaymentMethodSaved = "payment_method.saved";

    // Subscriptions
    public const string CustomerSubscriptionCreated = "customer.subscription.created";
    public const string CustomerSubscriptionUpdated = "customer.subscription.updated";
    public const string CustomerSubscriptionDeleted = "customer.subscription.deleted";

    // Invoices
    public const string InvoiceCreated = "invoice.created";
    public const string InvoicePaid = "invoice.paid";
    public const string InvoicePaymentFailed = "invoice.payment_failed";

    // Withdrawals (Payouts)
    public const string PayoutCreated = "payout.created";
    public const string PayoutPaid = "payout.paid";
    public const string PayoutFailed = "payout.failed";

    // Refunds
    public const string RefundCreated = "refund.created";
    public const string RefundPaid = "refund.paid";
    public const string RefundFailed = "refund.failed";

    // Disputes
    public const string DisputeCreated = "dispute.created";
    public const string DisputeUpdated = "dispute.updated";

    // Conversions
    public const string ConversionCompleted = "conversion.completed";
    public const string ConversionFailed = "conversion.failed";

    // Customers
    public const string CustomerCreated = "customer.created";
    public const string CustomerUpdated = "customer.updated";

    // Connect & Platform Events
    public const string AccountUpdated = "account.updated";
    public const string CapabilityUpdated = "capability.updated";
    public const string TransferCreated = "transfer.created";

    /// <summary>
    /// Gets all known Bachs event types.
    /// </summary>
    public static readonly IReadOnlyList<string> All =
    [
        CheckoutCompleted,
        CheckoutExpired,
        CollectionSucceeded,
        CollectionFailed,
        CollectionUnderpaid,
        PaymentMethodSaved,
        CustomerSubscriptionCreated,
        CustomerSubscriptionUpdated,
        CustomerSubscriptionDeleted,
        InvoiceCreated,
        InvoicePaid,
        InvoicePaymentFailed,
        PayoutCreated,
        PayoutPaid,
        PayoutFailed,
        RefundCreated,
        RefundPaid,
        RefundFailed,
        DisputeCreated,
        DisputeUpdated,
        ConversionCompleted,
        ConversionFailed,
        CustomerCreated,
        CustomerUpdated,
        AccountUpdated,
        CapabilityUpdated,
        TransferCreated,
    ];
}
