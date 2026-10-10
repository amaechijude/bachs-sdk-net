using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Bachs.Net.Common;
using Bachs.Net.Serialization;
using Microsoft.Extensions.Options;

namespace Bachs.Net.Clients;

/// <summary>
/// Primary client implementation for interacting with the Bachs Fintech API.
/// </summary>
public sealed class BachsClient : IBachsClient, IDisposable
{
    private static readonly string SdkVersion =
        typeof(BachsClient).Assembly.GetName().Version?.ToString(3) ?? "1.0.0";

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
    public BachsClient(string apiKey)
        : this(new BachsClientOptions { ApiKey = apiKey }) { }

    /// <summary>
    /// Initializes a new instance of <see cref="BachsClient"/> with the specified secret API key and retry configuration.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="maxRetries"></param>
    public BachsClient(string apiKey, int maxRetries)
        : this(new BachsClientOptions { ApiKey = apiKey, MaxRetries = maxRetries }) { }

    /// <summary>
    /// Initializes a new instance of <see cref="BachsClient"/> with the specified secret API key and retry configuration.
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="maxRetries"></param>
    /// <param name="maxRetryDelay"></param>
    public BachsClient(string apiKey, int maxRetries, TimeSpan maxRetryDelay)
        : this(
            new BachsClientOptions
            {
                ApiKey = apiKey,
                MaxRetries = maxRetries,
                MaxRetryDelay = maxRetryDelay,
            }
        ) { }

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
        : this(httpClient, options.Value ?? throw new ArgumentNullException(nameof(options))) { }

    /// <summary>
    /// Initializes a new instance of <see cref="BachsClient"/> with an injected <see cref="HttpClient"/> and options.
    /// </summary>
    public BachsClient(HttpClient httpClient, BachsClientOptions options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        Options = options ?? throw new ArgumentNullException(nameof(options));
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
        if (!client.DefaultRequestHeaders.Accept.Any(h => h.MediaType == "application/json"))
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
            );

        if (client.DefaultRequestHeaders.UserAgent is { Count: 0 })
            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue("Bachs.Net", SdkVersion)
            );
    }

    private Uri BuildRequestUri(string path) => new(Options.GetEffectiveBaseUrl(), path);

    public async Task<TResponse> GetAsync<TResponse>(
        string path,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, BuildRequestUri(path));
        var response = await SendAsync<TResponse>(request, requestOptions, cancellationToken)
            .ConfigureAwait(false);
        return response.Data;
    }

    public async Task<TResponse> PostAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, BuildRequestUri(path));
        request.Content = JsonContent.Create(body, options: BachsJsonSerializer.Options);

        var response = await SendAsync<TResponse>(request, requestOptions, cancellationToken)
            .ConfigureAwait(false);
        return response.Data;
    }

    public async Task<TResponse> PostAsync<TResponse>(
        string path,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, BuildRequestUri(path));
        var response = await SendAsync<TResponse>(request, requestOptions, cancellationToken)
            .ConfigureAwait(false);
        return response.Data;
    }

    public async Task<TResponse> PatchAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch, BuildRequestUri(path));
        request.Content = JsonContent.Create(body, options: BachsJsonSerializer.Options);

        var response = await SendAsync<TResponse>(request, requestOptions, cancellationToken)
            .ConfigureAwait(false);
        return response.Data;
    }

    public async Task<TResponse> PutAsync<TRequest, TResponse>(
        string path,
        TRequest body,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, BuildRequestUri(path));
        request.Content = JsonContent.Create(body, options: BachsJsonSerializer.Options);

        var response = await SendAsync<TResponse>(request, requestOptions, cancellationToken)
            .ConfigureAwait(false);
        return response.Data;
    }

    public async Task<TResponse> DeleteAsync<TResponse>(
        string path,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, BuildRequestUri(path));
        var response = await SendAsync<TResponse>(request, requestOptions, cancellationToken)
            .ConfigureAwait(false);
        return response.Data;
    }

    public async Task DeleteAsync(
        string path,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, BuildRequestUri(path));
        await SendAsync<object>(request, requestOptions, cancellationToken).ConfigureAwait(false);
    }

    public async Task<BachsResponse<TResponse>> SendAsync<TResponse>(
        HttpRequestMessage request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var maxRetries = Options.MaxRetries;
        var needsCloning = maxRetries > 0;

        // Buffer payload only if we might retry
        byte[]? bodyBytes = null;
        string? contentType = null;
        if (needsCloning && request.Content != null)
        {
            bodyBytes = await request
                .Content.ReadAsByteArrayAsync(cancellationToken)
                .ConfigureAwait(false);
            contentType = request.Content.Headers.ContentType?.MediaType ?? "application/json";
        }

        var attempt = 0;

        while (true)
        {
            attempt++;

            // Use original request if no retries configured, else clone it
            HttpRequestMessage currentRequest = needsCloning
                ? CloneRequest(request, bodyBytes, contentType)
                : request;

            try
            {
                ApplyRequestHeaders(currentRequest, requestOptions);

                HttpResponseMessage response;
                try
                {
                    response = await _httpClient
                        .SendAsync(currentRequest, cancellationToken)
                        .ConfigureAwait(false);
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
                    await ApplyBackoffDelay(attempt, delay, cancellationToken)
                        .ConfigureAwait(false);
                    continue;
                }

                using (response)
                {
                    string? responseContent = null;
                    TResponse data = default!;

                    // Read content as string for errors so we can attach it to the exception
                    if (!response.IsSuccessStatusCode)
                    {
                        responseContent = await response
                            .Content.ReadAsStringAsync(cancellationToken)
                            .ConfigureAwait(false);

                        BachsError? error = null;
                        if (!string.IsNullOrWhiteSpace(responseContent))
                        {
                            try
                            {
                                error = BachsJsonSerializer.Deserialize<BachsError>(
                                    responseContent
                                );
                            }
                            catch
                            {
                                // Ignore JSON parsing failure for non-JSON error pages
                            }
                        }

                        if (Options.ThrowOnApiErrors)
                        {
                            throw BachsApiException.Create(
                                response.StatusCode,
                                error,
                                rateLimit,
                                responseContent
                            );
                        }
                    }

                    if (typeof(TResponse) == typeof(string))
                    {
                        responseContent ??= await response
                            .Content.ReadAsStringAsync(cancellationToken)
                            .ConfigureAwait(false);
                        data = (TResponse)(object)responseContent;
                    }
                    else if (response.StatusCode != HttpStatusCode.NoContent)
                    {
                        if (
                            !response.IsSuccessStatusCode
                            && !string.IsNullOrWhiteSpace(responseContent)
                        )
                        {
                            data = BachsJsonSerializer.Deserialize<TResponse>(responseContent)!;
                        }
                        else if (response.IsSuccessStatusCode)
                        {
                            // Success path: stream deserialize directly without allocating string
                            var stream = await response
                                .Content.ReadAsStreamAsync(cancellationToken)
                                .ConfigureAwait(false);
                            try
                            {
                                data = (
                                    await System
                                        .Text.Json.JsonSerializer.DeserializeAsync<TResponse>(
                                            stream,
                                            BachsJsonSerializer.Options,
                                            cancellationToken
                                        )
                                        .ConfigureAwait(false)
                                )!;
                            }
                            catch (System.Text.Json.JsonException)
                            {
                                // Ignore potential empty success responses parsing failures
                            }
                        }
                    }

                    return new BachsResponse<TResponse>
                    {
                        Data = data,
                        StatusCode = response.StatusCode,
                        RateLimit = rateLimit,
                        Headers = response.Headers,
                        RawJson = responseContent,
                    };
                }
            }
            finally
            {
                if (needsCloning)
                {
                    currentRequest.Dispose();
                }
            }
        }
    }

    private void ApplyRequestHeaders(HttpRequestMessage request, RequestOptions? requestOptions)
    {
        // 1. Authorization
        if (!string.IsNullOrWhiteSpace(Options.ApiKey))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Options.ApiKey);
        }

        // 2. Idempotency Key
        if (!string.IsNullOrWhiteSpace(requestOptions?.IdempotencyKey))
        {
            request.Headers.Remove("Idempotency-Key");
            request.Headers.Add("Idempotency-Key", requestOptions.IdempotencyKey);
        }

        // 3. Connected Account ID
        var accountId = requestOptions?.AccountId ?? Options.AccountId;
        if (!string.IsNullOrWhiteSpace(accountId))
        {
            request.Headers.Remove("X-Account-Id");
            request.Headers.Add("X-Account-Id", accountId);
        }

        // 4. Custom headers
        if (requestOptions?.CustomHeaders == null)
            return;
        foreach (var (headerKey, headerVal) in requestOptions.CustomHeaders)
        {
            request.Headers.Remove(headerKey);
            request.Headers.Add(headerKey, headerVal);
        }
    }

    private static HttpRequestMessage CloneRequest(
        HttpRequestMessage original,
        byte[]? bodyBytes,
        string? contentType
    )
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri);
        foreach (var header in original.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        if (bodyBytes != null)
        {
            clone.Content = new ByteArrayContent(bodyBytes);
            clone.Content.Headers.ContentType = new MediaTypeHeaderValue(
                contentType ?? "application/json"
            );
        }

        return clone;
    }

    private static bool ShouldRetry(HttpStatusCode statusCode) =>
        statusCode
            is HttpStatusCode.TooManyRequests
                or HttpStatusCode.InternalServerError
                or HttpStatusCode.BadGateway
                or HttpStatusCode.ServiceUnavailable
                or HttpStatusCode.GatewayTimeout;

    private async Task ApplyBackoffDelay(
        int attempt,
        TimeSpan? serverDelay,
        CancellationToken cancellationToken
    )
    {
        TimeSpan delay;
        if (serverDelay.HasValue && serverDelay.Value > TimeSpan.Zero)
        {
            delay =
                serverDelay.Value > Options.MaxRetryDelay
                    ? Options.MaxRetryDelay
                    : serverDelay.Value;
        }
        else
        {
            // Exponential backoff: 200ms, 400ms, 800ms with jitter
            var milliseconds = (int)(200 * Math.Pow(2, attempt - 1)) + Random.Shared.Next(50, 150);
            delay = TimeSpan.FromMilliseconds(milliseconds);
            if (delay > Options.MaxRetryDelay)
                delay = Options.MaxRetryDelay;
        }

        await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
    }

    public void Dispose()
    {
        // No-op: the internal HttpClient is either shared statically or injected via DI,
        // so BachsClient does not own its lifecycle.
    }
}
