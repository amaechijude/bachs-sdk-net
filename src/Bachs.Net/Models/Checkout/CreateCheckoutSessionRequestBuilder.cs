using Bachs.Net.Common;
using Bachs.Net.Models.Common;
using Bachs.Net.Serialization;

namespace Bachs.Net.Models.Checkout;

/// <summary>
/// Entry point for creating fluent builders to construct <see cref="CreateCheckoutSessionRequest"/>
/// for either a product catalog cart or raw ad-hoc pricing.
/// </summary>
public static class CreateCheckoutSessionRequestBuilder
{
    /// <summary>
    /// Starts building a checkout session using catalog products.
    /// </summary>
    /// <param name="items">The cart items to include.</param>
    /// <returns>A configured <see cref="ProductCartCheckoutRequestBuilder"/>.</returns>
    public static ProductCartCheckoutRequestBuilder WithProductCart(
        IList<CheckoutCartItemRequest> items
    ) => new ProductCartCheckoutRequestBuilder().WithProductCart(items);

    /// <summary>
    /// Starts building a checkout session using catalog products.
    /// </summary>
    /// <param name="items">The cart items to include.</param>
    /// <returns>A configured <see cref="ProductCartCheckoutRequestBuilder"/>.</returns>
    public static ProductCartCheckoutRequestBuilder WithProductCart(
        params CheckoutCartItemRequest[] items
    ) => new ProductCartCheckoutRequestBuilder().WithProductCart(items);

    /// <summary>
    /// Starts building a checkout session using catalog products from an empty cart.
    /// </summary>
    /// <returns>A new <see cref="ProductCartCheckoutRequestBuilder"/>.</returns>
    public static ProductCartCheckoutRequestBuilder ForProductCart() => new();

    /// <summary>
    /// Starts building a checkout session using raw ad-hoc pricing without catalog products.
    /// </summary>
    /// <param name="pricing">The pricing configuration.</param>
    /// <returns>A configured <see cref="RawPricingCheckoutRequestBuilder"/>.</returns>
    public static RawPricingCheckoutRequestBuilder WithPricing(CheckoutPricingRequest pricing) =>
        new RawPricingCheckoutRequestBuilder().WithPricing(pricing);

    /// <summary>
    /// Starts building a checkout session using raw ad-hoc pricing without catalog products.
    /// </summary>
    /// <param name="amount">The base charge amount.</param>
    /// <param name="currency">The base currency code. Defaults to <see cref="BachsCurrency.USD"/>.</param>
    /// <returns>A configured <see cref="RawPricingCheckoutRequestBuilder"/>.</returns>
    public static RawPricingCheckoutRequestBuilder WithPricing(
        Price amount,
        BachsCurrency currency = BachsCurrency.USD
    ) => new RawPricingCheckoutRequestBuilder().WithPricing(amount, currency);

    /// <summary>
    /// Starts building a checkout session using raw ad-hoc pricing without catalog products.
    /// </summary>
    /// <param name="amount">The base charge amount.</param>
    /// <param name="currency">The base currency string (e.g., "USD", "NGN").</param>
    /// <returns>A configured <see cref="RawPricingCheckoutRequestBuilder"/>.</returns>
    public static RawPricingCheckoutRequestBuilder WithPricing(Price amount, string currency)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currency);
        return new RawPricingCheckoutRequestBuilder().WithPricing(
            amount,
            Enum.Parse<BachsCurrency>(currency, true)
        );
    }

    /// <summary>
    /// Starts building a checkout session using raw ad-hoc pricing with a specified base currency.
    /// </summary>
    /// <param name="currency">The base currency code. Defaults to <see cref="BachsCurrency.USD"/>.</param>
    /// <returns>A new <see cref="RawPricingCheckoutRequestBuilder"/>.</returns>
    public static RawPricingCheckoutRequestBuilder ForRawPricing(
        BachsCurrency currency = BachsCurrency.USD
    ) => new RawPricingCheckoutRequestBuilder().WithCurrency(currency);

    /// <summary>
    /// Starts building a checkout session using raw ad-hoc pricing with a specified base currency.
    /// </summary>
    /// <param name="currency">The base currency string (e.g. "USD", "NGN").</param>
    /// <returns>A new <see cref="RawPricingCheckoutRequestBuilder"/>.</returns>
    public static RawPricingCheckoutRequestBuilder ForRawPricing(string currency)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(currency);
        return new RawPricingCheckoutRequestBuilder().WithCurrency(
            Enum.Parse<BachsCurrency>(currency, true)
        );
    }
}

/// <summary>
/// Common base builder providing shared options across checkout session creation requests.
/// </summary>
/// <typeparam name="TBuilder">The concrete builder type for fluent method chaining.</typeparam>
public abstract class CheckoutSessionRequestBuilder<TBuilder>
    where TBuilder : CheckoutSessionRequestBuilder<TBuilder>
{
    protected CheckoutCustomerRequest? CustomerRequest;
    protected string? SuccessUrlString;
    protected string? CancelUrlString;
    protected string? ReturnUrlString;
    protected BachsCurrency? BillingCurrencyValue;
    protected IList<string>? PaymentMethodTypesList;
    protected string? ReferenceString;
    protected Price? PlatformFeeAmount;
    protected CheckoutTransferDataRequest? TransferDataRequest;
    protected string? CustomerCreationString;
    protected int? ExpiresInMinutesValue;
    protected bool? SavePaymentMethodValue;
    protected IDictionary<string, string>? MetadataDictionary;

    protected TBuilder Self => (TBuilder)this;

    /// <summary>
    /// Sets the customer information for the checkout session.
    /// </summary>
    public TBuilder WithCustomer(CheckoutCustomerRequest customer)
    {
        CustomerRequest = customer;
        return Self;
    }

    /// <summary>
    /// Sets an existing customer by their customer ID (starts with 'cust_').
    /// </summary>
    public TBuilder WithExistingCustomer(string customerId)
    {
        CustomerRequest = new CheckoutCustomerRequest { CustomerId = customerId };
        return Self;
    }

    /// <summary>
    /// Sets contact details for a new customer.
    /// </summary>
    public TBuilder WithNewCustomer(string email, string? name = null, string? phoneNumber = null)
    {
        CustomerRequest = new CheckoutCustomerRequest
        {
            Email = email,
            Name = name,
            PhoneNumber = phoneNumber,
        };
        return Self;
    }

    /// <summary>
    /// Sets the redirect URL upon successful payment. Bachs appends '?checkout_id=&lt;id&gt;'.
    /// </summary>
    public TBuilder WithSuccessUrl(string successUrl)
    {
        SuccessUrlString = successUrl;
        return Self;
    }

    /// <summary>
    /// Sets the redirect URL if the buyer cancels or abandons checkout.
    /// </summary>
    public TBuilder WithCancelUrl(string cancelUrl)
    {
        CancelUrlString = cancelUrl;
        return Self;
    }

    /// <summary>
    /// Sets the deprecated return URL alias for <see cref="WithSuccessUrl(string)"/>.
    /// </summary>
    public TBuilder WithReturnUrl(string returnUrl)
    {
        ReturnUrlString = returnUrl;
        return Self;
    }

    /// <summary>
    /// Sets the checkout billing currency (e.g. <see cref="BachsCurrency.USD"/>, <see cref="BachsCurrency.NGN"/>).
    /// </summary>
    public TBuilder WithBillingCurrency(BachsCurrency billingCurrency)
    {
        BillingCurrencyValue = billingCurrency;
        return Self;
    }

    // /// <summary>
    // /// Sets the checkout billing currency string (e.g. "USD", "NGN").
    // /// </summary>
    // public TBuilder WithBillingCurrency(string billingCurrency)
    // {
    //     ArgumentException.ThrowIfNullOrWhiteSpace(billingCurrency);
    //     BillingCurrencyValue = Enum.Parse<Currency>(billingCurrency, true);
    //     return Self;
    // }

    /// <summary>
    /// Restricts checkout to specific payment methods (e.g. 'USD_CARD', 'NGN_BANK_TRANSFER', 'MOMO_GHS', 'CRYPTO').
    /// </summary>
    public TBuilder WithPaymentMethodTypes(IList<string> paymentMethodTypes)
    {
        PaymentMethodTypesList = paymentMethodTypes;
        return Self;
    }

    /// <summary>
    /// Restricts checkout to specific payment methods.
    /// </summary>
    public TBuilder WithPaymentMethodTypes(params string[] paymentMethodTypes)
    {
        PaymentMethodTypesList = paymentMethodTypes?.ToList();
        return Self;
    }

    /// <summary>
    /// Sets the merchant's external reference for this session (max 128 characters).
    /// </summary>
    public TBuilder WithReference(string reference)
    {
        ReferenceString = reference;
        return Self;
    }

    /// <summary>
    /// Sets the platform fee in base currency, taken from the merchant's proceeds.
    /// </summary>
    public TBuilder WithPlatformFee(Price platformFee)
    {
        PlatformFeeAmount = platformFee;
        return Self;
    }

    /// <summary>
    /// Sets the transfer destination and split amount for destination charges.
    /// </summary>
    public TBuilder WithTransferData(CheckoutTransferDataRequest transferData)
    {
        TransferDataRequest = transferData;
        return Self;
    }

    /// <summary>
    /// Sets the transfer destination and optional fixed amount for destination charges.
    /// </summary>
    public TBuilder WithTransferData(string destination, Price? amount = null)
    {
        TransferDataRequest = new CheckoutTransferDataRequest
        {
            Destination = destination,
            Amount = amount,
        };
        return Self;
    }

    /// <summary>
    /// Sets whether guest customer creation occurs on the hosted page ('always' or 'if_required').
    /// </summary>
    public TBuilder WithCustomerCreation(string customerCreation)
    {
        CustomerCreationString = customerCreation;
        return Self;
    }

    /// <summary>
    /// Forces creation of a customer record for guest buyers ('always').
    /// </summary>
    public TBuilder WithCustomerCreationAlways()
    {
        CustomerCreationString = "always";
        return Self;
    }

    /// <summary>
    /// Keeps guest buyers out of the customer directory ('if_required', default).
    /// </summary>
    public TBuilder WithCustomerCreationIfRequired()
    {
        CustomerCreationString = "if_required";
        return Self;
    }

    /// <summary>
    /// Sets the session lifetime in minutes (1 to 1440). Defaults to 60.
    /// </summary>
    public TBuilder WithExpiresInMinutes(int minutes)
    {
        ExpiresInMinutesValue = minutes;
        return Self;
    }

    /// <summary>
    /// Sets whether to save the customer's card for subsequent charges.
    /// </summary>
    public TBuilder WithSavePaymentMethod(bool save = true)
    {
        SavePaymentMethodValue = save;
        return Self;
    }

    /// <summary>
    /// Sets custom metadata key-value pairs (up to 20 keys, max 10 KB).
    /// </summary>
    public TBuilder WithMetadata(IDictionary<string, string> metadata)
    {
        MetadataDictionary = metadata;
        return Self;
    }

    /// <summary>
    /// Adds or updates a single metadata key-value entry.
    /// </summary>
    public TBuilder WithMetadata(string key, string value)
    {
        MetadataDictionary ??= new Dictionary<string, string>();
        MetadataDictionary[key] = value;
        return Self;
    }

    /// <summary>
    /// Validates and builds the <see cref="CreateCheckoutSessionRequest"/>.
    /// </summary>
    /// <returns>A validated <see cref="CreateCheckoutSessionRequest"/> instance.</returns>
    public abstract CreateCheckoutSessionRequest Build();
}

/// <summary>
/// Builder for constructing <see cref="CreateCheckoutSessionRequest"/> using catalog product cart items.
/// </summary>
public sealed class ProductCartCheckoutRequestBuilder
    : CheckoutSessionRequestBuilder<ProductCartCheckoutRequestBuilder>
{
    private IList<CheckoutCartItemRequest> _productCart = new List<CheckoutCartItemRequest>();

    /// <summary>
    /// Creates a new instance of <see cref="ProductCartCheckoutRequestBuilder"/>.
    /// </summary>
    public static ProductCartCheckoutRequestBuilder Create() => new();

    /// <summary>
    /// Sets the full list of cart items.
    /// </summary>
    public ProductCartCheckoutRequestBuilder WithProductCart(IList<CheckoutCartItemRequest> items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _productCart = items;
        return this;
    }

    /// <summary>
    /// Sets the cart items.
    /// </summary>
    public ProductCartCheckoutRequestBuilder WithProductCart(
        IEnumerable<CheckoutCartItemRequest> items
    )
    {
        ArgumentNullException.ThrowIfNull(items);
        _productCart = items.ToList();
        return this;
    }

    /// <summary>
    /// Sets the cart items.
    /// </summary>
    public ProductCartCheckoutRequestBuilder WithProductCart(params CheckoutCartItemRequest[] items)
    {
        ArgumentNullException.ThrowIfNull(items);
        _productCart = items.ToList();
        return this;
    }

    /// <summary>
    /// Adds a product cart item to the cart.
    /// </summary>
    public ProductCartCheckoutRequestBuilder AddProduct(CheckoutCartItemRequest item)
    {
        ArgumentNullException.ThrowIfNull(item);
        _productCart.Add(item);
        return this;
    }

    /// <summary>
    /// Adds a catalog product item with quantity and optional ad-hoc pricing overrides.
    /// </summary>
    public ProductCartCheckoutRequestBuilder AddProduct(
        string productId,
        int quantity = 1,
        Price? amount = null,
        CheckoutItemPricingOverrideRequest? pricing = null
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(productId);
        _productCart.Add(
            new CheckoutCartItemRequest
            {
                ProductId = productId,
                Quantity = quantity,
                Amount = amount,
                Pricing = pricing,
            }
        );
        return this;
    }

    /// <summary>
    /// Validates product cart constraints and builds the checkout request.
    /// </summary>
    public override CreateCheckoutSessionRequest Build()
    {
        if (_productCart == null || _productCart.Count == 0)
        {
            throw new BachsInvalidOperationException(
                "A product cart checkout session requires at least one product cart item."
            );
        }

        if (_productCart.Count > 20)
        {
            throw new BachsInvalidOperationException(
                "A product cart checkout session cannot contain more than 20 items."
            );
        }

        foreach (var item in _productCart)
        {
            if (string.IsNullOrWhiteSpace(item.ProductId))
            {
                throw new BachsInvalidOperationException(
                    "Each cart item must specify a valid ProductId."
                );
            }
            if (item.Quantity < 1)
            {
                throw new BachsInvalidOperationException(
                    $"Cart item '{item.ProductId}' must have a quantity of at least 1."
                );
            }
        }

        return new CreateCheckoutSessionRequest
        {
            ProductCart = _productCart,
            Pricing = null,
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

/// <summary>
/// Builder for constructing <see cref="CreateCheckoutSessionRequest"/> using raw ad-hoc pricing.
/// </summary>
public sealed class RawPricingCheckoutRequestBuilder
    : CheckoutSessionRequestBuilder<RawPricingCheckoutRequestBuilder>
{
    private CheckoutPricingRequest? _pricing;

    /// <summary>
    /// Creates a new instance of <see cref="RawPricingCheckoutRequestBuilder"/>.
    /// </summary>
    public static RawPricingCheckoutRequestBuilder Create() => new();

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
    public RawPricingCheckoutRequestBuilder WithPricing(
        Price amount,
        BachsCurrency currency = BachsCurrency.USD
    )
    {
        _pricing = new CheckoutPricingRequest { Amount = amount, Currency = currency };
        return this;
    }

    /// <summary>
    /// Sets the base charge amount.
    /// </summary>
    public RawPricingCheckoutRequestBuilder WithAmount(Price amount)
    {
        _pricing = (_pricing ?? new CheckoutPricingRequest()) with { Amount = amount };
        return this;
    }

    /// <summary>
    /// Sets the base currency code (e.g. <see cref="BachsCurrency.USD"/>).
    /// </summary>
    public RawPricingCheckoutRequestBuilder WithCurrency(BachsCurrency currency)
    {
        _pricing = (_pricing ?? new CheckoutPricingRequest()) with { Currency = currency };
        return this;
    }

    /// <summary>
    /// Sets the pricing mode ('fixed', 'custom', or 'free').
    /// </summary>
    public RawPricingCheckoutRequestBuilder WithPriceType(string priceType)
    {
        _pricing = (_pricing ?? new CheckoutPricingRequest()) with { PriceType = priceType };
        return this;
    }

    /// <summary>
    /// Sets the suggested starting amount for a custom price.
    /// </summary>
    public RawPricingCheckoutRequestBuilder WithPresetAmount(Price presetAmount)
    {
        _pricing = (_pricing ?? new CheckoutPricingRequest()) with { PresetAmount = presetAmount };
        return this;
    }

    /// <summary>
    /// Sets the lower bound for a custom price.
    /// </summary>
    public RawPricingCheckoutRequestBuilder WithMinimumAmount(Price minimumAmount)
    {
        _pricing = (_pricing ?? new CheckoutPricingRequest()) with
        {
            MinimumAmount = minimumAmount,
        };
        return this;
    }

    /// <summary>
    /// Sets the upper bound for a custom price.
    /// </summary>
    public RawPricingCheckoutRequestBuilder WithMaximumAmount(Price maximumAmount)
    {
        _pricing = (_pricing ?? new CheckoutPricingRequest()) with
        {
            MaximumAmount = maximumAmount,
        };
        return this;
    }

    /// <summary>
    /// Sets custom pricing bounds.
    /// </summary>
    public RawPricingCheckoutRequestBuilder WithPriceBounds(
        Price? minimumAmount,
        Price? maximumAmount
    )
    {
        _pricing = (_pricing ?? new CheckoutPricingRequest()) with
        {
            MinimumAmount = minimumAmount,
            MaximumAmount = maximumAmount,
        };
        return this;
    }

    /// <summary>
    /// Adds or updates a currency-specific pricing override.
    /// </summary>
    public RawPricingCheckoutRequestBuilder WithCurrencyOption(
        BachsCurrency currency,
        Price amount
    ) => WithCurrencyOption(currency.ToString(), amount);

    /// <summary>
    /// Adds or updates a currency-specific pricing override.
    /// </summary>
    public RawPricingCheckoutRequestBuilder WithCurrencyOption(string currency, Price amount)
    {
        var options =
            _pricing?.CurrencyOptions != null
                ? new Dictionary<string, Price>(_pricing.CurrencyOptions)
                : [];
        options[currency] = amount;
        _pricing = (_pricing ?? new CheckoutPricingRequest()) with { CurrencyOptions = options };
        return this;
    }

    /// <summary>
    /// Sets currency-specific pricing overrides.
    /// </summary>
    public RawPricingCheckoutRequestBuilder WithCurrencyOptions(
        IDictionary<string, Price> currencyOptions
    )
    {
        _pricing = (_pricing ?? new CheckoutPricingRequest()) with
        {
            CurrencyOptions = currencyOptions,
        };
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
