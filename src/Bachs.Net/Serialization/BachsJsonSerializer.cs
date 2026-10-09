using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bachs.Net.Serialization;

/// <summary>
/// Provides shared JSON serializer settings used across the Bachs SDK.
/// </summary>
public static class BachsJsonSerializer
{
    private static readonly JsonSerializerOptions DefaultSerializerOptions = CreateDefaultOptions();

    /// <summary>
    /// Gets the shared default <see cref="JsonSerializerOptions"/> configured for the Bachs API.
    /// </summary>
    public static JsonSerializerOptions Options => DefaultSerializerOptions;

    /// <summary>
    /// Serializes an object to a JSON string using Bachs default conventions.
    /// </summary>
    public static string Serialize<T>(T message) =>
        JsonSerializer.Serialize(message, DefaultSerializerOptions);

    /// <summary>
    /// Deserializes a JSON string to the specified type using Bachs default conventions.
    /// </summary>
    public static T? Deserialize<T>(string message) =>
        JsonSerializer.Deserialize<T>(message, DefaultSerializerOptions);

    /// <summary>
    /// Creates a new instance of <see cref="JsonSerializerOptions"/> configured with Bachs conventions.
    /// </summary>
    public static JsonSerializerOptions CreateDefaultOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        };

        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new DecimalStringConverter());
        options.Converters.Add(new NullableDecimalStringConverter());

        return options;
    }
}
