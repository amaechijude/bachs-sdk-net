using System.Text.Json.Serialization;
using Bachs.Net.Serialization;

namespace Bachs.Net.Models.Checkout;

/// <summary>
/// Request parameters to create a new checkout session.
/// Provide either <see cref="ProductCart"/> or <see cref="Pricing"/>, never both.
/// </summary>
public sealed record CreateCheckoutSessionRequest
{
    /// <summary>
    /// One or more catalog products being purchased.
    /// Provide this OR <see cref="Pricing"/>, not both.
    /// </summary>
    [JsonPropertyName("product_cart")]
    public IList<CheckoutCartItemRequest>? ProductCart { get; init; }

    /// <summary>
    /// Raw ad-hoc pricing with no catalog product.
    /// Provide this OR <see cref="ProductCart"/>, not both.
    /// </summary>
    [JsonPropertyName("pricing")]
    public CheckoutPricingRequest? Pricing { get; init; }

    /// <summary>
    /// The customer details or customer ID. Required for recurring products.
    /// </summary>
    [JsonPropertyName("customer")]
    public CheckoutCustomerRequest? Customer { get; init; }

    /// <summary>
    /// The URL to redirect the customer to upon successful payment completion.
    /// '?checkout_id=' will be appended to this URL by Bachs.
    /// </summary>
    [JsonPropertyName("success_url")]
    public string? SuccessUrl { get; init; }

    /// <summary>
    /// The URL to redirect the customer to if they cancel or abandon the checkout.
    /// </summary>
    [JsonPropertyName("cancel_url")]
    public string? CancelUrl { get; init; }

    /// <summary>
    /// Locks the session to a specific currency (e.g., "USD", "NGN").
    /// </summary>
    [JsonPropertyName("billing_currency")]
    public string? BillingCurrency { get; init; }

    /// <summary>
    /// Corridors to restrict available payment methods to (e.g. 'USD_CARD', 'NGN_BANK_TRANSFER', 'MOMO_GHS').
    /// </summary>
    [JsonPropertyName("payment_method_types")]
    public IList<string>? PaymentMethodTypes { get; init; }

    /// <summary>
    /// External order or charge ID reference (unique per organization, max 128 characters).
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    /// Decides whether collecting customer identity on the checkout page creates a customer record.
    /// Values: "if_required" (default) or "always".
    /// </summary>
    [JsonPropertyName("customer_creation")]
    public string? CustomerCreation { get; init; }

    /// <summary>
    /// Session lifetime in minutes (1 to 1440). Defaults to 60.
    /// </summary>
    [JsonPropertyName("expires_in_minutes")]
    public int? ExpiresInMinutes { get; init; }

    /// <summary>
    /// Custom key-value pairs (up to 20 keys, max 10 KB). Returned in webhooks.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Cart item for checkout session creation.
/// </summary>
public sealed record CheckoutCartItemRequest
{
    /// <summary>
    /// The ID of the catalog product (starts with 'prod_').
    /// </summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; init; } = string.Empty;

    /// <summary>
    /// The quantity of this item being purchased.
    /// </summary>
    [JsonPropertyName("quantity")]
    public int Quantity { get; init; } = 1;

    /// <summary>
    /// Optional price override for this checkout item.
    /// </summary>
    [JsonPropertyName("pricing")]
    public CheckoutItemPricingOverrideRequest? Pricing { get; init; }
}

/// <summary>
/// Price override for a product cart item.
/// </summary>
public sealed record CheckoutItemPricingOverrideRequest
{
    /// <summary>
    /// Pricing mode: 'fixed' or 'custom'.
    /// </summary>
    [JsonPropertyName("price_type")]
    public string? PriceType { get; init; }

    /// <summary>
    /// Fixed amount in the product's primary currency (e.g., "19.00").
    /// </summary>
    [JsonPropertyName("amount")]
    public Price? Amount { get; init; }

    /// <summary>
    /// Currency code (e.g., "USD").
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; init; }

    /// <summary>
    /// Suggested price shown prefilled for custom/pay-what-you-want pricing.
    /// </summary>
    [JsonPropertyName("preset_amount")]
    public Price? PresetAmount { get; init; }

    /// <summary>
    /// Lower bound for custom pricing.
    /// </summary>
    [JsonPropertyName("minimum_amount")]
    public Price? MinimumAmount { get; init; }

    /// <summary>
    /// Upper bound for custom pricing.
    /// </summary>
    [JsonPropertyName("maximum_amount")]
    public Price? MaximumAmount { get; init; }
}

/// <summary>
/// Ad-hoc pricing request for dynamic checkout amounts without catalog products.
/// </summary>
public sealed record CheckoutPricingRequest
{
    /// <summary>
    /// The currency of the charge. Default is "USD".
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; init; } = "USD";

    /// <summary>
    /// The charge amount (e.g., "42.00").
    /// </summary>
    [JsonPropertyName("amount")]
    public Price Amount { get; init; }

    /// <summary>
    /// Explicit price overrides per local currency.
    /// </summary>
    [JsonPropertyName("currency_options")]
    public IDictionary<string, Price>? CurrencyOptions { get; init; }
}

/// <summary>
/// Customer information attached when creating a checkout session.
/// </summary>
public sealed record CheckoutCustomerRequest
{
    /// <summary>
    /// ID of an existing customer (starts with 'cust_').
    /// </summary>
    [JsonPropertyName("customer_id")]
    public string? CustomerId { get; init; }

    /// <summary>
    /// Email of a new customer.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    /// <summary>
    /// Full name of a new customer.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Phone number in E.164 format.
    /// </summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; init; }
}
