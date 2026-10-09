using System.Text.Json.Serialization;

namespace Bachs.Net.Models.Checkout;

/// <summary>
/// Request parameters to create a new checkout session.
/// Provide either <see cref="ProductCart"/> or <see cref="Pricing"/>, never both.
/// </summary>
public class CreateCheckoutSessionRequest
{
    /// <summary>
    /// One or more catalog products being purchased.
    /// Provide this OR <see cref="Pricing"/>, not both.
    /// </summary>
    [JsonPropertyName("product_cart")]
    public IList<CheckoutCartItemRequest>? ProductCart { get; set; }

    /// <summary>
    /// Raw ad-hoc pricing with no catalog product.
    /// Provide this OR <see cref="ProductCart"/>, not both.
    /// </summary>
    [JsonPropertyName("pricing")]
    public CheckoutPricingRequest? Pricing { get; set; }

    /// <summary>
    /// The customer details or customer ID. Required for recurring products.
    /// </summary>
    [JsonPropertyName("customer")]
    public CheckoutCustomerRequest? Customer { get; set; }

    /// <summary>
    /// The URL to redirect the customer to upon successful payment completion.
    /// '?checkout_id=' will be appended to this URL by Bachs.
    /// </summary>
    [JsonPropertyName("success_url")]
    public string? SuccessUrl { get; set; }

    /// <summary>
    /// The URL to redirect the customer to if they cancel or abandon the checkout.
    /// </summary>
    [JsonPropertyName("cancel_url")]
    public string? CancelUrl { get; set; }

    /// <summary>
    /// Locks the session to a specific currency (e.g., "USD", "NGN").
    /// </summary>
    [JsonPropertyName("billing_currency")]
    public string? BillingCurrency { get; set; }

    /// <summary>
    /// Corridors to restrict available payment methods to (e.g. 'USD_CARD', 'NGN_BANK_TRANSFER', 'MOMO_GHS').
    /// </summary>
    [JsonPropertyName("payment_method_types")]
    public IList<string>? PaymentMethodTypes { get; set; }

    /// <summary>
    /// External order or charge ID reference (unique per organization, max 128 characters).
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; set; }

    /// <summary>
    /// Decides whether collecting customer identity on the checkout page creates a customer record.
    /// Values: "if_required" (default) or "always".
    /// </summary>
    [JsonPropertyName("customer_creation")]
    public string? CustomerCreation { get; set; }

    /// <summary>
    /// Session lifetime in minutes (1 to 1440). Defaults to 60.
    /// </summary>
    [JsonPropertyName("expires_in_minutes")]
    public int? ExpiresInMinutes { get; set; }

    /// <summary>
    /// Custom key-value pairs (up to 20 keys, max 10 KB). Returned in webhooks.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IDictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Cart item for checkout session creation.
/// </summary>
public class CheckoutCartItemRequest
{
    /// <summary>
    /// The ID of the catalog product (starts with 'prod_').
    /// </summary>
    [JsonPropertyName("product_id")]
    public string ProductId { get; set; } = string.Empty;

    /// <summary>
    /// The quantity of this item being purchased.
    /// </summary>
    [JsonPropertyName("quantity")]
    public int Quantity { get; set; } = 1;

    /// <summary>
    /// Optional price override for this checkout item.
    /// </summary>
    [JsonPropertyName("pricing")]
    public CheckoutItemPricingOverrideRequest? Pricing { get; set; }
}

/// <summary>
/// Price override for a product cart item.
/// </summary>
public class CheckoutItemPricingOverrideRequest
{
    /// <summary>
    /// Pricing mode: 'fixed' or 'custom'.
    /// </summary>
    [JsonPropertyName("price_type")]
    public string? PriceType { get; set; }

    /// <summary>
    /// Fixed amount in the product's primary currency (e.g., "19.00").
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal? Amount { get; set; }

    /// <summary>
    /// Currency code (e.g., "USD").
    /// </summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }

    /// <summary>
    /// Suggested price shown prefilled for custom/pay-what-you-want pricing.
    /// </summary>
    [JsonPropertyName("preset_amount")]
    public decimal? PresetAmount { get; set; }

    /// <summary>
    /// Lower bound for custom pricing.
    /// </summary>
    [JsonPropertyName("minimum_amount")]
    public decimal? MinimumAmount { get; set; }

    /// <summary>
    /// Upper bound for custom pricing.
    /// </summary>
    [JsonPropertyName("maximum_amount")]
    public decimal? MaximumAmount { get; set; }
}

/// <summary>
/// Ad-hoc pricing request for dynamic checkout amounts without catalog products.
/// </summary>
public class CheckoutPricingRequest
{
    /// <summary>
    /// The currency of the charge (e.g. "USD").
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;

    /// <summary>
    /// The charge amount (e.g., "42.00").
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Explicit price overrides per local currency.
    /// </summary>
    [JsonPropertyName("currency_options")]
    public IDictionary<string, decimal>? CurrencyOptions { get; set; }
}

/// <summary>
/// Customer information attached when creating a checkout session.
/// </summary>
public class CheckoutCustomerRequest
{
    /// <summary>
    /// ID of an existing customer (starts with 'cust_').
    /// </summary>
    [JsonPropertyName("customer_id")]
    public string? CustomerId { get; set; }

    /// <summary>
    /// Email of a new customer.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Full name of a new customer.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Phone number in E.164 format.
    /// </summary>
    [JsonPropertyName("phone_number")]
    public string? PhoneNumber { get; set; }
}
