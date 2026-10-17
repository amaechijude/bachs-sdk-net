using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bachs.Net.Common;

/// <summary>
/// Represents the standard error response body returned by the Bachs API.
/// </summary>
public sealed record BachsError
{
    /// <summary>
    /// Gets the stable, machine-readable error code (e.g., 'VALIDATION_ERROR', 'NOT_FOUND', 'UNAUTHORIZED').
    /// </summary>
    [JsonPropertyName("error_code")]
    public BachsErrorCode ErrorCode { get; init; } = BachsErrorCode.UNKNOWN;

    /// <summary>
    /// Gets the human-readable description of what went wrong.
    /// </summary>
    [JsonPropertyName("detail")]
    public string Detail { get; init; } = string.Empty;

    /// <summary>
    /// Gets the link to documentation for this error in the Bachs Error Reference, if provided.
    /// </summary>
    [JsonPropertyName("doc_url")]
    public string? DocUrl { get; init; }

    /// <summary>
    /// Gets optional field-level details on validation errors.
    /// </summary>
    [JsonPropertyName("errors")]
    public IReadOnlyList<BachsValidationError>? Errors { get; init; }

    /// <summary>
    /// Gets optional structured context (such as 'requested_amount' and 'max_allowed_amount' on limit errors).
    /// </summary>
    [JsonPropertyName("details")]
    public JsonElement? Details { get; init; }
}
