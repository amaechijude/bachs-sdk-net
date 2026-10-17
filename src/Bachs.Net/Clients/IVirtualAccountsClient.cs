using Bachs.Net.Common;
using Bachs.Net.Exceptions;
using Bachs.Net.Models.Common;
using Bachs.Net.Models.VirtualAccounts;

namespace Bachs.Net.Clients;

/// <summary>
/// Client for managing fixed virtual bank accounts for deposit collections.
/// </summary>
public interface IVirtualAccountsClient
{
    /// <summary>
    /// Creates a fixed virtual account number for continuous collections.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when the request body is <see langword="null"/>.</exception>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<VirtualAccount>> CreateAsync(
        CreateVirtualAccountRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves the fixed virtual bank account for your account or connected account.
    /// </summary>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<VirtualAccount>> GetAsync(
        BachsCurrency currency = BachsCurrency.NGN,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
