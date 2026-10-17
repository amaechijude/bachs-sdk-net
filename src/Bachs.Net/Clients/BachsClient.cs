using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Bachs.Net.Common;
using Bachs.Net.Exceptions;
using Bachs.Net.Serialization;
using Microsoft.Extensions.Options;
using Polly.CircuitBreaker;
using Polly.Timeout;

namespace Bachs.Net.Clients;

/// <summary>
/// Primary client implementation for interacting with the Bachs Fintech API.
/// </summary>
public sealed class BachsClient : IBachsClient, IDisposable
{
    private static readonly string SdkVersion =
        typeof(BachsClient).Assembly.GetName().Version?.ToString(3) ?? "1.0.0";
    private const string IdempotencyKeyHeader = "Idempotency-Key";
    private const string AccountIdHeader = "X-Account-Id";

    /// <summary>
    /// Shared static HttpClient used for all standalone instances to prevent socket exhaustion.
    /// It is stateless and does not hold API keys or base addresses.
    /// </summary>
    private static readonly HttpClient SharedHttpClient = CreateSharedHttpClient();

    private readonly HttpClient _httpClient;

    public BachsClientOptions Options { get; }
    public HttpClient HttpClient => _httpClient;

    // Resource sub-clients
    public ICheckoutSessionsClient CheckoutSessions { get; }
    public IProductsClient Products { get; }
    public ICustomersClient Customers { get; }
    public ISubscriptionsClient Subscriptions { get; }
    public IPaymentsClient Payments { get; }
    public IRefundsClient Refunds { get; }
    public IPayoutsClient Payouts { get; }
    public IPayoutDestinationsClient PayoutDestinations { get; }
    public IBalancesClient Balances { get; }
    public IConversionsClient Conversions { get; }
    public ITransfersClient Transfers { get; }
    public IDisputesClient Disputes { get; }
    public IAccountsClient Accounts { get; }
    public IVirtualAccountsClient VirtualAccounts { get; }
    public IWebhooksClient Webhooks { get; }
    public IMiscClient Misc { get; }

    /// <summary>
    /// Initializes a new instance of <see cref="BachsClient"/> with the specified secret API key.
    /// </summary>
    /// <param name="apiKey">Bachs secret key starting with 'sk_sandbox_' or 'sk_live_'.</param>
    /// <param name="webhookSecret">Bachs webhook secret</param>
    public BachsClient(string apiKey, string webhookSecret)
        : this(new BachsClientOptions { ApiKey = apiKey, WebhookSecret = webhookSecret }) { }

    /// <summary>
    /// Initializes a new instance of <see cref="BachsClient"/> with options using a shared static HTTP client.
    /// This is safe for high-throughput standalone usage without Dependency Injection.
    /// </summary>
    /// <param name="options">Client options.</param>
    public BachsClient(BachsClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();

        Options = options;
        _httpClient = SharedHttpClient;

        CheckoutSessions = new CheckoutSessionsClient(this);
        Products = new ProductsClient(this);
        Customers = new CustomersClient(this);
        Subscriptions = new SubscriptionsClient(this);
        Payments = new PaymentsClient(this);
        Refunds = new RefundsClient(this);
        Payouts = new PayoutsClient(this);
        PayoutDestinations = new PayoutDestinationsClient(this);
        Balances = new BalancesClient(this);
        Conversions = new ConversionsClient(this);
        Transfers = new TransfersClient(this);
        Disputes = new DisputesClient(this);
        Accounts = new AccountsClient(this);
        VirtualAccounts = new VirtualAccountsClient(this);
        Webhooks = new WebhooksClient(this);
        Misc = new MiscClient(this);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BachsClient"/> with an injected <see cref="HttpClient"/>
    /// and options (typically used with Microsoft.Extensions.DependencyInjection).
    /// </summary>
    [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
    public BachsClient(HttpClient httpClient, IOptions<BachsClientOptions> options)
        : this(httpClient, options.Value ?? throw new BachsArgumentNullException(nameof(options)))
    { }

    /// <summary>
    /// Initializes a new instance of <see cref="BachsClient"/> with an injected <see cref="HttpClient"/> and options.
    /// </summary>
    public BachsClient(HttpClient httpClient, BachsClientOptions options)
    {
        _httpClient = httpClient ?? throw new BachsArgumentNullException(nameof(httpClient));
        Options = options ?? throw new BachsArgumentNullException(nameof(options));
        Options.Validate();

        ConfigureHttpClientHeaders(_httpClient);

        CheckoutSessions = new CheckoutSessionsClient(this);
        Products = new ProductsClient(this);
        Customers = new CustomersClient(this);
        Subscriptions = new SubscriptionsClient(this);
        Payments = new PaymentsClient(this);
        Refunds = new RefundsClient(this);
        Payouts = new PayoutsClient(this);
        PayoutDestinations = new PayoutDestinationsClient(this);
        Balances = new BalancesClient(this);
        Conversions = new ConversionsClient(this);
        Transfers = new TransfersClient(this);
        Disputes = new DisputesClient(this);
        Accounts = new AccountsClient(this);
        VirtualAccounts = new VirtualAccountsClient(this);
        Webhooks = new WebhooksClient(this);
        Misc = new MiscClient(this);
    }

    private static HttpClient CreateSharedHttpClient()
    {
        var client = new HttpClient();
        ConfigureHttpClientHeaders(client);
        return client;
    }

    private static void ConfigureHttpClientHeaders(HttpClient client)
    {
        var headers = client.DefaultRequestHeaders;

        var hasJsonAccept = headers.Accept.Any(accept =>
            string.Equals(accept.MediaType, "application/json", StringComparison.OrdinalIgnoreCase)
        );

        if (!hasJsonAccept)
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        if (headers.UserAgent is { Count: 0 })
            headers.UserAgent.Add(new ProductInfoHeaderValue("Bachs.Net", SdkVersion));
    }

    private Uri BuildRequestUri(string path) => new(Options.GetEffectiveBaseUrl(), path);

    // ------------------------------------------------------------------
    // Public HTTP verbs. These intentionally are not `async`: they hand the Task straight
    // back from SendAsync, which avoids an extra state machine allocation per call.
    // ------------------------------------------------------------------

    public Task<BachsResponse<TResponse>> GetAsync<TResponse>(
        string path,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    ) => SendAsync<TResponse>(HttpMethod.Get, path, null, requestOptions, cancellationToken);

    public Task<BachsResponse<TResponse>> PostAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    ) =>
        SendAsync<TResponse>(
            HttpMethod.Post,
            path,
            CreateJsonContent(body),
            requestOptions,
            cancellationToken
        );

    public Task<BachsResponse<TResponse>> PostAsync<TResponse>(
        string path,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    ) => SendAsync<TResponse>(HttpMethod.Post, path, null, requestOptions, cancellationToken);

    public Task<BachsResponse<TResponse>> PatchAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    ) =>
        SendAsync<TResponse>(
            HttpMethod.Patch,
            path,
            CreateJsonContent(body),
            requestOptions,
            cancellationToken
        );

    public Task<BachsResponse<TResponse>> PutAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    ) =>
        SendAsync<TResponse>(
            HttpMethod.Put,
            path,
            CreateJsonContent(body),
            requestOptions,
            cancellationToken
        );

    public Task<BachsResponse<TResponse>> DeleteAsync<TResponse>(
        string path,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    ) => SendAsync<TResponse>(HttpMethod.Delete, path, null, requestOptions, cancellationToken);

    public Task<BachsResponse<object?>> DeleteAsync(
        string path,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    ) => SendAsync<object?>(HttpMethod.Delete, path, null, requestOptions, cancellationToken);

    // ------------------------------------------------------------------
    // Core pipeline
    // ------------------------------------------------------------------

    private static JsonContent CreateJsonContent<TRequest>(TRequest body) =>
        JsonContent.Create(body, options: BachsJsonSerializer.Options);

    private void ApplyRequestHeaders(
        HttpRequestMessage request,
        BachsRequestOptions? requestOptions
    )
    {
        string apiKey = requestOptions?.ScopedApiKey ?? Options.ApiKey;

        // 1. Authorization
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        // 2. Idempotency Key
        if (!string.IsNullOrWhiteSpace(requestOptions?.IdempotencyKey))
        {
            request.Headers.Remove(IdempotencyKeyHeader);
            request.Headers.Add(IdempotencyKeyHeader, requestOptions.IdempotencyKey);
        }

        // 3. Connected Account ID
        var accountId = requestOptions?.AccountId ?? Options.AccountId;
        if (!string.IsNullOrWhiteSpace(accountId))
        {
            request.Headers.Remove(AccountIdHeader);
            request.Headers.Add(AccountIdHeader, accountId);
        }

        // 4. Custom headers
        if (requestOptions?.CustomHeaders is not { Count: > 0 })
            return;
        foreach (var (headerKey, headerVal) in requestOptions.CustomHeaders)
        {
            request.Headers.Remove(headerKey);
            request.Headers.Add(headerKey, headerVal);
        }
    }

    public void Dispose()
    {
        // No-op: the internal HttpClient is either shared statically or injected via DI,
        // so BachsClient does not own its lifecycle.
    }

    private async Task<BachsResponse<TResponse>> SendAsync<TResponse>(
        HttpMethod method,
        string path,
        HttpContent? content,
        BachsRequestOptions? requestOptions,
        CancellationToken cancellationToken
    )
    {
        // Disposing the request also disposes its content.
        using var httpRequest = new HttpRequestMessage(method, BuildRequestUri(path))
        {
            Content = content,
        };

        ApplyRequestHeaders(httpRequest, requestOptions);
        HttpResponseMessage httpResponse;

        try
        {
            // HttpResponseMessage is disposable, so we wrap its usage
            httpResponse = await _httpClient
                .SendAsync(httpRequest, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (TimeoutRejectedException ex)
        {
            throw new BachsTimeoutException(ex);
        }
        catch (BrokenCircuitException ex)
        {
            throw new BachsServiceUnavailableException(ex);
        }
        catch (HttpRequestException ex)
        {
            throw new BachsNetworkException(ex);
        }
        catch (OperationCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            // Treat transport-level timeouts (e.g. HttpClient.Timeout) as SDK timeouts,
            // but let genuine caller cancellation propagate unchanged.
            throw new BachsTimeoutException(ex);
        }
        catch (JsonException ex)
        {
            throw new BachsSerializationException(ex);
        }

        // Use a 'using' block or declaration to ensure it's disposed
        // immediately after we read the content streams.
        using (httpResponse)
        {
            string body;
            try
            {
                body = await httpResponse
                    .Content.ReadAsStringAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (JsonException ex)
            {
                throw new BachsSerializationException(ex);
            }

            if (httpResponse.IsSuccessStatusCode)
            {
                TResponse? data = default;
                if (!string.IsNullOrWhiteSpace(body))
                {
                    try
                    {
                        data = BachsJsonSerializer.Deserialize<TResponse>(body);
                    }
                    catch (JsonException ex)
                    {
                        throw new BachsSerializationException(ex);
                    }
                }

                return BachsResponse<TResponse>.Success<TResponse>(data!, httpResponse.StatusCode);
            }

            BachsError? errorBody = null;
            if (!string.IsNullOrWhiteSpace(body))
            {
                try
                {
                    errorBody = BachsJsonSerializer.Deserialize<BachsError>(body);
                }
                catch (JsonException)
                {
                    // Non-JSON or unexpected error body: keep the status code, drop the body.
                    errorBody = null;
                }
            }

            return BachsResponse<TResponse>.Failure(errorBody, httpResponse.StatusCode);
        }
    }
}
