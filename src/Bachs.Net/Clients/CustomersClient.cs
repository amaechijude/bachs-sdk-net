using Bachs.Net.Common;
using Bachs.Net.Models.Customers;

namespace Bachs.Net.Clients;

/// <summary>
/// Client implementation for managing customers and portal sessions.
/// </summary>
public sealed class CustomersClient(IBachsClient bachsClient)
    : BaseClient(bachsClient),
        ICustomersClient
{
    public Task<BachsResponse<Customer>> CreateAsync(
        CreateCustomerRequest request,
        BachsRequestOptions? requestOptions = null,
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

    public Task<BachsResponse<Customer>> GetAsync(
        string customerId,
        BachsRequestOptions? requestOptions = null,
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

    public Task<BachsResponse<BachsList<Customer>>> ListAsync(
        ListCustomersOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    )
    {
        var path = BachsQueryBuilder.BuildUrl("v1/customers", options?.ToQueryParameters());
        return BachsClient.GetAsync<BachsList<Customer>>(
            path,
            requestOptions ?? options,
            cancellationToken
        );
    }

    public Task<BachsResponse<Customer>> UpdateAsync(
        string customerId,
        UpdateCustomerRequest request,
        BachsRequestOptions? requestOptions = null,
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

    public Task<BachsResponse<CustomerPortalSession>> CreatePortalSessionAsync(
        string customerId,
        BachsRequestOptions? requestOptions = null,
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
