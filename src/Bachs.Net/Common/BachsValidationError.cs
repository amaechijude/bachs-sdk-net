using System.Text.Json.Serialization;

namespace Bachs.Net.Common;

/// <summary>
/// Details about an individual field-level validation error returned by the Bachs API.
/// </summary>
public record BachsValidationError
{
    /// <summary>
    /// Gets the name of the request field that failed validation.
    /// </summary>
    [JsonPropertyName("field")]
    public string? Field { get; init; }

    /// <summary>
    /// Gets the human-readable explanation of why the field failed validation.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    /// <summary>
    /// Gets the category or type of validation failure (e.g., 'missing', 'invalid').
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// Gets an optional machine-readable error code for this field error.
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; init; }
}
