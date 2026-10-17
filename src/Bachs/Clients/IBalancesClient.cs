using Bachs.Common;
using Bachs.Exceptions;
using Bachs.Models.Balances;

namespace Bachs.Clients;

/// <summary>
/// Client for reading balances and managing payout settings.
/// </summary>
public interface IBalancesClient
{
    /// <summary>
    /// Retrieves organization balance buckets across all currencies.
    /// </summary>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<Balance>> GetBalancesAsync(
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves current automatic payout balance settings.
    /// </summary>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<BalanceSettings>> GetSettingsAsync(
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates automatic payout balance settings.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown when the request body is <see langword="null"/>.</exception>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<BalanceSettings>> UpdateSettingsAsync(
        BalanceSettings settings,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
