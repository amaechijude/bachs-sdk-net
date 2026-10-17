namespace Bachs.Exceptions;

public sealed class BachsTimeoutException(Exception innerException)
    : BachsException("The request timed out.", innerException);
