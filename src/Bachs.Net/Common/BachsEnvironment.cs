namespace Bachs.Net.Common;

/// <summary>
/// Represents the Bachs environment to target.
/// </summary>
public enum BachsEnvironment
{
    /// <summary>
    /// The sandbox environment (https://sandbox-api.bachs.io).
    /// Used for testing without moving real money.
    /// </summary>
    Sandbox,

    /// <summary>
    /// The production environment (https://api.bachs.io).
    /// Used for live transactions.
    /// </summary>
    Production,

    /// <summary>
    /// Custom base URL specified in configuration.
    /// </summary>
    Custom,
}
