using System.Text.Json.Serialization;

namespace Bachs.Net.Models.Products;

/// <summary>
/// Parameters for creating a new product.
/// </summary>
public sealed class CreateProductRequest
{
    /// <summary>
    /// The name of the product shown to customers at checkout.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of the product.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Pricing configuration for this product.
    /// </summary>
    [JsonPropertyName("price")]
    public ProductPriceRequest Price { get; set; } = new();

    /// <summary>
    /// Cadence to make this product recurring. Omit for a one-time product.
    /// </summary>
    [JsonPropertyName("billing_cycle")]
    public ProductBillingCycleRequest? BillingCycle { get; set; }

    /// <summary>
    /// Free trial period before the first recurring charge.
    /// </summary>
    [JsonPropertyName("trial_period")]
    public ProductTrialPeriodRequest? TrialPeriod { get; set; }

    /// <summary>
    /// Custom key-value pairs stored on the product.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IDictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Parameters for updating an existing product.
/// </summary>
public sealed class UpdateProductRequest
{
    /// <summary>
    /// Updated name of the product.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Updated description of the product.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Updated pricing configuration.
    /// </summary>
    [JsonPropertyName("price")]
    public ProductPriceRequest? Price { get; set; }

    /// <summary>
    /// Recurring billing cadence (only settable if not previously set).
    /// </summary>
    [JsonPropertyName("billing_cycle")]
    public ProductBillingCycleRequest? BillingCycle { get; set; }

    /// <summary>
    /// Free trial period (only settable if not previously set).
    /// </summary>
    [JsonPropertyName("trial_period")]
    public ProductTrialPeriodRequest? TrialPeriod { get; set; }

    /// <summary>
    /// Custom key-value pairs.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IDictionary<string, string>? Metadata { get; set; }
}

/// <summary>
/// Pricing parameters when creating or updating a product.
/// </summary>
public sealed class ProductPriceRequest
{
    /// <summary>
    /// Pricing mode: 'fixed' (default), 'free', or 'custom'.
    /// </summary>
    [JsonPropertyName("price_type")]
    public string? PriceType { get; set; }

    /// <summary>
    /// Amount for fixed pricing as a decimal (e.g. 12.00m).
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal? Amount { get; set; }

    /// <summary>
    /// Primary currency code (e.g., "USD").
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = "USD";

    /// <summary>
    /// Suggested price shown prefilled for custom pricing.
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

    /// <summary>
    /// Local currency explicit price overrides.
    /// </summary>
    [JsonPropertyName("currency_options")]
    public IDictionary<string, decimal>? CurrencyOptions { get; set; }
}

/// <summary>
/// Billing cycle parameters.
/// </summary>
public sealed class ProductBillingCycleRequest
{
    /// <summary>
    /// Interval: 'day', 'week', 'month', or 'year'.
    /// </summary>
    [JsonPropertyName("interval")]
    public string Interval { get; set; } = "month";

    /// <summary>
    /// Number of intervals per cycle (e.g. 1).
    /// </summary>
    [JsonPropertyName("frequency")]
    public int Frequency { get; set; } = 1;
}

/// <summary>
/// Trial period parameters.
/// </summary>
public sealed class ProductTrialPeriodRequest
{
    /// <summary>
    /// Interval: 'day', 'week', 'month', or 'year'.
    /// </summary>
    [JsonPropertyName("interval")]
    public string Interval { get; set; } = "day";

    /// <summary>
    /// Number of intervals in trial (e.g. 14).
    /// </summary>
    [JsonPropertyName("frequency")]
    public int Frequency { get; set; } = 14;
}
