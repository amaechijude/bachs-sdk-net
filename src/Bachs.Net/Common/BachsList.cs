using System.Text.Json.Serialization;

namespace Bachs.Net.Common;

/// <summary>
/// Represents a paginated list of resources returned by the Bachs API.
/// </summary>
/// <typeparam name="T">The type of items in the list.</typeparam>
public record BachsList<T>
{
    /// <summary>
    /// Gets the list of items for the current page.
    /// </summary>
    [JsonPropertyName("items")]
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();

    /// <summary>
    /// Gets the pagination metadata for this response.
    /// </summary>
    [JsonPropertyName("pagination")]
    public BachsPagination Pagination { get; init; } = new();

    /// <summary>
    /// Gets whether there are more items available on subsequent pages.
    /// </summary>
    [JsonIgnore]
    public bool HasMore => Pagination?.HasMore ?? false;

    /// <summary>
    /// Gets the cursor for retrieving the next page of results, or null if no further pages exist.
    /// </summary>
    [JsonIgnore]
    public string? NextCursor => Pagination?.NextCursor;

    /// <summary>
    /// Gets the cursor for retrieving the previous page of results, or null if on the first page.
    /// </summary>
    [JsonIgnore]
    public string? PrevCursor => Pagination?.PrevCursor;

    /// <summary>
    /// Gets the total number of items matching the query, if available.
    /// </summary>
    [JsonIgnore]
    public int? Total => Pagination?.Total;

    /// <summary>
    /// Gets the count of items in the current page.
    /// </summary>
    [JsonIgnore]
    public int Count => Items.Count;
}
