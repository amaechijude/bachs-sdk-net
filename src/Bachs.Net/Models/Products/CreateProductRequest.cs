using System.Text.Json.Serialization;
using Bachs.Net.Models.Common;
using Bachs.Net.Serialization;

namespace Bachs.Net.Models.Products;

/// <summary>
/// Parameters for creating a new product.
/// </summary>
public sealed record CreateProductRequest
{
    /// <summary>
    /// The name of the product shown to customers at checkout.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Description of the product.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Pricing configuration for this product.
    /// </summary>
    [JsonPropertyName("price")]
    public ProductPriceRequest Price { get; init; } = new();

    /// <summary>
    /// Cadence to make this product recurring. Omit for a one-time product.
    /// </summary>
    [JsonPropertyName("billing_cycle")]
    public ProductBillingCycleRequest? BillingCycle { get; init; }

    /// <summary>
    /// Free trial period before the first recurring charge.
    /// </summary>
    [JsonPropertyName("trial_period")]
    public ProductTrialPeriodRequest? TrialPeriod { get; init; }

    /// <summary>
    /// Custom key-value pairs stored on the product.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Parameters for updating an existing product.
/// </summary>
public sealed record UpdateProductRequest
{
    /// <summary>
    /// Updated name of the product.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Updated description of the product.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Updated pricing configuration.
    /// </summary>
    [JsonPropertyName("price")]
    public ProductPriceRequest? Price { get; init; }

    /// <summary>
    /// Recurring billing cadence (only settable if not previously set).
    /// </summary>
    [JsonPropertyName("billing_cycle")]
    public ProductBillingCycleRequest? BillingCycle { get; init; }

    /// <summary>
    /// Free trial period (only settable if not previously set).
    /// </summary>
    [JsonPropertyName("trial_period")]
    public ProductTrialPeriodRequest? TrialPeriod { get; init; }

    /// <summary>
    /// Custom key-value pairs.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IDictionary<string, string>? Metadata { get; init; }
}

/// <summary>
/// Pricing parameters when creating or updating a product.
/// </summary>
public sealed record ProductPriceRequest
{
    /// <summary>
    /// Pricing mode: 'fixed' (default), 'free', or 'custom'.
    /// </summary>
    [JsonPropertyName("price_type")]
    public string? PriceType { get; init; }

    /// <summary>
    /// Amount for fixed pricing as a price (e.g. 12.00m).
    /// </summary>
    [JsonPropertyName("amount")]
    public Price? Amount { get; init; }

    /// <summary>
    /// Primary currency code (e.g., Currency.USD). Defaults to Currency.USD.
    /// </summary>
    [JsonPropertyName("currency")]
    public Currency Currency { get; init; } = Currency.USD;

    /// <summary>
    /// Suggested price shown prefilled for custom pricing.
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

    /// <summary>
    /// Local currency explicit price overrides.
    /// </summary>
    [JsonPropertyName("currency_options")]
    public IDictionary<string, Price>? CurrencyOptions { get; init; }
}

/// <summary>
/// Billing cycle parameters.
/// </summary>
public sealed record ProductBillingCycleRequest
{
    /// <summary>
    /// Interval: 'day', 'week', 'month', or 'year'.
    /// </summary>
    [JsonPropertyName("interval")]
    public string Interval { get; init; } = "month";

    /// <summary>
    /// Number of intervals per cycle (e.g. 1).
    /// </summary>
    [JsonPropertyName("frequency")]
    public int Frequency { get; init; } = 1;
}

/// <summary>
/// Trial period parameters.
/// </summary>
public sealed record ProductTrialPeriodRequest
{
    /// <summary>
    /// Interval: 'day', 'week', 'month', or 'year'.
    /// </summary>
    [JsonPropertyName("interval")]
    public string Interval { get; init; } = "day";

    /// <summary>
    /// Number of intervals in trial (e.g. 14).
    /// </summary>
    [JsonPropertyName("frequency")]
    public int Frequency { get; init; } = 14;
}
