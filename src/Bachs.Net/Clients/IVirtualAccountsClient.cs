using Bachs.Net.Common;
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
    Task<VirtualAccount> CreateAsync(
        CreateVirtualAccountRequest request,
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Retrieves the fixed virtual bank account for your account or connected account.
    /// </summary>
    Task<VirtualAccount> GetAsync(
        string currency = "NGN",
        RequestOptions? requestOptions = null,
        CancellationToken cancellationToken = default
    );
}
