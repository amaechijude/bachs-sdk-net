using Bachs.Common;
using Bachs.Models.Accounts;

namespace Bachs.Clients;

/// <summary>
/// Client implementation for managing connected accounts.
/// </summary>
public sealed class AccountsClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        IAccountsClient
{
    public Task<BachsResponse<Account>> CreateAsync(
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

    public Task<BachsResponse<Account>> GetAsync(
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

    public Task<BachsResponse<BachsList<Account>>> ListAsync(
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

    public Task<BachsResponse<Account>> UpdateAsync(
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

    public Task<BachsResponse<AccountLink>> CreateAccountLinkAsync(
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

    public Task<BachsResponse<Account>> GetMeAsync(
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        return BachsClient.GetAsync<Account>("v1/accounts/me", requestOptions, cancellationToken);
    }
}
