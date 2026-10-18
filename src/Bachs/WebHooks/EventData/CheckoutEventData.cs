using System.Text.Json.Serialization;
using Bachs.Models.Common;

namespace Bachs.WebHooks.EventData;

/// <summary>
/// Event data for checkout webhook events (<c>checkout.completed</c> and <c>checkout.expired</c>).
/// </summary>
public sealed record CheckoutEventData
{
    private readonly string? _id;
    private readonly string? _checkoutId;

    /// <summary>
    /// Checkout session ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id
    {
        get => !string.IsNullOrEmpty(_id) ? _id : _checkoutId ?? string.Empty;
        init => _id = value;
    }

    /// <summary>
    /// Checkout session identifier (starts with 'chk_').
    /// </summary>
    [JsonPropertyName("checkout_id")]
    public string CheckoutId
    {
        get => !string.IsNullOrEmpty(_checkoutId) ? _checkoutId : _id ?? string.Empty;
        init => _checkoutId = value;
    }

    /// <summary>
    /// Checkout lifecycle status (e.g. 'completed', 'expired').
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// The checkout's mode ('payment', 'setup', 'subscription').
    /// </summary>
    [JsonPropertyName("mode")]
    public string? Mode { get; init; }

    /// <summary>
    /// Payment collection status ('paid', 'no_payment_required', null).
    /// </summary>
    [JsonPropertyName("payment_status")]
    public string? PaymentStatus { get; init; }

    /// <summary>
    /// Checkout amount as a decimal price.
    /// </summary>
    [JsonPropertyName("amount")]
    public BachsPrice? Amount { get; init; }

    /// <summary>
    /// Currency code (e.g., 'USD', 'NGN').
    /// </summary>
    [JsonPropertyName("currency")]
    public BachsCurrency? Currency { get; init; }

    /// <summary>
    /// Checkout reference supplied during checkout creation.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    /// Customer record attached to the checkout session, if any.
    /// </summary>
    [JsonPropertyName("customer")]
    public CheckoutEventCustomer? Customer { get; init; }

    /// <summary>
    /// Buyer contact details supplied during checkout on the hosted page.
    /// </summary>
    [JsonPropertyName("customer_details")]
    public CheckoutEventCustomerDetails? CustomerDetails { get; init; }

    /// <summary>
    /// Associated charge created by this checkout, if any.
    /// </summary>
    [JsonPropertyName("charge")]
    public CheckoutEventCharge? Charge { get; init; }

    /// <summary>
    /// Associated subscription reference for subscription-mode checkouts.
    /// </summary>
    [JsonPropertyName("subscription")]
    public IReadOnlyDictionary<string, string>? Subscription { get; init; }

    /// <summary>
    /// URL the customer was redirected to after successful completion.
    /// </summary>
    [JsonPropertyName("success_url")]
    public string? SuccessUrl { get; init; }

    /// <summary>
    /// URL the customer was redirected to if they canceled.
    /// </summary>
    [JsonPropertyName("cancel_url")]
    public string? CancelUrl { get; init; }

    /// <summary>
    /// Public metadata attached to the checkout session.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// ISO 8601 UTC timestamp when the session was completed.
    /// </summary>
    [JsonPropertyName("completed_at")]
    public DateTimeOffset? CompletedAt { get; init; }

    /// <summary>
    /// ISO 8601 UTC timestamp when the checkout expires.
    /// </summary>
    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>
    /// ISO 8601 UTC timestamp when the checkout session was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }
}

/// <summary>
/// Customer details inside a checkout webhook event payload.
/// </summary>
public sealed record CheckoutEventCustomer
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
/// Buyer contact details captured on the checkout hosted page.
/// </summary>
public sealed record CheckoutEventCustomerDetails
{
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

/// <summary>
/// Charge summary inside a checkout webhook event payload.
/// </summary>
public sealed record CheckoutEventCharge
{
    private readonly string? _id;
    private readonly string? _chargeId;

    [JsonPropertyName("id")]
    public string Id
    {
        get => !string.IsNullOrEmpty(_id) ? _id : _chargeId ?? string.Empty;
        init => _id = value;
    }

    [JsonPropertyName("charge_id")]
    public string ChargeId
    {
        get => !string.IsNullOrEmpty(_chargeId) ? _chargeId : _id ?? string.Empty;
        init => _chargeId = value;
    }

    [JsonPropertyName("organization_id")]
    public string? OrganizationId { get; init; }

    [JsonPropertyName("customer_id")]
    public string? CustomerId { get; init; }

    [JsonPropertyName("amount")]
    public BachsPrice Amount { get; init; }

    [JsonPropertyName("currency")]
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;

    [JsonPropertyName("settlement_currency")]
    public BachsCurrency? SettlementCurrency { get; init; }

    [JsonPropertyName("settlement_amount")]
    public BachsPrice? SettlementAmount { get; init; }

    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }
}
