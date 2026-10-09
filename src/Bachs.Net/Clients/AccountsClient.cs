using Bachs.Net.Common;
using Bachs.Net.Models.Accounts;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for managing connected accounts.
/// </summary>
public class AccountsClient : BaseClient, IAccountsClient
{
    public AccountsClient(IBachsClient bachsClient) : base(bachsClient) { }

    public Task<Account> CreateAsync(
        CreateAccountRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<CreateAccountRequest, Account>(
            "v1/accounts",
            request,
            requestOptions,
            cancellationToken);
    }

    public Task<Account> GetAsync(
        string accountId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);
        return BachsClient.GetAsync<Account>(
            $"v1/accounts/{Uri.EscapeDataString(accountId)}",
            requestOptions,
            cancellationToken);
    }

    public Task<BachsList<Account>> ListAsync(
        ListAccountsOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        var path = QueryBuilder.BuildUrl("v1/accounts", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Account>>(
            path,
            requestOptions ?? options,
            cancellationToken);
    }

    public Task<Account> UpdateAsync(
        string accountId,
        UpdateAccountRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<UpdateAccountRequest, Account>(
            $"v1/accounts/{Uri.EscapeDataString(accountId)}",
            request,
            requestOptions,
            cancellationToken);
    }

    public Task<AccountLink> CreateAccountLinkAsync(
        string accountId,
        CreateAccountLinkRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<CreateAccountLinkRequest, AccountLink>(
            $"v1/accounts/{Uri.EscapeDataString(accountId)}/account-links",
            request,
            requestOptions,
            cancellationToken);
    }

    public Task<Account> GetMeAsync(
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default)
    {
        return BachsClient.GetAsync<Account>(
            "v1/accounts/me",
            requestOptions,
            cancellationToken);
    }
}
