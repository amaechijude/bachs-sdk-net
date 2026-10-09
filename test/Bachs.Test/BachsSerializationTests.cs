using Bachs.Net.Models.Checkout;
using Bachs.Net.Models.Products;
using Bachs.Net.Serialization;

namespace Bachs.Test;

public sealed class BachsSerializationTests
{
    [Fact]
    public void Serializes_DecimalAmount_AsTwoDecimalPlacesString()
    {
        var product = new ProductPrice
        {
            Amount = 29.5m,
            Currency = "USD",
            PriceType = "fixed"
        };

        var json = BachsJsonSerializer.Serialize(product);

        Assert.Contains("\"amount\":\"29.50\"", json);
        Assert.Contains("\"currency\":\"USD\"", json);
        Assert.Contains("\"price_type\":\"fixed\"", json);
    }

    [Fact]
    public void Deserializes_DecimalAmount_FromString()
    {
        var json = """{"amount":"75000.00","currency":"NGN","price_type":"fixed"}""";

        var price = BachsJsonSerializer.Deserialize<ProductPrice>(json);

        Assert.NotNull(price);
        Assert.Equal(75000.00m, price.Amount);
        Assert.Equal("NGN", price.Currency);
    }

    [Fact]
    public void Ignores_NullProperties_WhenSerializing()
    {
        var request = new CreateProductRequest
        {
            Name = "Starter Plan",
            Price = new ProductPriceRequest
            {
                Amount = 10.00m,
                Currency = "USD"
            },
            Description = null // should be omitted
        };

        var json = BachsJsonSerializer.Serialize(request);

        Assert.DoesNotContain("\"description\"", json);
    }
}
