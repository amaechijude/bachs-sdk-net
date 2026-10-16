namespace Bachs.Net.Exceptions;

/// <summary>
/// Base exception thrown by the Bachs .NET SDK.
/// </summary>
public class BachsException : Exception
{
    /// <summary>
    /// Initializes a new instance of <see cref="BachsException"/> with a message.
    /// </summary>
    public BachsException(string message)
        : base(message) { }

    /// <summary>
    /// Initializes a new instance of <see cref="BachsException"/> with a message and inner exception.
    /// </summary>
    public BachsException(string message, Exception innerException)
        : base(message, innerException) { }
}
