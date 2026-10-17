using Bachs.Models.Common;

namespace Bachs.Models.Checkout;

/// <summary>
/// Entry point for creating fluent builders to construct <see cref="CreateCheckoutSessionRequest"/>
/// for either a product catalog cart or raw ad-hoc pricing.
/// </summary>
public static class CreateCheckoutSessionRequestBuilder
{
    /// <summary>
    /// Starts building a checkout session using catalog products.
    /// </summary>
    /// <param name="items">The cart items to include. range: 1-20 items</param>
    /// <returns>A configured <see cref="ProductCartCheckoutRequestBuilder"/>.</returns>
    public static ProductCartCheckoutRequestBuilder WithProductCart(
        IList<CheckoutCartItemRequest> items
    ) => new ProductCartCheckoutRequestBuilder().WithProductCart(items);

    /// <summary>
    /// Starts building a checkout session using catalog products.
    /// </summary>
    /// <param name="items">The cart items to include. range: 1-20 items</param>
    /// <returns>A configured <see cref="ProductCartCheckoutRequestBuilder"/>.</returns>
    public static ProductCartCheckoutRequestBuilder WithProductCart(
        params CheckoutCartItemRequest[] items
    ) => new ProductCartCheckoutRequestBuilder().WithProductCart(items);

    /// <summary>
    /// Starts building a checkout session using raw ad-hoc pricing without catalog products.
    /// </summary>
    /// <param name="amount">The base charge amount.</param>
    /// <param name="currency">The base currency code. Defaults to <see cref="BachsCurrency.USD"/>.</param>
    /// <returns>A configured <see cref="RawPricingCheckoutRequestBuilder"/>.</returns>
    public static RawPricingCheckoutRequestBuilder WithPricing(
        BachsPrice amount,
        BachsCurrency currency
    ) => new RawPricingCheckoutRequestBuilder().WithPricing(amount, currency);
}
