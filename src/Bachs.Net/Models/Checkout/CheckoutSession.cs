using System.Text.Json.Serialization;
using Bachs.Net.Serialization;

namespace Bachs.Net.Models.Checkout;

/// <summary>
/// Represents a Bachs hosted checkout session.
/// </summary>
public sealed record CheckoutSession
{
    /// <summary>
    /// Unique identifier for the checkout session (starts with 'chk_').
    /// </summary>
    [JsonPropertyName("checkout_id")]
    public string CheckoutId { get; init; } = string.Empty;

    /// <summary>
    /// Hosted URL where the customer is redirected to complete payment.
    /// </summary>
    [JsonPropertyName("checkout_url")]
    public string CheckoutUrl { get; init; } = string.Empty;

    /// <summary>
    /// Status of the checkout session ('open', 'completed', 'expired').
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// When the checkout session expires.
    /// </summary>
    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>
    /// When the checkout session was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// The customer record backing this checkout, if one exists.
    /// </summary>
    [JsonPropertyName("customer")]
    public CheckoutCustomerRecord? Customer { get; init; }

    /// <summary>
    /// The customer details entered on the checkout page, whether or not a customer record exists.
    /// </summary>
    [JsonPropertyName("customer_details")]
    public CheckoutCustomerDetails? CustomerDetails { get; init; }

    /// <summary>
    /// The line items in the cart if the checkout was created with product items.
    /// </summary>
    [JsonPropertyName("product_cart")]
    public IReadOnlyList<CheckoutCartItem>? ProductCart { get; init; }

    /// <summary>
    /// Pricing configuration if the checkout was created with ad-hoc pricing.
    /// </summary>
    [JsonPropertyName("pricing")]
    public CheckoutPricing? Pricing { get; init; }

    /// <summary>
    /// The URL to redirect the customer to upon payment completion.
    /// </summary>
    [JsonPropertyName("success_url")]
    public string? SuccessUrl { get; init; }

    /// <summary>
    /// The URL to redirect the customer to if they cancel or abandon checkout.
    /// </summary>
    [JsonPropertyName("cancel_url")]
    public string? CancelUrl { get; init; }

    /// <summary>
    /// External reference or order ID provided at creation.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    /// Custom key-value metadata attached to this session.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// Associated charge ID or payment details if payment was completed.
    /// </summary>
    [JsonPropertyName("charge")]
    public CheckoutChargeSummary? Charge { get; init; }
}

/// <summary>
/// Customer record associated with a checkout session.
/// </summary>
public sealed record CheckoutCustomerRecord
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

/// <summary>
/// Contact details supplied by the customer at checkout.
/// </summary>
public sealed record CheckoutCustomerDetails
{
    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; init; }
}

/// <summary>
/// Cart item in a checkout session.
/// </summary>
public sealed record CheckoutCartItem
{
    [JsonPropertyName("product_id")]
    public string ProductId { get; init; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; init; }

    [JsonPropertyName("pricing")]
    public CheckoutItemPricingOverride? Pricing { get; init; }
}

/// <summary>
/// Pricing override for a cart item in a checkout session.
/// </summary>
public sealed record CheckoutItemPricingOverride
{
    [JsonPropertyName("price_type")]
    public string? PriceType { get; init; }

    [JsonPropertyName("amount")]
    public Price? Amount { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    [JsonPropertyName("preset_amount")]
    public Price? PresetAmount { get; init; }

    [JsonPropertyName("minimum_amount")]
    public Price? MinimumAmount { get; init; }

    [JsonPropertyName("maximum_amount")]
    public Price? MaximumAmount { get; init; }
}

/// <summary>
/// Ad-hoc pricing definition for checkout sessions without a product catalog.
/// </summary>
public sealed record CheckoutPricing
{
    [JsonPropertyName("currency")]
    public string Currency { get; init; } = string.Empty;

    [JsonPropertyName("amount")]
    public Price Amount { get; init; }

    [JsonPropertyName("currency_options")]
    public IReadOnlyDictionary<string, Price>? CurrencyOptions { get; init; }
}

/// <summary>
/// Charge summary linked to a completed checkout session.
/// </summary>
public sealed record CheckoutChargeSummary
{
    [JsonPropertyName("charge_id")]
    public string? ChargeId { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("amount")]
    public Price? Amount { get; init; }

    [JsonPropertyName("currency")]
    public string? Currency { get; init; }
}
