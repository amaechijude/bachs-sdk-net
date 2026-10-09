using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Bachs.Net.Common;
using Bachs.Net.Serialization;
using Microsoft.Extensions.Options;

namespace Bachs.Net.Clients;

/// <summary>
/// Primary client implementation for interacting with the Bachs Fintech API.
/// </summary>
public class BachsClient : IBachsClient, IDisposable
{
    private static readonly string SdkVersion =
        typeof(BachsClient).Assembly.GetName().Version?.ToString(3) ?? "1.0.0";

    private readonly HttpClient _httpClient;
    private readonly bool _disposeHttpClient;

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
    public BachsClient(string apiKey)
        : this(new BachsClientOptions { ApiKey = apiKey })
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BachsClient"/> with options.
    /// </summary>
    /// <param name="options">Client options.</param>
    public BachsClient(BachsClientOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        options.Validate();

        Options = options;
        _httpClient = CreateConfiguredHttpClient(options);
        _disposeHttpClient = true;

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
        : this(httpClient, options?.Value ?? throw new ArgumentNullException(nameof(options)))
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="BachsClient"/> with an injected <see cref="HttpClient"/> and options.
    /// </summary>
    public BachsClient(HttpClient httpClient, BachsClientOptions options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Options = options ?? throw new ArgumentNullException(nameof(options));
        Options.Validate();

        _disposeHttpClient = false;
        ConfigureHttpClientHeaders(_httpClient, Options);

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

    private static HttpClient CreateConfiguredHttpClient(BachsClientOptions options)
    {
        var client = new HttpClient
        {
            BaseAddress = options.GetEffectiveBaseUrl(),
            Timeout = options.Timeout
        };

        ConfigureHttpClientHeaders(client, options);
        return client;
    }

    private static void ConfigureHttpClientHeaders(HttpClient client, BachsClientOptions options)
    {
        if (client.BaseAddress == null)
        {
            client.BaseAddress = options.GetEffectiveBaseUrl();
        }

        if (client.DefaultRequestHeaders.Authorization == null && !string.IsNullOrWhiteSpace(options.ApiKey))
        {
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", options.ApiKey);
        }

        if (!client.DefaultRequestHeaders.Accept.Any(h => h.MediaType == "application/json"))
        {
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }

        if (!client.DefaultRequestHeaders.UserAgent.Any())
        {
            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("Bachs.Net", SdkVersion));
        }
    }

    public async Task<TResponse> GetAsync<TResponse>(
        string path,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        var response = await SendAsync<TResponse>(request, requestOptions, cancellationToken).ConfigureAwait(false);
        return response.Data;
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path);
        var json = BachsJsonSerializer.Serialize(body);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await SendAsync<TResponse>(request, requestOptions, cancellationToken).ConfigureAwait(false);
        return response.Data;
    }

    public async Task<TResponse> PostAsync<TResponse>(
        string path,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, path);
        var response = await SendAsync<TResponse>(request, requestOptions, cancellationToken).ConfigureAwait(false);
        return response.Data;
    }

    public async Task<TResponse> PatchAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch, path);
        var json = BachsJsonSerializer.Serialize(body);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await SendAsync<TResponse>(request, requestOptions, cancellationToken).ConfigureAwait(false);
        return response.Data;
    }

    public async Task<TResponse> PutAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, path);
        var json = BachsJsonSerializer.Serialize(body);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await SendAsync<TResponse>(request, requestOptions, cancellationToken).ConfigureAwait(false);
        return response.Data;
    }

    public async Task<TResponse> DeleteAsync<TResponse>(
        string path,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, path);
        var response = await SendAsync<TResponse>(request, requestOptions, cancellationToken).ConfigureAwait(false);
        return response.Data;
    }

    public async Task DeleteAsync(
        string path,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, path);
        await SendAsync<object>(request, requestOptions, cancellationToken).ConfigureAwait(false);
    }

    public async Task<BachsResponse<TResponse>> SendAsync<TResponse>(
        HttpRequestMessage request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        // Buffer payload so we can retry on transient failures if needed
        byte[]? bodyBytes = null;
        string? contentType = null;
        if (request.Content != null)
        {
            bodyBytes = await request.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
            contentType = request.Content.Headers.ContentType?.MediaType ?? "application/json";
        }

        var attempt = 0;
        var maxRetries = Options.MaxRetries;

        while (true)
        {
            attempt++;

            // Create copy of request for this attempt
            using var currentRequest = CloneRequest(request, bodyBytes, contentType);
            ApplyRequestHeaders(currentRequest, requestOptions);

            HttpResponseMessage? response = null;
            try
            {
                response = await _httpClient.SendAsync(currentRequest, cancellationToken).ConfigureAwait(false);
            }
            catch (HttpRequestException) when (attempt <= maxRetries)
            {
                await ApplyBackoffDelay(attempt, null, cancellationToken).ConfigureAwait(false);
                continue;
            }

            var rateLimit = RateLimitInfo.FromHeaders(response.Headers);

            // Handle transient retry on 429 or 5xx
            if (attempt <= maxRetries && ShouldRetry(response.StatusCode))
            {
                var delay = rateLimit.RetryAfter;
                response.Dispose();
                await ApplyBackoffDelay(attempt, delay, cancellationToken).ConfigureAwait(false);
                continue;
            }

            using (response)
            {
                var responseContent = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                if (!response.IsSuccessStatusCode)
                {
                    BachsError? error = null;
                    if (!string.IsNullOrWhiteSpace(responseContent))
                    {
                        try
                        {
                            error = BachsJsonSerializer.Deserialize<BachsError>(responseContent);
                        }
                        catch
                        {
                            // Ignore JSON parsing failure for non-JSON error pages
                        }
                    }

                    if (Options.ThrowOnApiErrors)
                    {
                        throw BachsApiException.Create(response.StatusCode, error, rateLimit, responseContent);
                    }
                }

                TResponse data = default!;
                if (typeof(TResponse) == typeof(string))
                {
                    data = (TResponse)(object)responseContent;
                }
                else if (response.StatusCode != HttpStatusCode.NoContent && !string.IsNullOrWhiteSpace(responseContent))
                {
                    data = BachsJsonSerializer.Deserialize<TResponse>(responseContent)!;
                }

                return new BachsResponse<TResponse>
                {
                    Data = data,
                    StatusCode = response.StatusCode,
                    RateLimit = rateLimit,
                    Headers = response.Headers,
                    RawJson = responseContent
                };
            }
        }
    }

    private void ApplyRequestHeaders(HttpRequestMessage request, RequestOptions? requestOptions)
    {
        // 1. Idempotency Key
        if (!string.IsNullOrWhiteSpace(requestOptions?.IdempotencyKey))
        {
            request.Headers.Remove("Idempotency-Key");
            request.Headers.Add("Idempotency-Key", requestOptions.IdempotencyKey);
        }

        // 2. Connected Account ID
        var accountId = requestOptions?.AccountId ?? Options.AccountId;
        if (!string.IsNullOrWhiteSpace(accountId))
        {
            request.Headers.Remove("X-Account-Id");
            request.Headers.Add("X-Account-Id", accountId);
        }

        // 3. Custom headers
        if (requestOptions?.CustomHeaders != null)
        {
            foreach (var (headerKey, headerVal) in requestOptions.CustomHeaders)
            {
                request.Headers.Remove(headerKey);
                request.Headers.Add(headerKey, headerVal);
            }
        }
    }

    private static HttpRequestMessage CloneRequest(HttpRequestMessage original, byte[]? bodyBytes, string? contentType)
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri);
        foreach (var header in original.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (bodyBytes != null)
        {
            clone.Content = new ByteArrayContent(bodyBytes);
            clone.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType ?? "application/json");
        }

        return clone;
    }

    private static bool ShouldRetry(HttpStatusCode statusCode)
    {
        return statusCode == HttpStatusCode.TooManyRequests ||
               statusCode == HttpStatusCode.InternalServerError ||
               statusCode == HttpStatusCode.BadGateway ||
               statusCode == HttpStatusCode.ServiceUnavailable ||
               statusCode == HttpStatusCode.GatewayTimeout;
    }

    private async Task ApplyBackoffDelay(int attempt, TimeSpan? serverDelay, CancellationToken cancellationToken)
    {
        TimeSpan delay;
        if (serverDelay.HasValue && serverDelay.Value > TimeSpan.Zero)
        {
            delay = serverDelay.Value > Options.MaxRetryDelay ? Options.MaxRetryDelay : serverDelay.Value;
        }
        else
        {
            // Exponential backoff: 200ms, 400ms, 800ms with jitter
            var milliseconds = (int)(200 * Math.Pow(2, attempt - 1)) + Random.Shared.Next(50, 150);
            delay = TimeSpan.FromMilliseconds(milliseconds);
            if (delay > Options.MaxRetryDelay)
            {
                delay = Options.MaxRetryDelay;
            }
        }

        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
    }

    public void Dispose()
    {
        if (_disposeHttpClient)
        {
            _httpClient.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}
