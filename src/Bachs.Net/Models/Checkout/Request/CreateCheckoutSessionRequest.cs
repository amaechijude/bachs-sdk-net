using System.Text.Json.Serialization;
using Bachs.Net.Models.Common;

namespace Bachs.Net.Models.Checkout.Request;

/// <summary>
/// Request parameters to create a new checkout session.
/// Provide either <see cref="ProductCart"/> or <see cref="Pricing"/>, never both.
/// Use <see cref="CreateCheckoutSessionRequestBuilder"/>, <see cref="ProductCartCheckoutRequestBuilder"/>,
/// or <see cref="RawPricingCheckoutRequestBuilder"/> to safely construct this request.
/// </summary>
public sealed record CreateCheckoutSessionRequest
{
    /// <summary>
    /// One or more catalog products being purchased (min 1, max 20).
    /// Mutually exclusive with <see cref="Pricing"/>.
    /// </summary>
    [JsonPropertyName("product_cart")]
    public IList<CheckoutCartItemRequest>? ProductCart { get; init; }

    /// <summary>
    /// Raw ad-hoc pricing for a product-less (pure) checkout.
    /// Mutually exclusive with <see cref="ProductCart"/>.
    /// </summary>
    [JsonPropertyName("pricing")]
    public CheckoutPricingRequest? Pricing { get; init; }

    /// <summary>
    /// The customer details or customer ID. Required for recurring checkout.
    /// </summary>
    [JsonPropertyName("customer")]
    public CheckoutCustomerRequest? Customer { get; init; }

    /// <summary>
    /// The URL to redirect the customer to upon successful payment completion.
    /// Bachs appends '?checkout_id=&lt;id&gt;'.
    /// </summary>
    [JsonPropertyName("success_url")]
    public string? SuccessUrl { get; init; }

    /// <summary>
    /// The URL to redirect the customer to if they cancel or abandon the checkout.
    /// </summary>
    [JsonPropertyName("cancel_url")]
    public string? CancelUrl { get; init; }

    /// <summary>
    /// Deprecated alias for <see cref="SuccessUrl"/>, kept for backward compatibility.
    /// If both are set, <see cref="SuccessUrl"/> wins.
    /// </summary>
    [JsonPropertyName("return_url")]
    public string? ReturnUrl { get; init; }

    /// <summary>
    /// Optional checkout billing currency (e.g., Currency.USD, Currency.NGN).
    /// If omitted, defaults to product pricing currency.
    /// </summary>
    [JsonPropertyName("billing_currency")]
    public BachsCurrency? BillingCurrency { get; init; }

    /// <summary>
    /// Restricts checkout to specific payment methods (e.g. 'USD_CARD', 'NGN_BANK_TRANSFER', 'MOMO_GHS', 'CRYPTO').
    /// </summary>
    [JsonPropertyName("payment_method_types")]
    public IList<string>? PaymentMethodTypes { get; init; }

    /// <summary>
    /// Merchant's own reference for this session, unique per account (max 128 characters).
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    /// The platform's cut of this sale in base currency, taken from merchant's proceeds.
    /// Mutually exclusive with <see cref="CheckoutTransferDataRequest.Amount"/>.
    /// </summary>
    [JsonPropertyName("platform_fee")]
    public BachsPrice? PlatformFee { get; init; }

    /// <summary>
    /// Names the connected account this checkout pays out to for destination charges.
    /// </summary>
    [JsonPropertyName("transfer_data")]
    public CheckoutTransferDataRequest? TransferData { get; init; }

    /// <summary>
    /// Decides whether collecting customer identity on the hosted page creates a customer record.
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
    /// Whether to save the customer's card for subsequent off-session charges.
    /// </summary>
    [JsonPropertyName("save_payment_method")]
    public bool? SavePaymentMethod { get; init; }

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
    public required string ProductId { get; init; }

    /// <summary>
    /// The quantity of this item being purchased (minimum 1). Defaults to 1.
    /// </summary>
    [JsonPropertyName("quantity")]
    public int Quantity { get; init; } = 1;

    /// <summary>
    /// Chosen amount for a pay-what-you-want catalog product or custom ad-hoc price.
    /// </summary>
    [JsonPropertyName("amount")]
    public BachsPrice? Amount { get; init; }

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
    /// Pricing mode: 'fixed', 'custom', or 'free'. Defaults to 'fixed'.
    /// </summary>
    [JsonPropertyName("price_type")]
    public string? PriceType { get; init; }

    /// <summary>
    /// Fixed amount in the product's primary currency (e.g., "19.00").
    /// </summary>
    [JsonPropertyName("amount")]
    public BachsPrice? Amount { get; init; }

    /// <summary>
    /// Currency code (e.g., Currency.USD).
    /// </summary>
    [JsonPropertyName("currency")]
    public BachsCurrency? Currency { get; init; }

    /// <summary>
    /// Suggested price shown prefilled for custom/pay-what-you-want pricing.
    /// </summary>
    [JsonPropertyName("preset_amount")]
    public BachsPrice? PresetAmount { get; init; }

    /// <summary>
    /// Lower bound for custom pricing.
    /// </summary>
    [JsonPropertyName("minimum_amount")]
    public BachsPrice? MinimumAmount { get; init; }

    /// <summary>
    /// Upper bound for custom pricing.
    /// </summary>
    [JsonPropertyName("maximum_amount")]
    public BachsPrice? MaximumAmount { get; init; }
}

/// <summary>
/// Ad-hoc pricing request for dynamic checkout amounts without catalog products.
/// </summary>
public sealed record CheckoutPricingRequest
{
    /// <summary>
    /// The currency of the charge (e.g. Currency.USD, Currency.NGN). Defaults to Currency.USD.
    /// </summary>
    [JsonPropertyName("currency")]
    public required BachsCurrency Currency { get; init; } = BachsCurrency.USD;

    /// <summary>
    /// The charge amount (e.g., "42.00"). Required for fixed pricing; omit for custom/free pricing.
    /// </summary>
    [JsonPropertyName("amount")]
    public BachsPrice? Amount { get; init; }

    /// <summary>
    /// Pricing mode: 'fixed' (default when amount is set), 'custom', or 'free'.
    /// </summary>
    [JsonPropertyName("price_type")]
    public string? PriceType { get; init; }

    /// <summary>
    /// Suggested starting amount for a custom price.
    /// </summary>
    [JsonPropertyName("preset_amount")]
    public BachsPrice? PresetAmount { get; init; }

    /// <summary>
    /// Lower bound for a custom price.
    /// </summary>
    [JsonPropertyName("minimum_amount")]
    public BachsPrice? MinimumAmount { get; init; }

    /// <summary>
    /// Upper bound for a custom price.
    /// </summary>
    [JsonPropertyName("maximum_amount")]
    public BachsPrice? MaximumAmount { get; init; }

    /// <summary>
    /// Explicit price overrides per local fiat currency code.
    /// </summary>
    [JsonPropertyName("currency_options")]
    public IDictionary<string, BachsPrice>? CurrencyOptions { get; init; }
}

/// <summary>
/// Destination account transfer information for split payments / destination charges.
/// </summary>
public sealed record CheckoutTransferDataRequest
{
    /// <summary>
    /// The connected account ID to transfer the sale proceeds to (starts with 'acct_').
    /// </summary>
    [JsonPropertyName("destination")]
    public required string Destination { get; init; }

    /// <summary>
    /// What the destination account receives in base currency.
    /// Mutually exclusive with <see cref="CreateCheckoutSessionRequest.PlatformFee"/>.
    /// </summary>
    [JsonPropertyName("amount")]
    public BachsPrice? Amount { get; init; }
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
