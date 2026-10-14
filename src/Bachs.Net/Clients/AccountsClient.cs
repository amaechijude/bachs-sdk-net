using Bachs.Net.Common;
using Bachs.Net.Models.Accounts;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for managing connected accounts.
/// </summary>
public sealed class AccountsClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        IAccountsClient
{
    public Task<Account> CreateAsync(
        CreateAccountRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        return BachsClient.PostAsync<CreateAccountRequest, Account>(
            "v1/accounts",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<Account> GetAsync(
        string accountId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);
        return BachsClient.GetAsync<Account>(
            $"v1/accounts/{Uri.EscapeDataString(accountId)}",
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsList<Account>> ListAsync(
        ListAccountsOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = BachsQueryBuilder.BuildUrl("v1/accounts", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Account>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }

    public Task<Account> UpdateAsync(
        string accountId,
        UpdateAccountRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);

        return BachsClient.PostAsync<UpdateAccountRequest, Account>(
            $"v1/accounts/{Uri.EscapeDataString(accountId)}",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<AccountLink> CreateAccountLinkAsync(
        string accountId,
        CreateAccountLinkRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);

        return BachsClient.PostAsync<CreateAccountLinkRequest, AccountLink>(
            $"v1/accounts/{Uri.EscapeDataString(accountId)}/account-links",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<Account> GetMeAsync(
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        return BachsClient.GetAsync<Account>("v1/accounts/me", requestOptions, cancellationToken);
    }
}
