using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bachs.Net.Serialization;

public sealed class PriceJsonConverter : JsonConverter<Price>
{
    public override Price Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Number)
            return new Price(reader.GetDecimal());

        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString().AsSpan();
            if (str.IsEmpty || str.IsWhiteSpace())
                return default;

            if (
                decimal.TryParse(
                    str,
                    NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture,
                    out var v
                )
            )
                return new Price(v);

            throw new JsonException($"Unable to parse '{str}' as Price.");
        }

        throw new JsonException($"Invalid price value (token: {reader.TokenType}).");
    }

    public override void Write(Utf8JsonWriter writer, Price value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToApiString());
}

[JsonConverter(typeof(PriceJsonConverter))]
public readonly record struct Price(decimal Value)
{
    /// <summary>From minor units, e.g. 2999 cents => 29.99.</summary>
    public static Price FromMinorUnits(long cents) => new(cents / 100m);

    /// <summary>From a double. Prefer decimal; doubles carry binary rounding error.</summary>
    public static Price FromDouble(double value) => new((decimal)value);

    /// <summary>
    /// Full currency units. int and long also work, via the built-in int -> decimal conversion.
    /// </summary>
    public static implicit operator Price(decimal value) => new(value);

    /// <summary>
    /// Implicit conversion to decimal for calculation and comparison.
    /// </summary>
    public static implicit operator decimal(Price price) => price.Value;

    /// <summary>The value as sent to the API: rounded to 2 dp, e.g. "29.00".</summary>
    public string ToApiString() =>
        Math.Round(Value, 2, MidpointRounding.AwayFromZero)
            .ToString("F2", CultureInfo.InvariantCulture);

    public override string ToString() => ToApiString();
}
