using Bachs.Common;
using Bachs.Exceptions;
using Bachs.Models.Common;

namespace Bachs.Models.Checkout;

/// <summary>
/// Builder for constructing <see cref="CreateCheckoutSessionRequest"/> using catalog product cart items.
/// </summary>
public sealed class ProductCartCheckoutRequestBuilder
    : CheckoutSessionRequestBuilder<ProductCartCheckoutRequestBuilder>
{
    private IList<CheckoutCartItemRequest> _productCart = [];

    /// <summary>
    /// Sets the full list of cart items.
    /// </summary>
    public ProductCartCheckoutRequestBuilder WithProductCart(IList<CheckoutCartItemRequest> items)
    {
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
        _productCart = [.. items];
        return this;
    }

    /// <summary>
    /// Sets the cart items.
    /// </summary>
    public ProductCartCheckoutRequestBuilder WithProductCart(params CheckoutCartItemRequest[] items)
    {
        _productCart = [.. items];
        return this;
    }

    /// <summary>
    /// Adds a product cart item to the cart.
    /// </summary>
    public ProductCartCheckoutRequestBuilder AddProduct(CheckoutCartItemRequest item)
    {
        _productCart.Add(item);
        return this;
    }

    /// <summary>
    /// Adds a catalog product item with quantity and optional ad-hoc pricing overrides.
    /// </summary>
    public ProductCartCheckoutRequestBuilder AddProduct(
        string productId,
        int quantity = 1,
        BachsPrice? amount = null,
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
        if (_productCart is { Count: 0 })
            throw new BachsInvalidOperationException(
                "A product cart checkout session requires at least one product cart item."
            );

        if (_productCart is { Count: > 20 })
            throw new BachsInvalidOperationException(
                "A product cart checkout session cannot contain more than 20 items."
            );

        foreach (var item in _productCart)
        {
            if (string.IsNullOrWhiteSpace(item.ProductId))
                throw new BachsInvalidOperationException(
                    "Each cart item must specify a valid ProductId."
                );

            if (item.Quantity < 1)
                throw new BachsInvalidOperationException(
                    $"Cart item '{item.ProductId}' must have a quantity of at least 1."
                );
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
