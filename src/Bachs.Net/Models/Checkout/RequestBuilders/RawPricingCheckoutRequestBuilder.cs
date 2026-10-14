using Bachs.Net.Common;
using Bachs.Net.Models.Checkout.Request;
using Bachs.Net.Models.Common;

namespace Bachs.Net.Models.Checkout;

/// <summary>
/// Builder for constructing <see cref="CreateCheckoutSessionRequest"/> using raw ad-hoc pricing.
/// </summary>
public sealed class RawPricingCheckoutRequestBuilder
    : CheckoutSessionRequestBuilder<RawPricingCheckoutRequestBuilder>
{
    private CheckoutPricingRequest? _pricing;

    /// <summary>
    /// Sets the pricing configuration.
    /// </summary>
    public RawPricingCheckoutRequestBuilder WithPricing(CheckoutPricingRequest pricing)
    {
        _pricing = pricing;
        return this;
    }

    /// <summary>
    /// Sets the charge amount and currency.
    /// </summary>
    public RawPricingCheckoutRequestBuilder WithPricing(BachsPrice amount, BachsCurrency currency)
    {
        _pricing = new CheckoutPricingRequest { Amount = amount, Currency = currency };
        return this;
    }

    /// <summary>
    /// Sets the pricing mode ('fixed', 'custom', or 'free').
    /// </summary>
    public RawPricingCheckoutRequestBuilder WithPriceType(string priceType)
    {
        ArgumentNullException.ThrowIfNull(_pricing);
        _pricing = _pricing with { PriceType = priceType };
        return this;
    }

    /// <summary>
    /// Sets the suggested starting amount for a custom price.
    /// </summary>
    public RawPricingCheckoutRequestBuilder WithPresetAmount(BachsPrice presetAmount)
    {
        ArgumentNullException.ThrowIfNull(_pricing);
        _pricing = _pricing with { PresetAmount = presetAmount };
        return this;
    }

    /// <summary>
    /// Sets custom pricing bounds.
    /// </summary>
    public RawPricingCheckoutRequestBuilder WithPriceBounds(
        BachsPrice? minimumAmount,
        BachsPrice? maximumAmount
    )
    {
        ArgumentNullException.ThrowIfNull(_pricing);
        _pricing = _pricing with { MinimumAmount = minimumAmount, MaximumAmount = maximumAmount };
        return this;
    }

    /// <summary>
    /// Adds or updates a currency-specific pricing override.
    /// </summary>
    public RawPricingCheckoutRequestBuilder WithCurrencyOption(
        BachsCurrency currency,
        BachsPrice amount
    )
    {
        var options =
            _pricing?.CurrencyOptions != null
                ? new Dictionary<string, BachsPrice>(_pricing.CurrencyOptions)
                : [];
        options[currency.ToString()] = amount;
        ArgumentNullException.ThrowIfNull(_pricing);
        _pricing = _pricing with { CurrencyOptions = options };
        return this;
    }

    /// <summary>
    /// Sets currency-specific pricing overrides.
    /// </summary>
    public RawPricingCheckoutRequestBuilder WithCurrencyOptions(
        IDictionary<string, BachsPrice> currencyOptions
    )
    {
        ArgumentNullException.ThrowIfNull(_pricing);
        _pricing = _pricing with { CurrencyOptions = currencyOptions };
        return this;
    }

    /// <summary>
    /// Validates raw pricing constraints and builds the checkout request.
    /// </summary>
    public override CreateCheckoutSessionRequest Build()
    {
        if (_pricing == null)
            throw new BachsInvalidOperationException(
                "A raw pricing checkout session requires pricing configuration."
            );

        return new CreateCheckoutSessionRequest
        {
            Pricing = _pricing,
            ProductCart = null,
            Customer = CustomerRequest,
            SuccessUrl = SuccessUrlString,
            CancelUrl = CancelUrlString,
            ReturnUrl = ReturnUrlString,
            BillingCurrency = BillingCurrencyValue,
            PaymentMethodTypes = PaymentMethodTypesList,
            Reference = ReferenceString,
            PlatformFee = PlatformFeeAmount,
            TransferData = TransferDataRequest,
            CustomerCreation = CustomerCreationString,
            ExpiresInMinutes = ExpiresInMinutesValue,
            SavePaymentMethod = SavePaymentMethodValue,
            Metadata = MetadataDictionary,
        };
    }
}
