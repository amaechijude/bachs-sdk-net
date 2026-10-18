using System.Text.Json.Serialization;
using Bachs.Models.Common;

namespace Bachs.WebHooks.EventData;

/// <summary>
/// Event data for collection/payment webhook events (<c>collection.succeeded</c>, <c>collection.failed</c>, and <c>collection.underpaid</c>).
/// </summary>
public sealed record CollectionEventData
{
    private readonly string? _id;
    private readonly string? _chargeId;

    /// <summary>
    /// Unique identifier for the charge/payment (starts with 'ch_').
    /// </summary>
    [JsonPropertyName("id")]
    public string Id
    {
        get => !string.IsNullOrEmpty(_id) ? _id : _chargeId ?? string.Empty;
        init => _id = value;
    }

    /// <summary>
    /// Charge identifier in webhook events.
    /// </summary>
    [JsonPropertyName("charge_id")]
    public string ChargeId
    {
        get => !string.IsNullOrEmpty(_chargeId) ? _chargeId : _id ?? string.Empty;
        init => _chargeId = value;
    }

    /// <summary>
    /// Associated checkout identifier, if initiated via checkout.
    /// </summary>
    [JsonPropertyName("checkout_id")]
    public string? CheckoutId { get; init; }

    /// <summary>
    /// External reference for this transaction.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    /// Status of the payment (e.g. 'SUCCEEDED', 'FAILED', 'UNDERPAID').
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Total requested payment amount.
    /// </summary>
    [JsonPropertyName("amount")]
    public BachsPrice? Amount { get; init; }

    /// <summary>
    /// Amount paid so far (populated on underpaid collections).
    /// </summary>
    [JsonPropertyName("amount_paid")]
    public BachsPrice? AmountPaid { get; init; }

    /// <summary>
    /// Expected amount (populated on underpaid collections).
    /// </summary>
    [JsonPropertyName("amount_expected")]
    public BachsPrice? AmountExpected { get; init; }

    /// <summary>
    /// Remaining balance yet to be paid (populated on underpaid collections).
    /// </summary>
    [JsonPropertyName("amount_remaining")]
    public BachsPrice? AmountRemaining { get; init; }

    /// <summary>
    /// Payment currency.
    /// </summary>
    [JsonPropertyName("currency")]
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;

    /// <summary>
    /// Settlement amount credited to the account.
    /// </summary>
    [JsonPropertyName("settlement_amount")]
    public BachsPrice? SettlementAmount { get; init; }

    /// <summary>
    /// Settlement currency.
    /// </summary>
    [JsonPropertyName("settlement_currency")]
    public BachsCurrency? SettlementCurrency { get; init; }

    /// <summary>
    /// Processing fee charged on the transaction.
    /// </summary>
    [JsonPropertyName("processing_fee")]
    public BachsPrice? ProcessingFee { get; init; }

    /// <summary>
    /// Processing fee currency.
    /// </summary>
    [JsonPropertyName("processing_fee_currency")]
    public BachsCurrency? ProcessingFeeCurrency { get; init; }

    /// <summary>
    /// Entity bearing the processing fee ('merchant' or 'customer').
    /// </summary>
    [JsonPropertyName("fee_bearer")]
    public string? FeeBearer { get; init; }

    /// <summary>
    /// Line items / product cart associated with the collection.
    /// </summary>
    [JsonPropertyName("product_cart")]
    public IReadOnlyList<CollectionProductCartItem>? ProductCart { get; init; }

    /// <summary>
    /// Customer details attached to the payment.
    /// </summary>
    [JsonPropertyName("customer")]
    public CollectionCustomerInfo? Customer { get; init; }

    /// <summary>
    /// Payment method rail and transaction details (e.g., bank transfer info).
    /// </summary>
    [JsonPropertyName("payment_method_details")]
    public CollectionPaymentMethodDetails? PaymentMethodDetails { get; init; }

    /// <summary>
    /// Failure reason when status is 'FAILED'.
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    /// <summary>
    /// Custom key-value pairs stored on the transaction.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// When the charge was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset? CreatedAt { get; init; }
}

/// <summary>
/// Line item in a collection event product cart.
/// </summary>
public sealed record CollectionProductCartItem
{
    [JsonPropertyName("product_id")]
    public string ProductId { get; init; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; init; }

    [JsonPropertyName("amount")]
    public BachsPrice? Amount { get; init; }
}

/// <summary>
/// Customer information in a collection event payload.
/// </summary>
public sealed record CollectionCustomerInfo
{
    private readonly string? _id;
    private readonly string? _customerId;

    [JsonPropertyName("id")]
    public string Id
    {
        get => !string.IsNullOrEmpty(_id) ? _id : _customerId ?? string.Empty;
        init => _id = value;
    }

    [JsonPropertyName("customer_id")]
    public string CustomerId
    {
        get => !string.IsNullOrEmpty(_customerId) ? _customerId : _id ?? string.Empty;
        init => _customerId = value;
    }

    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("name")]
    public string? Name { get; init; }
}

/// <summary>
/// Details of the payment method rail used in a collection.
/// </summary>
public sealed record CollectionPaymentMethodDetails
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    [JsonPropertyName("bank_transfer")]
    public CollectionBankTransferDetails? BankTransfer { get; init; }
}

/// <summary>
/// Bank transfer transaction metadata in a collection event payload.
/// </summary>
public sealed record CollectionBankTransferDetails
{
    [JsonPropertyName("sender_name")]
    public string? SenderName { get; init; }

    [JsonPropertyName("sender_bank")]
    public string? SenderBank { get; init; }

    [JsonPropertyName("sender_bank_code")]
    public string? SenderBankCode { get; init; }

    [JsonPropertyName("sender_account_number")]
    public string? SenderAccountNumber { get; init; }

    [JsonPropertyName("session_id")]
    public string? SessionId { get; init; }

    [JsonPropertyName("narration")]
    public string? Narration { get; init; }

    [JsonPropertyName("virtual_account")]
    public CollectionVirtualAccountDetails? VirtualAccount { get; init; }
}

/// <summary>
/// Virtual account information used for collection.
/// </summary>
public sealed record CollectionVirtualAccountDetails
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("account_number")]
    public string? AccountNumber { get; init; }

    [JsonPropertyName("bank_name")]
    public string? BankName { get; init; }

    [JsonPropertyName("type")]
    public string? Type { get; init; }

    [JsonPropertyName("expires_at")]
    public DateTimeOffset? ExpiresAt { get; init; }
}
