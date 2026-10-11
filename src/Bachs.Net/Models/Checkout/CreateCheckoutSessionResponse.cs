using System.Text.Json.Serialization;
using Bachs.Net.Serialization;

namespace Bachs.Net.Models.Checkout;

/// <summary>
/// Response payload returned by POST /v1/checkout-sessions when creating a hosted checkout session.
/// </summary>
public sealed record CreateCheckoutSessionResponse
{
    /// <summary>
    /// Checkout identifier (starts with 'chk_').
    /// </summary>
    [JsonPropertyName("checkout_id")]
    public string CheckoutId { get; init; } = string.Empty;

    /// <summary>
    /// Convenient alias for <see cref="CheckoutId"/>.
    /// </summary>
    [JsonIgnore]
    public string Id => CheckoutId;

    /// <summary>
    /// Hosted checkout URL where the customer can complete payment.
    /// </summary>
    [JsonPropertyName("checkout_url")]
    public string CheckoutUrl { get; init; } = string.Empty;

    /// <summary>
    /// Current checkout lifecycle status ('open', 'completed', 'expired', 'cancelled').
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// ISO 8601 UTC timestamp indicating when the checkout will expire.
    /// </summary>
    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; init; }

    /// <summary>
    /// ISO 8601 UTC timestamp indicating when the checkout was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Merchant's own reference for this checkout session, echoed back unchanged.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    /// The platform's cut of this sale in base currency, echoed back from the request.
    /// Null on a checkout that carries no platform fee.
    /// </summary>
    [JsonPropertyName("platform_fee")]
    public Price? PlatformFee { get; init; }

    /// <summary>
    /// The seller's contracted share of this sale in base currency, echoed back from transfer_data.amount.
    /// Null on a checkout that carries no split or that used platform_fee instead.
    /// </summary>
    [JsonPropertyName("destination_amount")]
    public Price? DestinationAmount { get; init; }
}
