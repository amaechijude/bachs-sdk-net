using System.Globalization;
using System.Text.Json.Serialization;
using Bachs.Net.Serialization;

namespace Bachs.Net.Models.Common;

[JsonConverter(typeof(BachsPriceJsonConverter))]
public readonly record struct BachsPrice(decimal Value)
{
    /// <summary>From minor units, e.g. 2999 cents => 29.99.</summary>
    public static BachsPrice FromMinorUnits(long cents) => new(cents / 100m);

    /// <summary>From a double. Prefer decimal; doubles carry binary rounding error.</summary>
    public static BachsPrice FromDouble(double value) => new((decimal)value);

    /// <summary>
    /// Full currency units. int and long also work, via the built-in int -> decimal conversion.
    /// </summary>
    public static implicit operator BachsPrice(decimal value) => new(value);

    /// <summary>
    /// Implicit conversion to decimal for calculation and comparison.
    /// </summary>
    public static implicit operator decimal(BachsPrice price) => price.Value;

    /// <summary>The value as sent to the API: rounded to 2 dp, e.g. "29.00".</summary>
    public string ToApiString() =>
        Math.Round(Value, 2, MidpointRounding.AwayFromZero)
            .ToString("F2", CultureInfo.InvariantCulture);

    public override string ToString() => ToApiString();
}
