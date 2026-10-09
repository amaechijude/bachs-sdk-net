using Bachs.Net.Common;
using Bachs.Net.Models.Payouts;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for managing payouts and quotes.
/// </summary>
public class PayoutsClient : BaseClient, IPayoutsClient
{
    public PayoutsClient(IBachsClient bachsClient) : base(bachsClient) { }

    public Task<Payout> CreateAsync(
        CreatePayoutRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<CreatePayoutRequest, Payout>(
            "v1/payouts",
            request,
            requestOptions,
            cancellationToken);
    }

    public Task<PayoutQuote> CreateQuoteAsync(
        CreatePayoutQuoteRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<CreatePayoutQuoteRequest, PayoutQuote>(
            "v1/payouts/quotes",
            request,
            requestOptions,
            cancellationToken);
    }

    public Task<Payout> GetAsync(
        string withdrawalId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(withdrawalId);
        return BachsClient.GetAsync<Payout>(
            $"v1/payouts/{Uri.EscapeDataString(withdrawalId)}",
            requestOptions,
            cancellationToken);
    }

    public Task<BachsList<Payout>> ListAsync(
        ListPayoutsOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        var path = QueryBuilder.BuildUrl("v1/payouts", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Payout>>(
            path,
            requestOptions ?? options,
            cancellationToken);
    }
}

/// <summary>
/// Client implementation for managing payout destinations.
/// </summary>
public class PayoutDestinationsClient : BaseClient, IPayoutDestinationsClient
{
    public PayoutDestinationsClient(IBachsClient bachsClient) : base(bachsClient) { }

    public Task<PayoutDestination> CreateAsync(
        CreatePayoutDestinationRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<CreatePayoutDestinationRequest, PayoutDestination>(
            "v1/payouts/destinations",
            request,
            requestOptions,
            cancellationToken);
    }

    public Task<PayoutDestination> GetAsync(
        string destinationId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationId);
        return BachsClient.GetAsync<PayoutDestination>(
            $"v1/payouts/destinations/{Uri.EscapeDataString(destinationId)}",
            requestOptions,
            cancellationToken);
    }

    public Task<BachsList<PayoutDestination>> ListAsync(
        ListRequestOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        var path = QueryBuilder.BuildUrl("v1/payouts/destinations", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<PayoutDestination>>(
            path,
            requestOptions ?? options,
            cancellationToken);
    }

    public Task<PayoutDestination> UpdateAsync(
        string destinationId,
        UpdatePayoutDestinationRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationId);
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PatchAsync<UpdatePayoutDestinationRequest, PayoutDestination>(
            $"v1/payouts/destinations/{Uri.EscapeDataString(destinationId)}",
            request,
            requestOptions,
            cancellationToken);
    }

    public Task DeleteAsync(
        string destinationId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationId);
        return BachsClient.DeleteAsync(
            $"v1/payouts/destinations/{Uri.EscapeDataString(destinationId)}",
            requestOptions,
            cancellationToken);
    }
}
