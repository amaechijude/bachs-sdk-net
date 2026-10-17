namespace Bachs.Exceptions;

public sealed class BachsServiceUnavailableException(Exception innerException)
    : BachsException("The service is temporarily unavailable.", innerException);
