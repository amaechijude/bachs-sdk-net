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
    Task<Customer> CreateAsync(
        CreateCustomerRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a customer by their ID.
    /// </summary>
    Task<Customer> GetAsync(
        string customerId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists customers with pagination and optional search.
    /// </summary>
    Task<BachsList<Customer>> ListAsync(
        ListCustomersOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing customer record.
    /// </summary>
    Task<Customer> UpdateAsync(
        string customerId,
        UpdateCustomerRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a pre-authenticated customer portal session for self-service billing management.
    /// </summary>
    Task<CustomerPortalSession> CreatePortalSessionAsync(
        string customerId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default);
}
