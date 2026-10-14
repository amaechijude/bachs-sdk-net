namespace Bachs.Net.Common;

/// <summary>
/// Options for paginated list requests to the Bachs API.
/// </summary>
public class BachsListRequestOptions : BachsRequestOptions
{
    /// <summary>
    /// Gets or sets the maximum number of items to return (1-100).
    /// Values above 100 are clamped by the Bachs API to 100. Default is typically 20.
    /// </summary>
    public int? Limit { get; set; }

    /// <summary>
    /// Gets or sets the pagination cursor to fetch the next or previous page.
    /// Takes precedence over <see cref="Offset"/>.
    /// </summary>
    public string? Cursor { get; set; }

    /// <summary>
    /// Gets or sets the record offset to start from. Ignored when <see cref="Cursor"/> is set.
    /// </summary>
    public int? Offset { get; set; }

    /// <summary>
    /// Converts the pagination options into URL query parameters.
    /// </summary>
    /// <returns>A dictionary of query parameter key-value pairs.</returns>
    public virtual IDictionary<string, string> ToQueryParameters()
    {
        var parameters = new Dictionary<string, string>();

        if (Limit.HasValue)
        {
            parameters["limit"] = Limit.Value.ToString();
        }

        if (!string.IsNullOrWhiteSpace(Cursor))
        {
            parameters["cursor"] = Cursor;
        }
        else if (Offset.HasValue)
        {
            parameters["offset"] = Offset.Value.ToString();
        }

        return parameters;
    }
}
