namespace Bachs.Exceptions;

public sealed class BachsNetworkException(Exception innerException)
    : BachsException("A network error occurred while sending the request.", innerException);
