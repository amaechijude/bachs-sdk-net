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
    /// Deserializes a JSON string to the specified type using Bachs default conventions.
    /// </summary>
    public static T? Deserialize<T>(Stream message) =>
        JsonSerializer.Deserialize<T>(message, DefaultSerializerOptions);

    /// <summary>
    /// async Deserializes a JSON stream to the specified type using Bachs default conventions.
    /// </summary>
    public static ValueTask<T?> DeserializeAsync<T>(Stream message) =>
        JsonSerializer.DeserializeAsync<T>(message, DefaultSerializerOptions);

    /// <summary>
    /// Gets the shared default <see cref="JsonSerializerOptions"/> configured for the Bachs API.
    /// </summary>
    /// <returns>The default <see cref="JsonSerializerOptions"/>.</returns>
    public static JsonSerializerOptions CreateDefaultOptions() =>
        new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
            Converters = { new JsonStringEnumConverter(), new BachsPriceJsonConverter() },
        };
}
