using Bachs.Common;
using Bachs.Exceptions;
using Bachs.Models.Refunds;

namespace Bachs.Clients;

/// <summary>
/// Client for issuing and tracking refunds.
/// </summary>
public interface IRefundsClient
{
    /// <summary>
    /// Issues a full or partial refund against a payment.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when the request body is <see langword="null"/>.</exception>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<Refund>> CreateAsync(
        CreateRefundRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a refund by its ID.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the identifier argument is null, empty, or whitespace.</exception>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<Refund>> GetAsync(
        string refundId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists refunds with pagination and filtering.
    /// </summary>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<BachsList<Refund>>> ListAsync(
        ListRefundsOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves refunds associated with a specific charge/payment ID.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the identifier argument is null, empty, or whitespace.</exception>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<BachsList<Refund>>> GetByPaymentIdAsync(
        string paymentId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
