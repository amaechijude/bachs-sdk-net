using Bachs.Net.Common;
using Bachs.Net.Models.Customers;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for managing customers and portal sessions.
/// </summary>
public sealed class CustomersClient(IBachsClient bachsClient) : BaseClient(bachsClient), ICustomersClient
{
    public Task<Customer> CreateAsync(
        CreateCustomerRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PostAsync<CreateCustomerRequest, Customer>(
            "v1/customers",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<Customer> GetAsync(
        string customerId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        return BachsClient.GetAsync<Customer>(
            $"v1/customers/{Uri.EscapeDataString(customerId)}",
            requestOptions,
            cancellationToken
        );
    }

    public Task<BachsList<Customer>> ListAsync(
        ListCustomersOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = QueryBuilder.BuildUrl("v1/customers", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Customer>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }

    public Task<Customer> UpdateAsync(
        string customerId,
        UpdateCustomerRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        ArgumentNullException.ThrowIfNull(request);
        return BachsClient.PatchAsync<UpdateCustomerRequest, Customer>(
            $"v1/customers/{Uri.EscapeDataString(customerId)}",
            request,
            requestOptions,
            cancellationToken
        );
    }

    public Task<CustomerPortalSession> CreatePortalSessionAsync(
        string customerId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(customerId);
        return BachsClient.PostAsync<CustomerPortalSession>(
            $"v1/customers/{Uri.EscapeDataString(customerId)}/portal-sessions",
            requestOptions,
            cancellationToken
        );
    }
}
