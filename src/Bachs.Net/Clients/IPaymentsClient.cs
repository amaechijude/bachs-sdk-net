using Bachs.Net.Common;
using Bachs.Net.Models.Payments;

namespace Bachs.Net.Clients;

/// <summary>
/// Client for reading and tracking payments and collections.
/// </summary>
public interface IPaymentsClient
{
    /// <summary>
    /// Retrieves a single payment by its charge ID.
    /// </summary>
    Task<BachsResponse<Payment>> GetAsync(
        string paymentId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists payments with pagination and filtering.
    /// </summary>
    Task<BachsResponse<BachsList<Payment>>> ListAsync(
        ListPaymentsOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
