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
    Task<Transfer> CreateAsync(
        CreateTransferRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a transfer by its ID.
    /// </summary>
    Task<Transfer> GetAsync(
        string transferId,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists transfers with pagination.
    /// </summary>
    Task<BachsList<Transfer>> ListAsync(
        ListTransfersOptions? options = null,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default);
}
