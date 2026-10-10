using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bachs.Net.Serialization;

public sealed class PriceJsonConverter : JsonConverter<Price>
{
    public override Price Read(
        ref Utf8JsonReader reader,
        Type type,
        JsonSerializerOptions options
    ) =>
        reader.TokenType switch
        {
            JsonTokenType.Number => new Price(reader.GetDecimal()),
            JsonTokenType.String
                when decimal.TryParse(
                    reader.GetString().AsSpan(),
                    NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture,
                    out var v
                ) => new Price(v),

            _ => throw new JsonException($"Invalid price value (token: {reader.TokenType})."),
        };

    public override void Write(Utf8JsonWriter writer, Price value, JsonSerializerOptions options) =>
        writer.WriteStringValue(value.ToApiString());
}

[JsonConverter(typeof(PriceJsonConverter))]
public readonly record struct Price(decimal Value)
{
    /// <summary>From minor units, e.g. 2999 cents => 29.99.</summary>
    public static Price FromMinorUnits(long cents) => new(cents / 100m);

    ///  <summary>From a double. Prefer decimal; doubles carry binary rounding error.</summary>
    public static Price FromDouble(double value) => new((decimal)value);

    /// <summary>
    ///  Full currency units. int and long also work, via the built-in int -> decimal conversion.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator Price(decimal value) => new(value);

    /// <summary>The value as sent to the API: rounded to 2 dp, e.g. "29.00".</summary>
    public string ToApiString() =>
        Math.Round(Value, 2, MidpointRounding.AwayFromZero)
            .ToString("F2", CultureInfo.InvariantCulture);
}
