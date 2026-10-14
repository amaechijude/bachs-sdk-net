using Bachs.Net.Common;
using Bachs.Net.Models.Accounts;

namespace Bachs.Net.Clients;

/// <summary>
/// Client for managing connected accounts (sub-accounts) on a platform.
/// </summary>
public interface IAccountsClient
{
    /// <summary>
    /// Creates a connected account under your platform.
    /// </summary>
    Task<Account> CreateAsync(
        CreateAccountRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a connected account by its ID.
    /// </summary>
    Task<Account> GetAsync(
        string accountId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists connected accounts under your platform.
    /// </summary>
    Task<BachsList<Account>> ListAsync(
        ListAccountsOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates a connected account.
    /// </summary>
    Task<Account> UpdateAsync(
        string accountId,
        UpdateAccountRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates a hosted onboarding link for an account to satisfy requirements.
    /// </summary>
    Task<AccountLink> CreateAccountLinkAsync(
        string accountId,
        CreateAccountLinkRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves the current platform account profile.
    /// </summary>
    Task<Account> GetMeAsync(
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
