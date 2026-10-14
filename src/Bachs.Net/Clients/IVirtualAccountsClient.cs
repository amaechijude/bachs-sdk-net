using Bachs.Net.Common;
using Bachs.Net.Models.Common;
using Bachs.Net.Models.VirtualAccounts;

namespace Bachs.Net.Clients;

/// <summary>
/// Client for managing fixed virtual bank accounts for deposit collections.
/// </summary>
public interface IVirtualAccountsClient
{
    /// <summary>
    /// Creates a fixed virtual account number for continuous collections.
    /// </summary>
    Task<BachsResponse<VirtualAccount>> CreateAsync(
        CreateVirtualAccountRequest request,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves the fixed virtual bank account for your account or connected account.
    /// </summary>
    Task<BachsResponse<VirtualAccount>> GetAsync(
        BachsCurrency currency = BachsCurrency.NGN,
        BachsRequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
