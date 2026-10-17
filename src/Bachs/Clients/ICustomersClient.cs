using Bachs.Common;
using Bachs.Exceptions;
using Bachs.Models.Customers;

namespace Bachs.Clients;

/// <summary>
/// Client for managing customers and customer portal sessions.
/// </summary>
public interface ICustomersClient
{
    /// <summary>
    /// Creates a new customer record.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when the request body is <see langword="null"/>.</exception>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<Customer>> CreateAsync(
        CreateCustomerRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a customer by their ID.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the identifier argument is null, empty, or whitespace.</exception>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<Customer>> GetAsync(
        string customerId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists customers with pagination and optional search.
    /// </summary>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<BachsList<Customer>>> ListAsync(
        ListCustomersOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates an existing customer record.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the identifier argument is null, empty, or whitespace.</exception>
    /// <exception cref="ArgumentNullException">Thrown when the request body is <see langword="null"/>.</exception>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<Customer>> UpdateAsync(
        string customerId,
        UpdateCustomerRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates a pre-authenticated customer portal session for self-service billing management.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown when the identifier argument is null, empty, or whitespace.</exception>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<CustomerPortalSession>> CreatePortalSessionAsync(
        string customerId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
