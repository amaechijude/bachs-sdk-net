using System.Text.Json.Serialization;
using Bachs.Net.Common;
using Bachs.Net.Models.Common;

namespace Bachs.Net.Models.Payments;

/// <summary>
/// Represents a payment or charge collected in Bachs.
/// </summary>
public sealed record Payment
{
    private readonly string? _id;
    private readonly string? _chargeId;

    /// <summary>
    /// Unique identifier for the payment (starts with 'ch_').
    /// </summary>
    [JsonPropertyName("id")]
    public string Id
    {
        get => !string.IsNullOrEmpty(_id) ? _id : _chargeId ?? string.Empty;
        init => _id = value;
    }

    /// <summary>
    /// Alias for <see cref="Id"/>, also mapped to 'charge_id' in webhook events.
    /// </summary>
    [JsonPropertyName("charge_id")]
    public string ChargeId
    {
        get => !string.IsNullOrEmpty(_chargeId) ? _chargeId : _id ?? string.Empty;
        init => _chargeId = value;
    }

    /// <summary>
    /// The payment amount as a Price.
    /// </summary>
    [JsonPropertyName("amount")]
    public BachsPrice Amount { get; init; }

    /// <summary>
    /// Currency code (e.g., Currency.USD, Currency.NGN).
    /// </summary>
    [JsonPropertyName("currency")]
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;

    /// <summary>
    /// Status of the payment (e.g., 'SUCCEEDED', 'PROCESSING', 'FAILED', 'UNDERPAID').
    /// </summary>
    [JsonPropertyName("status")]
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// ID of the customer who made the payment.
    /// </summary>
    [JsonPropertyName("customer_id")]
    public string? CustomerId { get; init; }

    /// <summary>
    /// ID of the checkout session that created this payment, if applicable.
    /// </summary>
    [JsonPropertyName("checkout_id")]
    public string? CheckoutId { get; init; }

    /// <summary>
    /// Payment method used (e.g., 'card', 'bank_transfer', 'momo').
    /// </summary>
    [JsonPropertyName("payment_method")]
    public string? PaymentMethod { get; init; }

    /// <summary>
    /// Processing fees deducted from the payment.
    /// </summary>
    [JsonPropertyName("fees")]
    public PaymentFees? Fees { get; init; }

    /// <summary>
    /// Amount settled to the merchant.
    /// </summary>
    [JsonPropertyName("settlement_amount")]
    public BachsPrice? SettlementAmount { get; init; }

    /// <summary>
    /// Settlement currency code.
    /// </summary>
    [JsonPropertyName("settlement_currency")]
    public BachsCurrency? SettlementCurrency { get; init; }

    /// <summary>
    /// Processing fee charged on the payment.
    /// </summary>
    [JsonPropertyName("processing_fee")]
    public BachsPrice? ProcessingFee { get; init; }

    /// <summary>
    /// Processing fee currency code.
    /// </summary>
    [JsonPropertyName("processing_fee_currency")]
    public BachsCurrency? ProcessingFeeCurrency { get; init; }

    /// <summary>
    /// Who bears the processing fee ('merchant' or 'customer').
    /// </summary>
    [JsonPropertyName("fee_bearer")]
    public string? FeeBearer { get; init; }

    /// <summary>
    /// Total amount refunded against this payment so far.
    /// </summary>
    [JsonPropertyName("refunded_amount")]
    public BachsPrice? RefundedAmount { get; init; }

    /// <summary>
    /// Whether this payment is eligible for a refund.
    /// </summary>
    [JsonPropertyName("is_refundable")]
    public bool IsRefundable { get; init; }

    /// <summary>
    /// External reference passed during payment or checkout creation.
    /// </summary>
    [JsonPropertyName("reference")]
    public string? Reference { get; init; }

    /// <summary>
    /// Custom key-value pairs stored on the payment.
    /// </summary>
    [JsonPropertyName("metadata")]
    public IReadOnlyDictionary<string, string>? Metadata { get; init; }

    /// <summary>
    /// When the payment was created.
    /// </summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }
}

/// <summary>
/// Breakdown of fees on a payment.
/// </summary>
public sealed record PaymentFees
{
    [JsonPropertyName("amount")]
    public BachsPrice Amount { get; init; }

    [JsonPropertyName("currency")]
    public BachsCurrency Currency { get; init; } = BachsCurrency.USD;
}

/// <summary>
/// Options for listing payments.
/// </summary>
public sealed class ListPaymentsOptions : ListRequestOptions
{
    /// <summary>
    /// Filter by customer ID.
    /// </summary>
    public string? CustomerId { get; set; }

    /// <summary>
    /// Filter by payment status.
    /// </summary>
    public string? Status { get; set; }

    public override IDictionary<string, string> ToQueryParameters()
    {
        var parameters = base.ToQueryParameters();
        if (!string.IsNullOrWhiteSpace(CustomerId))
        {
            parameters["customer_id"] = CustomerId;
        }
        if (!string.IsNullOrWhiteSpace(Status))
        {
            parameters["status"] = Status;
        }
        return parameters;
    }
}
