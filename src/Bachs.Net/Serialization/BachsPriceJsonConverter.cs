using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bachs.Net.Models.Common;

namespace Bachs.Net.Serialization;

public sealed class BachsPriceJsonConverter : JsonConverter<BachsPrice>
{
    public override BachsPrice Read(
        ref Utf8JsonReader reader,
        Type type,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType == JsonTokenType.Number)
            return new BachsPrice(reader.GetDecimal());

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
                return new BachsPrice(v);

            throw new JsonException($"Unable to parse '{str}' as Price.");
        }

        throw new JsonException($"Invalid price value (token: {reader.TokenType}).");
    }

    public override void Write(
        Utf8JsonWriter writer,
        BachsPrice value,
        JsonSerializerOptions options
    ) => writer.WriteStringValue(value.ToApiString());
}
