using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bachs.Net.Serialization;

/// <summary>
/// Converts decimal values to and from decimal strings with 2 decimal places as expected by the Bachs API (e.g. "29.00").
/// </summary>
public class DecimalStringConverter : JsonConverter<decimal>
{
    public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return 0m;
            }

            if (decimal.TryParse(stringValue, NumberStyles.Number | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            throw new JsonException($"Unable to parse '{stringValue}' as decimal.");
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetDecimal();
        }

        throw new JsonException($"Unexpected token type {reader.TokenType} when parsing decimal.");
    }

    public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
    {
        // Bachs expects amounts as decimal strings with two decimal places (e.g., "29.00")
        writer.WriteStringValue(value.ToString("F2", CultureInfo.InvariantCulture));
    }
}

/// <summary>
/// Converts nullable decimal values to and from decimal strings with 2 decimal places.
/// </summary>
public class NullableDecimalStringConverter : JsonConverter<decimal?>
{
    public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var stringValue = reader.GetString();
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return null;
            }

            if (decimal.TryParse(stringValue, NumberStyles.Number | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            throw new JsonException($"Unable to parse '{stringValue}' as decimal.");
        }

        if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetDecimal();
        }

        throw new JsonException($"Unexpected token type {reader.TokenType} when parsing nullable decimal.");
    }

    public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
    {
        if (value.HasValue)
        {
            writer.WriteStringValue(value.Value.ToString("F2", CultureInfo.InvariantCulture));
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
