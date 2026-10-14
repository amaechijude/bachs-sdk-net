using Bachs.Net.Common;
using Bachs.Net.Models.Transfers;

namespace Bachs.Net.Clients;

/// <summary>
/// Client for transferring funds between platform and connected accounts.
/// </summary>
public interface ITransfersClient
{
    /// <summary>
    /// Creates a transfer of funds.
    /// </summary>
    Task<BachsResponse<Transfer>> CreateAsync(
        CreateTransferRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves a transfer by its ID.
    /// </summary>
    Task<BachsResponse<Transfer>> GetAsync(
        string transferId,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Lists transfers with pagination.
    /// </summary>
    Task<BachsResponse<BachsList<Transfer>>> ListAsync(
        ListTransfersOptions? options = null,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
