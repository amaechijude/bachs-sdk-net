using Bachs.Net.Common;

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
    /// Sends a GET request and deserializes the JSON response body.
    /// </summary>
    Task<TResponse> GetAsync<TResponse>(
        string path,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a POST request with a JSON body and deserializes the JSON response body.
    /// </summary>
    Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends an empty POST request and deserializes the JSON response body.
    /// </summary>
    Task<TResponse> PostAsync<TResponse>(
        string path,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a PATCH request with a JSON body and deserializes the JSON response body.
    /// </summary>
    Task<TResponse> PatchAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a PUT request with a JSON body and deserializes the JSON response body.
    /// </summary>
    Task<TResponse> PutAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a DELETE request and deserializes the JSON response body.
    /// </summary>
    Task<TResponse> DeleteAsync<TResponse>(
        string path,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Sends a DELETE request without expecting a response body.
    /// </summary>
    Task DeleteAsync(
        string path,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Executes an HTTP request with retry handling and returns the complete <see cref="BachsResponse{TResponse}"/>.
    /// </summary>
    Task<BachsResponse<TResponse>> SendAsync<TResponse>(
        HttpRequestMessage request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
