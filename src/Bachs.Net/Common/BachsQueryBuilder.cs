using System.Net;
using System.Text;

namespace Bachs.Net.Common;

/// <summary>
/// Helper for safely building URL query parameter strings.
/// </summary>
public static class BachsQueryBuilder
{
    /// <summary>
    /// Appends query parameters to a base URL path.
    /// </summary>
    /// <param name="path">The base path or URL.</param>
    /// <param name="parameters">The query parameters to append.</param>
    /// <returns>The combined URL path with encoded query parameters.</returns>
    public static string BuildUrl(string path, IDictionary<string, string>? parameters)
    {
        if (parameters == null || parameters.Count == 0)
        {
            return path;
        }

        var sb = new StringBuilder(path);
        var hasQuery = path.Contains('?');

        foreach (var (key, value) in parameters)
        {
            if (string.IsNullOrEmpty(key) || value == null)
            {
                continue;
            }

            sb.Append(hasQuery ? '&' : '?');
            sb.Append(WebUtility.UrlEncode(key));
            sb.Append('=');
            sb.Append(WebUtility.UrlEncode(value));
            hasQuery = true;
        }

        return sb.ToString();
    }
}
