using Bachs.Net.Common;
using Bachs.Net.Models.Payouts;

namespace Bachs.Net.Clients;

/// <summary>
/// Client for managing payouts, withdrawals, quotes, and destinations.
/// </summary>
public interface IPayoutsClient
{
    /// <summary>
    /// Creates and initiates a payout withdrawal.
    /// </summary>
    Task<Payout> CreateAsync(
        CreatePayoutRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Requests a conversion quote for an international/cross-currency payout.
    /// </summary>
    Task<PayoutQuote> CreateQuoteAsync(
        CreatePayoutQuoteRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a payout by its withdrawal ID.
    /// </summary>
    Task<Payout> GetAsync(
        string withdrawalId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists payouts with pagination and filtering.
    /// </summary>
    Task<BachsList<Payout>> ListAsync(
        ListPayoutsOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}

/// <summary>
/// Client for managing payout destinations (bank accounts, mobile money wallets, crypto wallets).
/// </summary>
public interface IPayoutDestinationsClient
{
    /// <summary>
    /// Registers a new payout destination.
    /// </summary>
    Task<PayoutDestination> CreateAsync(
        CreatePayoutDestinationRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a payout destination by its ID.
    /// </summary>
    Task<PayoutDestination> GetAsync(
        string destinationId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists saved payout destinations.
    /// </summary>
    Task<BachsList<PayoutDestination>> ListAsync(
        ListRequestOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates a payout destination.
    /// </summary>
    Task<PayoutDestination> UpdateAsync(
        string destinationId,
        UpdatePayoutDestinationRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Deletes a payout destination.
    /// </summary>
    Task DeleteAsync(
        string destinationId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
