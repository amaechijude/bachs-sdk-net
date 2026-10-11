using System.Text.Json.Serialization;
using Bachs.Net.Serialization;

namespace Bachs.Net.Models.Checkout;

/// <summary>
/// Detailed checkout session response returned by GET /v1/checkout-sessions/{checkout_id}.
/// </summary>
public sealed record GetCheckoutSessionResponse
{
    /// <summary>
    /// Unique checkout identifier (starts with 'chk_').
    /// </summary>
    [JsonPropertyName("checkout_id")]
    public string CheckoutId { get; init; } = string.Empty;

    /// <summary>
    /// Convenient alias for <see cref="CheckoutId"/>.
    /// </summary>
    [JsonIgnore]
    public string Id => CheckoutId;

    /// <summary>
    /// Current lifecycle status ('open', 'completed', 'expired', 'cancelled').
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// The recurring cadence when the checkout starts a subscription; null for a one-time checkout.
    /// </summary>
    [JsonPropertyName("recurring")]
    public CheckoutRecurring? Recurring { get; init; }

    /// <summary>
    /// Payment lifecycle status ('requires_payment_method', 'requires_confirmation', 'requires_action',
    /// 'processing', 'succeeded', 'failed', 'canceled').
    /// </summary>
    [JsonPropertyName("payment_status")]
    public string? PaymentStatus { get; init; }

    /// <summary>
    /// Origin that created the checkout (e.g. 'CHECKOUT_SESSION' or 'API').
    /// </summary>
    [JsonPropertyName("source_type")]
    public string? SourceType { get; init; }

    /// <summary>
    /// Total amount in <see cref="Currency"/>.
    /// </summary>
    [JsonPropertyName("amount")]
    public Price? Amount { get; init; }

    /// <summary>
    /// Base currency code (e.g., 'USD', 'NGN').
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    /// <summary>
    /// The reference set when creating the session; null if none set.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    /// The payment created by this checkout once attempted; null before then.
    /// </summary>
    [JsonPropertyName("charge")]
    public CheckoutPaymentResponse? Charge { get; init; }

    /// <summary>
    /// Payment method selected for the checkout (e.g., 'CARD', 'NGN_BANK_TRANSFER', 'MOMO_GHS', 'CRYPTO').
    /// </summary>
    [JsonPropertyName("payment_method")]
    public string? PaymentMethod { get; init; }

    /// <summary>
    /// The customer record attached to the checkout.
    /// </summary>
    [JsonPropertyName("customer")]
    public CheckoutCustomer? Customer { get; init; }

    /// <summary>
    /// What the buyer supplied on the checkout page; present whenever identity was collected.
    /// </summary>
    [JsonPropertyName("customer_details")]
    public CheckoutCustomerDetails? CustomerDetails { get; init; }

    /// <summary>
    /// URL the customer is redirected to after successful payment.
    /// </summary>
    [JsonPropertyName("success_url")]
    public string? SuccessUrl { get; init; }

    /// <summary>
    /// URL the customer is redirected to if they cancel.
    /// </summary>
    [JsonPropertyName("cancel_url")]
    public string? CancelUrl { get; init; }

    /// <summary>
    /// Resolved product line items. Populated for 'CART' sessions; null for 'SELECTION' sessions before product pick.
    /// </summary>
    [JsonPropertyName("products")]
    public IReadOnlyList<ResolvedProductItem>? Products { get; init; }

    /// <summary>
    /// Currency the customer selected for billing.
    /// </summary>
    [JsonPropertyName("billing_currency")]
    public string? BillingCurrency { get; init; }

    /// <summary>
    /// The platform's cut of this sale in base currency.
    /// </summary>
    [JsonPropertyName("platform_fee")]
    public Price? PlatformFee { get; init; }

    /// <summary>
    /// The seller's contracted share of this sale in base currency.
    /// </summary>
    [JsonPropertyName("destination_amount")]
    public Price? DestinationAmount { get; init; }

    /// <summary>
    /// How products are presented ('CART' or 'SELECTION').
    /// </summary>
    [JsonPropertyName("session_mode")]
    public string? SessionMode { get; init; }

    /// <summary>
    /// Public metadata attached at session creation.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// ISO 8601 creation timestamp in UTC.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// ISO 8601 expiry timestamp in UTC.
    /// </summary>
    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>
    /// ISO 8601 timestamp when the session was completed in UTC.
    /// </summary>
    [JsonPropertyName("completed_at")]
    public DateTimeOffset? CompletedAt { get; init; }

    /// <summary>
    /// ISO 8601 last-updated timestamp in UTC.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }
}

/// <summary>
/// Recurring billing interval cadence when a checkout creates a subscription.
/// </summary>
public sealed record CheckoutRecurring
{
    /// <summary>
    /// The billing interval ('day', 'week', 'month', 'year').
    /// </summary>
    [JsonPropertyName("interval")]
    public string Interval { get; init; } = string.Empty;

    /// <summary>
    /// Number of intervals per billing cycle. Defaults to 1.
    /// </summary>
    [JsonPropertyName("interval_count")]
    public int IntervalCount { get; init; } = 1;
}

/// <summary>
/// Resolved customer record attached to a checkout session.
/// </summary>
public sealed record CheckoutCustomer
{
    /// <summary>
    /// Customer ID (starts with 'cust_'). Null until customer is matched or created.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// Customer email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    /// <summary>
    /// Customer name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

/// <summary>
/// Buyer contact details supplied during checkout on the hosted page.
/// </summary>
public sealed record CheckoutCustomerDetails
{
    /// <summary>
    /// Email supplied by the buyer.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    /// <summary>
    /// Name supplied by the buyer.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Phone number supplied by the buyer.
    /// </summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; init; }
}

/// <summary>
/// A resolved product line item within a retrieved checkout session.
/// </summary>
public sealed record ResolvedProductItem
{
    /// <summary>
    /// Product identifier (starts with 'prod_').
    /// </summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; init; } = string.Empty;

    /// <summary>
    /// Product display name.
    /// </summary>
    [JsonPropertyName("product_name")]
    public string ProductName { get; init; } = string.Empty;

    /// <summary>
    /// Number of units.
    /// </summary>
    [JsonPropertyName("quantity")]
    public int Quantity { get; init; }

    /// <summary>
    /// Price per unit in <see cref="Currency"/>.
    /// </summary>
    [JsonPropertyName("unit_amount")]
    public Price UnitAmount { get; init; }

    /// <summary>
    /// Currency code for this line item.
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; init; } = string.Empty;

    /// <summary>
    /// Pricing mode: 'fixed', 'free', or 'custom'.
    /// </summary>
    [JsonPropertyName("price_type")]
    public string PriceType { get; init; } = string.Empty;

    /// <summary>
    /// Minimum allowed amount when price_type is custom.
    /// </summary>
    [JsonPropertyName("minimum_amount")]
    public Price? MinimumAmount { get; init; }

    /// <summary>
    /// Maximum allowed amount when price_type is custom.
    /// </summary>
    [JsonPropertyName("maximum_amount")]
    public Price? MaximumAmount { get; init; }

    /// <summary>
    /// Total for this line item (unit_amount × quantity).
    /// </summary>
    [JsonPropertyName("line_total")]
    public Price LineTotal { get; init; }
}

/// <summary>
/// Detailed payment response attached to a checkout session.
/// </summary>
public sealed record CheckoutPaymentResponse
{
    /// <summary>
    /// Unique identifier for the payment (starts with 'pay_').
    /// </summary>
    [JsonPropertyName("payment_id")]
    public string PaymentId { get; init; } = string.Empty;

    /// <summary>
    /// Convenient alias for <see cref="PaymentId"/>.
    /// </summary>
    [JsonIgnore]
    public string ChargeId => PaymentId;

    /// <summary>
    /// Checkout reference when available.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    /// Why this payment exists ('purchase', 'subscription_create', 'subscription_cycle', 'subscription_update').
    /// </summary>
    [JsonPropertyName("billing_reason")]
    public string? BillingReason { get; init; }

    /// <summary>
    /// Associated checkout identifier.
    /// </summary>
    [JsonPropertyName("checkout_id")]
    public string? CheckoutId { get; init; }

    /// <summary>
    /// Current payment status ('created', 'processing', 'succeeded', 'failed', 'refunded', etc.).
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Whether this payment can currently be refunded.
    /// </summary>
    [JsonPropertyName("is_refundable")]
    public bool? IsRefundable { get; init; }

    /// <summary>
    /// Requested amount in <see cref="Currency"/>.
    /// </summary>
    [JsonPropertyName("amount")]
    public Price? Amount { get; init; }

    /// <summary>
    /// Amount received so far.
    /// </summary>
    [JsonPropertyName("amount_paid")]
    public Price? AmountPaid { get; init; }

    /// <summary>
    /// Remaining amount still expected.
    /// </summary>
    [JsonPropertyName("amount_remaining")]
    public Price? AmountRemaining { get; init; }

    /// <summary>
    /// Payment currency code.
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    /// <summary>
    /// Processing fee for this payment converted to USD.
    /// </summary>
    [JsonPropertyName("fee_usd")]
    public Price? FeeUsd { get; init; }

    /// <summary>
    /// Processing fee in original collection currency.
    /// </summary>
    [JsonPropertyName("fees")]
    public CheckoutFeeInfo? Fees { get; init; }

    /// <summary>
    /// Whether merchant bears processing cost.
    /// </summary>
    [JsonPropertyName("merchant_bears_cost")]
    public bool? MerchantBearsCost { get; init; }

    /// <summary>
    /// Platform fee cut in base currency.
    /// </summary>
    [JsonPropertyName("platform_fee")]
    public Price? PlatformFee { get; init; }

    /// <summary>
    /// Destination account share in base currency.
    /// </summary>
    [JsonPropertyName("destination_amount")]
    public Price? DestinationAmount { get; init; }

    /// <summary>
    /// Who Bachs processing fee came from ('merchant' or 'platform').
    /// </summary>
    [JsonPropertyName("fee_paid_by")]
    public string? FeePaidBy { get; init; }

    /// <summary>
    /// Method rail used to collect payment (e.g. 'USD_CARD', 'NGN_BANK_TRANSFER', 'MOMO_GHS').
    /// </summary>
    [JsonPropertyName("payment_method")]
    public string? PaymentMethod { get; init; }

    /// <summary>
    /// Origin channel (e.g. 'checkout' or 'api').
    /// </summary>
    [JsonPropertyName("channel")]
    public string? Channel { get; init; }

    /// <summary>
    /// Payment description or narration.
    /// </summary>
    [JsonPropertyName("narration")]
    public string? Narration { get; init; }

    /// <summary>
    /// Human-readable payment status message.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    /// <summary>
    /// Public metadata stored for this payment.
    /// </summary>
    [JsonPropertyName("meta")]
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// Customer details attached to the payment.
    /// </summary>
    [JsonPropertyName("customer")]
    public CheckoutCustomer? Customer { get; init; }

    /// <summary>
    /// Product line items covered by this payment.
    /// </summary>
    [JsonPropertyName("line_items")]
    public IReadOnlyList<CheckoutPaymentProductItem>? LineItems { get; init; }

    /// <summary>
    /// Associated subscription ID for recurring payments.
    /// </summary>
    [JsonPropertyName("subscription_id")]
    public string? SubscriptionId { get; init; }

    /// <summary>
    /// Invoice associated with subscription payment.
    /// </summary>
    [JsonPropertyName("invoice")]
    public CheckoutPaymentInvoiceInfo? Invoice { get; init; }

    /// <summary>
    /// IDs of any refunds issued for this payment.
    /// </summary>
    [JsonPropertyName("refunds")]
    public IReadOnlyList<string>? Refunds { get; init; }

    /// <summary>
    /// Chronological list of status changes for this payment.
    /// </summary>
    [JsonPropertyName("status_history")]
    public IReadOnlyList<CheckoutPaymentStatusHistoryItem>? StatusHistory { get; init; }

    /// <summary>
    /// Creation timestamp in UTC.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Last update timestamp in UTC.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; init; }

    /// <summary>
    /// Completion timestamp in UTC when available.
    /// </summary>
    [JsonPropertyName("completed_at")]
    public DateTimeOffset? CompletedAt { get; init; }
}

/// <summary>
/// Processing fee details in the currency charged.
/// </summary>
public sealed record CheckoutFeeInfo
{
    [JsonPropertyName("amount")]
    public Price Amount { get; init; }

    [JsonPropertyName("currency")]
    public string Currency { get; init; } = string.Empty;
}

/// <summary>
/// Product line item purchased in a payment.
/// </summary>
public sealed record CheckoutPaymentProductItem
{
    [JsonPropertyName("product_id")]
    public string ProductId { get; init; } = string.Empty;

    [JsonPropertyName("product_name")]
    public string ProductName { get; init; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; init; }

    [JsonPropertyName("unit_amount")]
    public Price UnitAmount { get; init; }

    [JsonPropertyName("currency")]
    public string Currency { get; init; } = string.Empty;

    [JsonPropertyName("line_total")]
    public Price LineTotal { get; init; }
}

/// <summary>
/// Subscription invoice details attached to a payment.
/// </summary>
public sealed record CheckoutPaymentInvoiceInfo
{
    [JsonPropertyName("invoice_id")]
    public string InvoiceId { get; init; } = string.Empty;

    [JsonPropertyName("number")]
    public string? Number { get; init; }

    [JsonPropertyName("subscription_id")]
    public string? SubscriptionId { get; init; }

    [JsonPropertyName("period_start")]
    public DateTimeOffset? PeriodStart { get; init; }

    [JsonPropertyName("period_end")]
    public DateTimeOffset? PeriodEnd { get; init; }

    [JsonPropertyName("kind")]
    public string? Kind { get; init; }
}

/// <summary>
/// Status change history event for a checkout payment.
/// </summary>
public sealed record CheckoutPaymentStatusHistoryItem
{
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("occurred_at")]
    public DateTimeOffset OccurredAt { get; init; }

    [JsonPropertyName("reason")]
    public string? Reason { get; init; }
}
