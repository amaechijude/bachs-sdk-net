using System.Text.Json.Serialization;

namespace Bachs.Net.Common;

/// <summary>
/// Details about the pagination state in a list response from the Bachs API.
/// </summary>
public sealed record BachsPagination
{
    /// <summary>
    /// Gets the cursor to pass on your next request to fetch the following page.
    /// Returns null on the last page.
    /// </summary>
    [JsonPropertyName("next_cursor")]
    public string? NextCursor { get; init; }

    /// <summary>
    /// Gets the cursor to pass to fetch the previous page.
    /// Returns null on the first page.
    /// </summary>
    [JsonPropertyName("prev_cursor")]
    public string? PrevCursor { get; init; }

    /// <summary>
    /// Gets whether more results exist beyond this page.
    /// </summary>
    [JsonPropertyName("has_more")]
    public bool HasMore { get; init; }

    /// <summary>
    /// Gets the page size actually applied, after clamping to the maximum (100).
    /// </summary>
    [JsonPropertyName("limit")]
    public int Limit { get; init; }

    /// <summary>
    /// Gets the record offset this page starts from.
    /// </summary>
    [JsonPropertyName("offset")]
    public int Offset { get; init; }

    /// <summary>
    /// Gets the number of items returned in this page.
    /// </summary>
    [JsonPropertyName("returned")]
    public int Returned { get; init; }

    /// <summary>
    /// Gets the total number of records matching the query.
    /// </summary>
    [JsonPropertyName("total")]
    public int? Total { get; init; }
}
