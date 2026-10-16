namespace Bachs.Net.Common;

/// <summary>
/// Thrown when a request is rejected because the Bachs client's circuit breaker is open.
/// </summary>
/// <remarks>
/// Initializes a new instance of <see cref="BachsCircuitBreakerOpenException"/>.
/// </remarks>
/// <param name="retryAfter">The time remaining before a probe request can be attempted.</param>
public sealed class BachsCircuitBreakerOpenException(TimeSpan retryAfter)
    : BachsException(
        $"The Bachs client circuit breaker is open. Try again in {Math.Max(0, retryAfter.TotalSeconds):0.###} seconds."
    );

public sealed class BachsArgumentNullException(string message) : BachsException(message);
