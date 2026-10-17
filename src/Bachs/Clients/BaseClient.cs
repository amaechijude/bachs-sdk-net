namespace Bachs.Clients;

/// <summary>
/// Abstract base class for Bachs resource clients.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="BaseClient"/>.
/// </remarks>
/// <param name="bachsClient">The root Bachs client.</param>
public abstract class BaseClient(IBachsClient bachsClient)
{
    /// <summary>
    /// Gets the parent Bachs client instance.
    /// </summary>
    protected readonly IBachsClient BachsClient =
        bachsClient ?? throw new ArgumentNullException(nameof(bachsClient));
}
