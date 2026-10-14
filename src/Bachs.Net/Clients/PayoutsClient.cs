using Bachs.Net.Common;
using Bachs.Net.Models.Payouts;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for managing payouts and quotes.
/// </summary>
public sealed class PayoutsClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        IPayoutsClient
{
    public Task<BachsResponse<Payout>> CreateAsync(
        CreatePayoutRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<CreatePayoutRequest, Payout>(
            "v1/payouts",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<PayoutQuote>> CreateQuoteAsync(
        CreatePayoutQuoteRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<CreatePayoutQuoteRequest, PayoutQuote>(
            "v1/payouts/quotes",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<Payout>> GetAsync(
        string withdrawalId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(withdrawalId);
        return BachsClient.GetAsync<Payout>(
            $"v1/payouts/{Uri.EscapeDataString(withdrawalId)}",
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<BachsList<Payout>>> ListAsync(
        ListPayoutsOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = BachsQueryBuilder.BuildUrl("v1/payouts", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Payout>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }
}

/// <summary>
/// Client implementation for managing payout destinations.
/// </summary>
public sealed class PayoutDestinationsClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        IPayoutDestinationsClient
{
    public Task<BachsResponse<PayoutDestination>> CreateAsync(
        CreatePayoutDestinationRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<CreatePayoutDestinationRequest, PayoutDestination>(
            "v1/payouts/destinations",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<PayoutDestination>> GetAsync(
        string destinationId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationId);
        return BachsClient.GetAsync<PayoutDestination>(
            $"v1/payouts/destinations/{Uri.EscapeDataString(destinationId)}",
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<BachsList<PayoutDestination>>> ListAsync(
        BachsListRequestOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = BachsQueryBuilder.BuildUrl(
            "v1/payouts/destinations",
            options?.ToQueryParameters()
        );
        return BachsClient.GetAsync<BachsList<PayoutDestination>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }

    public Task<BachsResponse<PayoutDestination>> UpdateAsync(
        string destinationId,
        UpdatePayoutDestinationRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationId);
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PatchAsync<UpdatePayoutDestinationRequest, PayoutDestination>(
            $"v1/payouts/destinations/{Uri.EscapeDataString(destinationId)}",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsResponse<object?>> DeleteAsync(
        string destinationId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(destinationId);
        return BachsClient.DeleteAsync(
            $"v1/payouts/destinations/{Uri.EscapeDataString(destinationId)}",
            requestOptions,
            cancellationToken
        );
    }
}
