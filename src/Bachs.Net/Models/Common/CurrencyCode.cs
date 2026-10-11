using System.Text.Json.Serialization;

namespace Bachs.Net.Models.Common;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Currency
{
    // Fiat currencies
    /// <summary>Ghanaian cedi.</summary>
    GHS,

    /// <summary>Kenyan shilling.</summary>
    KES,

    /// <summary>Malawian kwacha.</summary>
    MWK,

    /// <summary>Nigerian naira.</summary>
    NGN,

    /// <summary>Rwandan franc.</summary>
    RWF,

    /// <summary>Tanzanian shilling.</summary>
    TZS,

    /// <summary>Ugandan shilling.</summary>
    UGX,

    /// <summary>United States dollar.</summary>
    USD,

    /// <summary>Central African CFA franc.</summary>
    XAF,

    /// <summary>West African CFA franc.</summary>
    XOF,

    /// <summary>Zambian kwacha.</summary>
    ZMW,

    // Crypto currencies
    /// <summary>Bitcoin Cash on the Bitcoin Cash blockchain.</summary>
    BCH_BCH,

    /// <summary>BNB on the BNB Smart Chain (BEP-20).</summary>
    BNB_BEP20,

    /// <summary>Bitcoin on the Bitcoin blockchain.</summary>
    BTC_BTC,

    /// <summary>Celo on the Celo blockchain.</summary>
    CELO_CELO,

    /// <summary>Dogecoin on the Dogecoin blockchain.</summary>
    DOGE_DOGE,

    /// <summary>Ethereum on the Base network.</summary>
    ETH_BASE,

    /// <summary>Ethereum on the Ethereum blockchain.</summary>
    ETH_ETH,

    /// <summary>Litecoin on the Litecoin blockchain.</summary>
    LTC_LTC,

    /// <summary>Solana on the Solana blockchain.</summary>
    SOL_SOL,

    /// <summary>Toncoin on the TON blockchain.</summary>
    TON_TON,

    /// <summary>TRON on the TRC-20 network.</summary>
    TRX_TRC20,

    /// <summary>USD Coin on the Base network.</summary>
    USDC_BASE,

    /// <summary>USD Coin on the BNB Smart Chain (BEP-20).</summary>
    USDC_BEP20,

    /// <summary>USD Coin on the Celo blockchain.</summary>
    USDC_CELO,

    /// <summary>USD Coin on the Ethereum network (ERC-20).</summary>
    USDC_ERC20,

    /// <summary>USD Coin on the Solana blockchain.</summary>
    USDC_SOL,

    /// <summary>Tether on the BNB Smart Chain (BEP-20).</summary>
    USDT_BEP20,

    /// <summary>Tether on the Celo blockchain.</summary>
    USDT_CELO,

    /// <summary>Tether on the Ethereum network (ERC-20).</summary>
    USDT_ERC20,

    /// <summary>Tether on the Polygon network.</summary>
    USDT_POLYGON,

    /// <summary>Tether on the Solana blockchain.</summary>
    USDT_SOL,

    /// <summary>Tether on the TRON network (TRC-20).</summary>
    USDT_TRC20,
}
