using Bachs.Net.Common;
using Bachs.Net.Exceptions;

namespace Bachs.Net.Clients;

/// <summary>
/// Primary client for interacting with the Bachs Fintech API.
/// Provides access to specialized resource clients and low-level HTTP communication.
/// </summary>
public interface IBachsClient
{
    /// <summary>
    /// Gets the client configuration options.
    /// </summary>
    BachsClientOptions Options { get; }

    /// <summary>
    /// Gets the underlying <see cref="System.Net.Http.HttpClient"/>.
    /// </summary>
    HttpClient HttpClient { get; }

    /// <summary>
    /// Client for hosted checkout sessions.
    /// </summary>
    ICheckoutSessionsClient CheckoutSessions { get; }

    /// <summary>
    /// Client for products and billing catalog.
    /// </summary>
    IProductsClient Products { get; }

    /// <summary>
    /// Client for customer records and portal sessions.
    /// </summary>
    ICustomersClient Customers { get; }

    /// <summary>
    /// Client for recurring subscriptions.
    /// </summary>
    ISubscriptionsClient Subscriptions { get; }

    /// <summary>
    /// Client for payments and charges.
    /// </summary>
    IPaymentsClient Payments { get; }

    /// <summary>
    /// Client for payment refunds.
    /// </summary>
    IRefundsClient Refunds { get; }

    /// <summary>
    /// Client for payout withdrawals and quotes.
    /// </summary>
    IPayoutsClient Payouts { get; }

    /// <summary>
    /// Client for payout destinations.
    /// </summary>
    IPayoutDestinationsClient PayoutDestinations { get; }

    /// <summary>
    /// Client for balances and payout schedules.
    /// </summary>
    IBalancesClient Balances { get; }

    /// <summary>
    /// Client for currency conversions.
    /// </summary>
    IConversionsClient Conversions { get; }

    /// <summary>
    /// Client for internal balance transfers.
    /// </summary>
    ITransfersClient Transfers { get; }

    /// <summary>
    /// Client for payment disputes and chargebacks.
    /// </summary>
    IDisputesClient Disputes { get; }

    /// <summary>
    /// Client for connected accounts (Connect).
    /// </summary>
    IAccountsClient Accounts { get; }

    /// <summary>
    /// Client for fixed virtual bank accounts.
    /// </summary>
    IVirtualAccountsClient VirtualAccounts { get; }

    /// <summary>
    /// Client for webhook destinations and events.
    /// </summary>
    IWebhooksClient Webhooks { get; }

    /// <summary>
    /// Client for reference utilities (currencies, banks, account resolution).
    /// </summary>
    IMiscClient Misc { get; }

    /// <summary>
    /// Sends a GET request and returns the parsed body and HTTP response metadata.
    /// </summary>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<TResponse>> GetAsync<TResponse>(
        string path,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a POST request with a JSON body and returns the parsed body and HTTP response metadata.
    /// </summary>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<TResponse>> PostAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends an empty POST request and returns the parsed body and HTTP response metadata.
    /// </summary>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<TResponse>> PostAsync<TResponse>(
        string path,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a PATCH request with a JSON body and returns the parsed body and HTTP response metadata.
    /// </summary>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<TResponse>> PatchAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a PUT request with a JSON body and returns the parsed body and HTTP response metadata.
    /// </summary>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<TResponse>> PutAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a DELETE request and returns the parsed body and HTTP response metadata.
    /// </summary>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<TResponse>> DeleteAsync<TResponse>(
        string path,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a DELETE request without expecting a response body and returns HTTP response metadata.
    /// </summary>
    /// <exception cref="BachsTimeoutException">Thrown when the request exceeds the configured per-attempt or total timeout.</exception>
    /// <exception cref="BachsServiceUnavailableException">Thrown when the resilience circuit breaker is open.</exception>
    /// <exception cref="BachsNetworkException">Thrown when the underlying HTTP request fails (connection, DNS, or socket error).</exception>
    /// <exception cref="BachsSerializationException">Thrown when the response body cannot be deserialized.</exception>
    Task<BachsResponse<object?>> DeleteAsync(
        string path,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
