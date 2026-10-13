namespace Bachs.Net.Common;

/// <summary>
/// Initializes a new instance of <see cref="BachsInvalidOperationException"/> with a message.
/// </summary>
public sealed class BachsInvalidOperationException(string message) : BachsException(message);
