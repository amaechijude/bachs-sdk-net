using Bachs.Common;
using Bachs.Exceptions;
using Bachs.Models.Conversions;

namespace Bachs.Clients;

/// <summary>
/// Client for executing currency conversions between balance buckets.
/// </summary>
public interface IConversionsClient
{
    /// <summary>
    /// Locks an exchange rate quote for a conversion attempt.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when the request body is <see langword="null"/>.</exception>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<ConversionQuote>> CreateQuoteAsync(
        CreateConversionQuoteRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes a currency conversion using a previously created quote ID.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when the request body is <see langword="null"/>.</exception>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<Conversion>> ExecuteAsync(
        ExecuteConversionRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a conversion record by its ID.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the identifier argument is null, empty, or whitespace.</exception>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<Conversion>> GetAsync(
        string conversionId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists past conversion records with pagination and filtering.
    /// </summary>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<BachsList<Conversion>>> ListAsync(
        ListConversionsOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
