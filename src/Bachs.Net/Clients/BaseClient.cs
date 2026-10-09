namespace Bachs.Net.Clients;

/// <summary>
/// Abstract base class for Bachs resource clients.
/// </summary>
public abstract class BaseClient
{
    /// <summary>
    /// Gets the parent Bachs client instance.
    /// </summary>
    protected readonly IBachsClient BachsClient;

    /// <summary>
    /// Initializes a new instance of <see cref="BaseClient"/>.
    /// </summary>
    /// <param name="bachsClient">The root Bachs client.</param>
    protected BaseClient(IBachsClient bachsClient)
    {
        BachsClient = bachsClient ?? throw new ArgumentNullException(nameof(bachsClient));
    }
}
