using System.Text.Json.Serialization;

namespace Bachs.Net.Models.Common;

/// <summary>
/// Represents a physical or billing address.
/// </summary>
public sealed record Address
{
    /// <summary>
    /// Address line 1 (e.g., street, PO Box, or company name).
    /// </summary>
    [JsonPropertyName("line1")]
    public string? Line1 { get; init; }

    /// <summary>
    /// Address line 2 (e.g., apartment, suite, unit, or building).
    /// </summary>
    [JsonPropertyName("line2")]
    public string? Line2 { get; init; }

    /// <summary>
    /// City, district, suburb, town, or village.
    /// </summary>
    [JsonPropertyName("city")]
    public string? City { get; init; }

    /// <summary>
    /// State, county, province, or region.
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; init; }

    /// <summary>
    /// ZIP or postal code.
    /// </summary>
    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; init; }

    /// <summary>
    /// Two-letter ISO 3166-1 alpha-2 country code (e.g., "US", "NG", "GB").
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; init; }
}
