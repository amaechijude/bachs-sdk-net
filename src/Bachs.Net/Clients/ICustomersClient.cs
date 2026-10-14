using Bachs.Net.Common;
using Bachs.Net.Models.Customers;

namespace Bachs.Net.Clients;

/// <summary>
/// Client for managing customers and customer portal sessions.
/// </summary>
public interface ICustomersClient
{
    /// <summary>
    /// Creates a new customer record.
    /// </summary>
    Task<BachsResponse<Customer>> CreateAsync(
        CreateCustomerRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a customer by their ID.
    /// </summary>
    Task<BachsResponse<Customer>> GetAsync(
        string customerId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists customers with pagination and optional search.
    /// </summary>
    Task<BachsResponse<BachsList<Customer>>> ListAsync(
        ListCustomersOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Updates an existing customer record.
    /// </summary>
    Task<BachsResponse<Customer>> UpdateAsync(
        string customerId,
        UpdateCustomerRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Creates a pre-authenticated customer portal session for self-service billing management.
    /// </summary>
    Task<BachsResponse<CustomerPortalSession>> CreatePortalSessionAsync(
        string customerId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
