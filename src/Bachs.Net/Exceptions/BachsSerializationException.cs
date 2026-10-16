namespace Bachs.Net.Exceptions;

public sealed class BachsSerializationException(Exception innerException)
    : BachsException("Failed to deserialize the API response.", innerException);
