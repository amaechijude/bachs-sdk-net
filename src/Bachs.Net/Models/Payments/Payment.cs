using System.Text.Json.Serialization;
using Bachs.Net.Common;

namespace Bachs.Net.Models.Payments;

/// <summary>
/// Represents a payment or charge collected in Bachs.
/// </summary>
public sealed record Payment
{
    /// <summary>
    /// Unique identifier for the payment (starts with 'ch_').
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// Alias for <see cref="Id"/>.
    /// </summary>
    [JsonIgnore]
    public string ChargeId => Id;

    /// <summary>
    /// The payment amount as a decimal.
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal Amount { get; init; }

    /// <summary>
    /// Currency code (e.g., "USD", "NGN").
    /// </summary>
    [JsonPropertyName("currency")]
    public string Currency { get; init; } = string.Empty;

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
    /// Total amount refunded against this payment so far.
    /// </summary>
    [JsonPropertyName("refunded_amount")]
    public decimal? RefundedAmount { get; init; }

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
    public decimal Amount { get; init; }

    [JsonPropertyName("currency")]
    public string Currency { get; init; } = string.Empty;
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
