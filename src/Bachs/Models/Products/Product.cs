using System.Text.Json.Serialization;
using Bachs.Common;
using Bachs.Models.Common;

namespace Bachs.Models.Products;

/// <summary>
/// Represents a Bachs catalog product.
/// </summary>
public sealed record Product
{
    /// <summary>
    /// Unique identifier for the product (starts with 'prod_').
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Alias for <see cref="Id"/>.
    /// </summary>
    [JsonIgnore]
    public string ProductId => Id;

    /// <summary>
    /// Name/title of the product.
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
    public ProductPrice Price { get; init; } = new();

    /// <summary>
    /// Recurring billing cadence if this is a subscription product.
    /// </summary>
    [JsonPropertyName("billing_cycle")]
    public ProductBillingCycle? BillingCycle { get; init; }

    /// <summary>
    /// Free trial period before first charge on recurring products.
    /// </summary>
    [JsonPropertyName("trial_period")]
    public ProductTrialPeriod? TrialPeriod { get; init; }

    /// <summary>
    /// Whether the product is archived and no longer available for new purchases.
    /// </summary>
    [JsonPropertyName("is_archived")]
    public bool IsArchived { get; init; }

    /// <summary>
    /// Custom key-value pairs stored with this product.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// When the product was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// When the product was last updated.
    /// </summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset? UpdatedAt { get; init; }
}

/// <summary>
/// Pricing configuration for a product.
/// </summary>
public sealed record ProductPrice
{
    /// <summary>
    /// The pricing type: 'fixed', 'free', or 'custom'.
    /// </summary>
    [JsonPropertyName("price_type")]
    public string PriceType { get; init; } = "fixed";

    /// <summary>
    /// The charge amount for fixed pricing (e.g., "12.00").
    /// </summary>
    [JsonPropertyName("amount")]
    public BachsPrice? Amount { get; init; }

    /// <summary>
    /// The primary currency of the product (e.g., Currency.USD).
    /// </summary>
    [JsonPropertyName("currency")]
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;

    /// <summary>
    /// Suggested price shown prefilled for custom pricing.
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

    /// <summary>
    /// Price overrides for specific local currencies.
    /// </summary>
    [JsonPropertyName("currency_options")]
    public IReadOnlyDictionary<string, BachsPrice>? CurrencyOptions { get; init; }
}

/// <summary>
/// Recurring cadence for a subscription product.
/// </summary>
public sealed record ProductBillingCycle
{
    /// <summary>
    /// Cadence unit: 'day', 'week', 'month', or 'year'.
    /// </summary>
    [JsonPropertyName("interval")]
    public string Interval { get; init; } = "month";

    /// <summary>
    /// Number of intervals per cycle (e.g., 1 for monthly, 3 for quarterly).
    /// </summary>
    [JsonPropertyName("frequency")]
    public int Frequency { get; init; } = 1;
}

/// <summary>
/// Free trial period on a subscription product.
/// </summary>
public sealed record ProductTrialPeriod
{
    /// <summary>
    /// Cadence unit: 'day', 'week', 'month', or 'year'.
    /// </summary>
    [JsonPropertyName("interval")]
    public string Interval { get; init; } = "day";

    /// <summary>
    /// Number of intervals in trial period (e.g., 14 for 14-day trial).
    /// </summary>
    [JsonPropertyName("frequency")]
    public int Frequency { get; init; } = 14;
}

/// <summary>
/// Options for listing products.
/// </summary>
public sealed class ListProductsOptions : BachsListRequestOptions
{
    /// <summary>
    /// Whether to include archived products in the response. Defaults to false.
    /// </summary>
    public bool? IncludeArchived { get; set; }

    public override IDictionary<string, string> ToQueryParameters()
    {
        var parameters = base.ToQueryParameters();
        if (IncludeArchived.HasValue)
        {
            parameters["include_archived"] = IncludeArchived.Value ? "true" : "false";
        }
        return parameters;
    }
}
