using Bachs.Net.Models.Common;
using Bachs.Net.Models.Products;
using Bachs.Net.Models.Refunds;
using Bachs.Net.Models.Transfers;
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
            Currency = BachsCurrency.USD,
            PriceType = "fixed",
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
        Assert.Equal(BachsCurrency.NGN, price.Currency);
    }

    [Fact]
    public void Deserializes_Price_FromNumberToken()
    {
        var json = """{"amount":49.99,"currency":"USD","price_type":"fixed"}""";

        var price = BachsJsonSerializer.Deserialize<ProductPrice>(json);

        Assert.NotNull(price);
        Assert.Equal(new BachsPrice(49.99m), price.Amount);
        Assert.Equal(49.99m, (decimal)price.Amount!.Value);
    }

    [Fact]
    public void Price_ImplicitConversions_WorkBidirectionally()
    {
        // decimal -> Price
        BachsPrice price = 100.50m;
        Assert.Equal(100.50m, price.Value);

        // Price -> decimal
        decimal amount = price;
        Assert.Equal(100.50m, amount);

        // ToString() formats with 2 decimals
        Assert.Equal("100.50", price.ToString());
    }

    [Fact]
    public void RequestBodies_AreSealedRecords_AndSupportWithExpressions()
    {
        var transfer1 = new CreateTransferRequest
        {
            Amount = 250.00m,
            Currency = BachsCurrency.USD,
            Destination = "acct_target_123",
        };

        var transfer2 = transfer1 with { Amount = 350.00m };

        Assert.Equal(250.00m, (decimal)transfer1.Amount);
        Assert.Equal(350.00m, (decimal)transfer2.Amount);
        Assert.Equal(transfer1.Destination, transfer2.Destination);
    }

    [Fact]
    public void Serializes_CreateRefundRequest_WithPriceAmount()
    {
        var refundReq = new CreateRefundRequest
        {
            ChargeId = "ch_test_999",
            Reference = "ref-abc",
            Amount = 15.5m,
        };

        var json = BachsJsonSerializer.Serialize(refundReq);

        Assert.Contains("\"charge_id\":\"ch_test_999\"", json);
        Assert.Contains("\"amount\":\"15.50\"", json);
    }

    [Fact]
    public void Ignores_NullProperties_WhenSerializing()
    {
        var request = new CreateProductRequest
        {
            Name = "Starter Plan",
            Price = new ProductPriceRequest { Amount = 10.00m, Currency = BachsCurrency.USD },
            Description = null, // should be omitted
        };

        var json = BachsJsonSerializer.Serialize(request);

        Assert.DoesNotContain("\"description\"", json);
    }
}
