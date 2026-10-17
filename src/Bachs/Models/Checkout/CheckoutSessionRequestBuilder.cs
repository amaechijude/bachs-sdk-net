using Bachs.Models.Common;

namespace Bachs.Models.Checkout;

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
    protected BachsPrice? PlatformFeeAmount;
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
    /// Sets the customer email for the checkout session. This is a convenience method for creating a new customer with just an email address.
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    public TBuilder WithCustomer(string email)
    {
        CustomerRequest = new CheckoutCustomerRequest { Email = email };
        return Self;
    }

    /// <summary>
    /// Sets the customer information for the checkout session, including email, name, and optional phone number. This is a convenience method for creating a new customer with basic contact details.
    /// </summary>
    /// <param name="email"></param>
    /// <param name="name"></param>
    /// <param name="phoneNumber"></param>
    /// <returns></returns>
    public TBuilder WithCustomer(string email, string name, string? phoneNumber = null)
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
    public TBuilder WithPlatformFee(BachsPrice platformFee)
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
    public TBuilder WithTransferData(string destination, BachsPrice? amount = null)
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
